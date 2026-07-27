using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Parse.Utilities;

namespace Parse.Internal
{
	internal class ParseObjectController : IParseObjectController
	{
		private readonly IParseCommandRunner commandRunner;

		private const int MaximumBatchSize = 50;

		internal ParseObjectController(IParseCommandRunner commandRunner)
		{
			this.commandRunner = commandRunner;
		}

		public Task<IObjectState> FetchAsync(IObjectState state, string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand(string.Format("classes/{0}/{1}", Uri.EscapeDataString(state.ClassName), Uri.EscapeDataString(state.ObjectId)), "GET", sessionToken, (IList<KeyValuePair<string, string>>)null, (IDictionary<string, object>)null);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => ParseObjectCoder.Instance.Decode(t.Result.Item2, ParseDecoder.Instance));
		}

		public Task<IObjectState> SaveAsync(IObjectState state, IDictionary<string, IParseFieldOperation> operations, string sessionToken, CancellationToken cancellationToken)
		{
			IDictionary<string, object> data = ParseObject.ToJSONObjectForSaving(operations);
			ParseCommand command = new ParseCommand((state.ObjectId == null) ? string.Format("classes/{0}", Uri.EscapeDataString(state.ClassName)) : string.Format("classes/{0}/{1}", Uri.EscapeDataString(state.ClassName), state.ObjectId), (state.ObjectId == null) ? "POST" : "PUT", sessionToken, null, data);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => ParseObjectCoder.Instance.Decode(t.Result.Item2, ParseDecoder.Instance).MutatedClone(delegate(MutableObjectState mutableClone)
			{
				mutableClone.IsNew = t.Result.Item1 == HttpStatusCode.Created;
			}));
		}

		public IList<Task<IObjectState>> SaveAllAsync(IList<IObjectState> states, IList<IDictionary<string, IParseFieldOperation>> operationsList, string sessionToken, CancellationToken cancellationToken)
		{
			List<ParseCommand> requests = states.Zip(operationsList, (IObjectState item, IDictionary<string, IParseFieldOperation> ops) => new ParseCommand((item.ObjectId == null) ? string.Format("classes/{0}", Uri.EscapeDataString(item.ClassName)) : string.Format("classes/{0}/{1}", Uri.EscapeDataString(item.ClassName), Uri.EscapeDataString(item.ObjectId)), (item.ObjectId == null) ? "POST" : "PUT", null, null, ParseObject.ToJSONObjectForSaving(ops))).ToList();
			IList<Task<IDictionary<string, object>>> list = ExecuteBatchRequests(requests, sessionToken, cancellationToken);
			List<Task<IObjectState>> list2 = new List<Task<IObjectState>>();
			foreach (Task<IDictionary<string, object>> item in list)
			{
				list2.Add(item.OnSuccess((Task<IDictionary<string, object>> t) => ParseObjectCoder.Instance.Decode(t.Result, ParseDecoder.Instance)));
			}
			return list2;
		}

		public Task DeleteAsync(IObjectState state, string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand(string.Format("classes/{0}/{1}", state.ClassName, state.ObjectId), "DELETE", sessionToken, (IList<KeyValuePair<string, string>>)null, (IDictionary<string, object>)null);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken);
		}

		public IList<Task> DeleteAllAsync(IList<IObjectState> states, string sessionToken, CancellationToken cancellationToken)
		{
			List<ParseCommand> requests = (from item in states
				where item.ObjectId != null
				select new ParseCommand(string.Format("classes/{0}/{1}", Uri.EscapeDataString(item.ClassName), Uri.EscapeDataString(item.ObjectId)), "DELETE", (string)null, (IList<KeyValuePair<string, string>>)null, (IDictionary<string, object>)null)).ToList();
			return ExecuteBatchRequests(requests, sessionToken, cancellationToken).Cast<Task>().ToList();
		}

		internal IList<Task<IDictionary<string, object>>> ExecuteBatchRequests(IList<ParseCommand> requests, string sessionToken, CancellationToken cancellationToken)
		{
			List<Task<IDictionary<string, object>>> list = new List<Task<IDictionary<string, object>>>();
			int num = requests.Count;
			IEnumerable<ParseCommand> source = requests;
			while (num > 50)
			{
				List<ParseCommand> requests2 = source.Take(50).ToList();
				source = source.Skip(50);
				list.AddRange(ExecuteBatchRequest(requests2, sessionToken, cancellationToken));
				num = source.Count();
			}
			list.AddRange(ExecuteBatchRequest(source.ToList(), sessionToken, cancellationToken));
			return list;
		}

		private IList<Task<IDictionary<string, object>>> ExecuteBatchRequest(IList<ParseCommand> requests, string sessionToken, CancellationToken cancellationToken)
		{
			List<Task<IDictionary<string, object>>> list = new List<Task<IDictionary<string, object>>>();
			int batchSize = requests.Count;
			List<TaskCompletionSource<IDictionary<string, object>>> tcss = new List<TaskCompletionSource<IDictionary<string, object>>>();
			for (int i = 0; i < batchSize; i++)
			{
				TaskCompletionSource<IDictionary<string, object>> taskCompletionSource = new TaskCompletionSource<IDictionary<string, object>>();
				tcss.Add(taskCompletionSource);
				list.Add(taskCompletionSource.Task);
			}
			List<object> value = requests.Select(delegate(ParseCommand r)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{ "method", r.Method },
					{
						"path",
						r.Uri.AbsolutePath
					}
				};
				if (r.DataObject != null)
				{
					dictionary["body"] = r.DataObject;
				}
				return dictionary;
			}).Cast<object>().ToList();
			ParseCommand command = new ParseCommand("batch", "POST", sessionToken, null, new Dictionary<string, object> { { "requests", value } });
			commandRunner.RunCommandAsync(command, null, null, cancellationToken).ContinueWith(delegate(Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
			{
				if (t.IsFaulted || t.IsCanceled)
				{
					foreach (TaskCompletionSource<IDictionary<string, object>> item in tcss)
					{
						if (t.IsFaulted)
						{
							item.TrySetException(t.Exception);
						}
						else if (t.IsCanceled)
						{
							item.TrySetCanceled();
						}
					}
					return;
				}
				IList<object> list2 = Conversion.As<IList<object>>(t.Result.Item2["results"]);
				int count = list2.Count;
				if (count != batchSize)
				{
					foreach (TaskCompletionSource<IDictionary<string, object>> item2 in tcss)
					{
						item2.TrySetException(new InvalidOperationException("Batch command result count expected: " + batchSize + " but was: " + count + "."));
					}
					return;
				}
				for (int j = 0; j < batchSize; j++)
				{
					Dictionary<string, object> dictionary = list2[j] as Dictionary<string, object>;
					TaskCompletionSource<IDictionary<string, object>> taskCompletionSource2 = tcss[j];
					if (dictionary.ContainsKey("success"))
					{
						taskCompletionSource2.TrySetResult(dictionary["success"] as IDictionary<string, object>);
					}
					else if (dictionary.ContainsKey("error"))
					{
						IDictionary<string, object> dictionary2 = dictionary["error"] as IDictionary<string, object>;
						long num = (long)dictionary2["code"];
						taskCompletionSource2.TrySetException(new ParseException((ParseException.ErrorCode)num, dictionary2["error"] as string));
					}
					else
					{
						taskCompletionSource2.TrySetException(new InvalidOperationException("Invalid batch command response."));
					}
				}
			});
			return list;
		}
	}
}
