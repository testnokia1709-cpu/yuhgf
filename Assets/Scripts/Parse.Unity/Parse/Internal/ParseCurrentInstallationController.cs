using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseCurrentInstallationController : IParseCurrentInstallationController, IParseObjectCurrentController<ParseInstallation>
	{
		private readonly object mutex = new object();

		private readonly TaskQueue taskQueue = new TaskQueue();

		private readonly IInstallationIdController installationIdController;

		private ParseInstallation currentInstallation;

		internal ParseInstallation CurrentInstallation
		{
			get
			{
				lock (mutex)
				{
					return currentInstallation;
				}
			}
			set
			{
				lock (mutex)
				{
					currentInstallation = value;
				}
			}
		}

		public ParseCurrentInstallationController(IInstallationIdController installationIdController)
		{
			this.installationIdController = installationIdController;
		}

		public Task SetAsync(ParseInstallation installation, CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith(delegate
			{
				if (installation == null)
				{
					ParseClient.ApplicationSettings.Remove("CurrentInstallation");
				}
				else
				{
					IDictionary<string, object> dictionary = installation.ServerDataToJSONObjectForSerialization();
					dictionary["objectId"] = installation.ObjectId;
					if (installation.CreatedAt.HasValue)
					{
						dictionary["createdAt"] = installation.CreatedAt.Value.ToString(ParseClient.DateFormatStrings.First());
					}
					if (installation.UpdatedAt.HasValue)
					{
						dictionary["updatedAt"] = installation.UpdatedAt.Value.ToString(ParseClient.DateFormatStrings.First());
					}
					ParseClient.ApplicationSettings["CurrentInstallation"] = Json.Encode(dictionary);
				}
				CurrentInstallation = installation;
			}), cancellationToken);
		}

		public Task<ParseInstallation> GetAsync(CancellationToken cancellationToken)
		{
			ParseInstallation parseInstallation = CurrentInstallation;
			if (parseInstallation != null)
			{
				return Task.FromResult(parseInstallation);
			}
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith(delegate
			{
				object value;
				ParseClient.ApplicationSettings.TryGetValue("CurrentInstallation", out value);
				string text = value as string;
				ParseInstallation parseInstallation2 = null;
				if (text != null)
				{
					IDictionary<string, object> data = ParseClient.DeserializeJsonString(text);
					parseInstallation2 = ParseObject.FromState<ParseInstallation>(ParseObjectCoder.Instance.Decode(data, ParseDecoder.Instance), "_Installation");
				}
				else
				{
					parseInstallation2 = ParseObject.Create<ParseInstallation>();
					parseInstallation2.SetIfDifferent("installationId", installationIdController.Get().ToString());
				}
				CurrentInstallation = parseInstallation2;
				return parseInstallation2;
			}), cancellationToken);
		}

		public Task<bool> ExistsAsync(CancellationToken cancellationToken)
		{
			if (CurrentInstallation != null)
			{
				return Task.FromResult(true);
			}
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith((Task t) => ParseClient.ApplicationSettings.ContainsKey("CurrentInstallation")), cancellationToken);
		}

		public bool IsCurrent(ParseInstallation installation)
		{
			return CurrentInstallation == installation;
		}

		public void ClearFromMemory()
		{
			CurrentInstallation = null;
		}

		public void ClearFromDisk()
		{
			ClearFromMemory();
			ParseClient.ApplicationSettings.Remove("CurrentInstallation");
		}
	}
}
