using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseFileController : IParseFileController
	{
		private readonly IParseCommandRunner commandRunner;

		internal ParseFileController(IParseCommandRunner commandRunner)
		{
			this.commandRunner = commandRunner;
		}

		public Task<FileState> SaveAsync(FileState state, Stream dataStream, string sessionToken, IProgress<ParseUploadProgressEventArgs> progress, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (state.Url != null)
			{
				return Task.FromResult(state);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				TaskCompletionSource<FileState> taskCompletionSource = new TaskCompletionSource<FileState>();
				taskCompletionSource.TrySetCanceled();
				return taskCompletionSource.Task;
			}
			long oldPosition = dataStream.Position;
			ParseCommand command = new ParseCommand("files/" + state.Name, "POST", sessionToken, null, contentType: state.MimeType, stream: dataStream);
			return commandRunner.RunCommandAsync(command, progress, null, cancellationToken).OnSuccess(delegate(Task<Tuple<HttpStatusCode, IDictionary<string, object>>> uploadTask)
			{
				IDictionary<string, object> item = uploadTask.Result.Item2;
				cancellationToken.ThrowIfCancellationRequested();
				return new FileState
				{
					Name = (item["name"] as string),
					Url = new Uri(item["url"] as string, UriKind.Absolute),
					MimeType = state.MimeType
				};
			}).ContinueWith(delegate(Task<FileState> t)
			{
				if ((t.IsFaulted || t.IsCanceled) && dataStream.CanSeek)
				{
					dataStream.Seek(oldPosition, SeekOrigin.Begin);
				}
				return t;
			})
				.Unwrap();
		}
	}
}
