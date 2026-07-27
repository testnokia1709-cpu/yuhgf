using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Parse.Internal
{
	internal class HttpClient : IHttpClient
	{
		public Task<Tuple<HttpStatusCode, string>> ExecuteAsync(HttpRequest httpRequest, IProgress<ParseUploadProgressEventArgs> uploadProgress, IProgress<ParseDownloadProgressEventArgs> downloadProgress, CancellationToken cancellationToken)
		{
			TaskCompletionSource<Tuple<HttpStatusCode, string>> tcs = new TaskCompletionSource<Tuple<HttpStatusCode, string>>();
			cancellationToken.Register(delegate
			{
				tcs.TrySetCanceled();
			});
			uploadProgress = uploadProgress ?? new Progress<ParseUploadProgressEventArgs>();
			downloadProgress = downloadProgress ?? new Progress<ParseDownloadProgressEventArgs>();
			Hashtable headerTable = new Hashtable();
			if (httpRequest.Headers != null)
			{
				foreach (KeyValuePair<string, string> header in httpRequest.Headers)
				{
					headerTable[header.Key] = header.Value;
				}
			}
			if (!headerTable.ContainsKey("Content-Type"))
			{
				headerTable["Content-Type"] = "application/json";
			}
			Task task = null;
			IDisposable toDisposeAfterReading = null;
			byte[] bytes = null;
			if (!httpRequest.Method.Equals("POST") || httpRequest.Data == null)
			{
				bool noBody = httpRequest.Data == null;
				StreamReader streamReader = new StreamReader(httpRequest.Data ?? new MemoryStream(Encoding.UTF8.GetBytes("{}")));
				toDisposeAfterReading = streamReader;
				Task<string> task2 = ((!PlatformHooks.IsCompiledByIL2CPP) ? streamReader.ReadToEndAsync() : Task.FromResult(streamReader.ReadToEnd()));
				task = task2.OnSuccess(delegate(Task<string> t)
				{
					IDictionary<string, object> dictionary = Json.Parse(t.Result) as IDictionary<string, object>;
					dictionary["_method"] = httpRequest.Method;
					dictionary["_noBody"] = noBody;
					bytes = Encoding.UTF8.GetBytes(Json.Encode(dictionary));
				});
			}
			else
			{
				MemoryStream ms = new MemoryStream();
				toDisposeAfterReading = ms;
				task = httpRequest.Data.CopyToAsync(ms).OnSuccess(delegate
				{
					bytes = ms.ToArray();
				});
			}
			task.Safe().ContinueWith(delegate(Task t)
			{
				if (toDisposeAfterReading != null)
				{
					toDisposeAfterReading.Dispose();
				}
				return t;
			}).Unwrap()
				.OnSuccess(delegate
				{
					float oldDownloadProgress = 0f;
					float oldUploadProgress = 0f;
					PlatformHooks.RunOnMainThread(delegate
					{
						PlatformHooks.RegisterNetworkRequest(GenerateWWWInstance(httpRequest.Uri.AbsoluteUri, bytes, headerTable), delegate(WWW www)
						{
							if (cancellationToken.IsCancellationRequested)
							{
								tcs.TrySetCanceled();
							}
							else if (www.isDone)
							{
								uploadProgress.Report(new ParseUploadProgressEventArgs
								{
									Progress = 1.0
								});
								downloadProgress.Report(new ParseDownloadProgressEventArgs
								{
									Progress = 1.0
								});
								HttpStatusCode statusCode = GetStatusCode(www);
								if (!string.IsNullOrEmpty(www.error) && string.IsNullOrEmpty(www.text))
								{
									string item = string.Format("{{\"error\":\"{0}\"}}", www.error);
									tcs.TrySetResult(new Tuple<HttpStatusCode, string>(statusCode, item));
								}
								else
								{
									tcs.TrySetResult(new Tuple<HttpStatusCode, string>(statusCode, www.text));
								}
							}
							else
							{
								float uploadProgress2 = www.uploadProgress;
								if (oldUploadProgress < uploadProgress2)
								{
									uploadProgress.Report(new ParseUploadProgressEventArgs
									{
										Progress = uploadProgress2
									});
								}
								oldUploadProgress = uploadProgress2;
								float progress = www.progress;
								if (oldDownloadProgress < progress)
								{
									downloadProgress.Report(new ParseDownloadProgressEventArgs
									{
										Progress = progress
									});
								}
								oldDownloadProgress = progress;
							}
						});
					});
				});
			return tcs.Task.ContinueWith(delegate
			{
				TaskCompletionSource<object> dispatchTcs = new TaskCompletionSource<object>();
				if (PlatformHooks.IsCompiledByIL2CPP)
				{
					new Thread((ParameterizedThreadStart)delegate
					{
						dispatchTcs.TrySetResult(null);
					}).Start();
				}
				else
				{
					ThreadPool.QueueUserWorkItem(delegate
					{
						dispatchTcs.TrySetResult(null);
					});
				}
				return dispatchTcs.Task;
			}).Unwrap().ContinueWith((Task<object> _) => tcs.Task)
				.Unwrap();
		}

		private static HttpStatusCode GetStatusCode(WWW www)
		{
			if (string.IsNullOrEmpty(www.error))
			{
				return HttpStatusCode.Created;
			}
			string value = Regex.Match(www.error, "\\d+").Value;
			int result = 0;
			if (!int.TryParse(value, out result))
			{
				return HttpStatusCode.BadRequest;
			}
			return (HttpStatusCode)result;
		}

		private static WWW GenerateWWWInstance(string uri, byte[] bytes, Hashtable headerTable)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (DictionaryEntry item in headerTable)
			{
				dictionary[item.Key as string] = item.Value as string;
			}
			return new WWW(uri, bytes, dictionary);
		}
	}
}
