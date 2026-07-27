using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	public class ParseFile : IJsonConvertible
	{
		private FileState state;

		private readonly Stream dataStream;

		private readonly TaskQueue taskQueue = new TaskQueue();

		public bool IsDirty
		{
			get
			{
				return state.Url == null;
			}
		}

		[ParseFieldName("name")]
		public string Name
		{
			get
			{
				return state.Name;
			}
		}

		public string MimeType
		{
			get
			{
				return state.MimeType;
			}
		}

		[ParseFieldName("url")]
		public Uri Url
		{
			get
			{
				return state.SecureUrl;
			}
		}

		internal static IParseFileController FileController
		{
			get
			{
				return ParseCorePlugins.Instance.FileController;
			}
		}

		internal ParseFile(string name, Uri uri, string mimeType = null)
		{
			state = new FileState
			{
				Name = name,
				Url = uri,
				MimeType = mimeType
			};
		}

		public ParseFile(string name, byte[] data, string mimeType = null)
			: this(name, new MemoryStream(data), mimeType)
		{
		}

		public ParseFile(string name, Stream data, string mimeType = null)
		{
			state = new FileState
			{
				Name = name,
				MimeType = mimeType
			};
			dataStream = data;
		}

		IDictionary<string, object> IJsonConvertible.ToJSON()
		{
			if (IsDirty)
			{
				throw new InvalidOperationException("ParseFile must be saved before it can be serialized.");
			}
			return new Dictionary<string, object>
			{
				{ "__type", "File" },
				{ "name", Name },
				{ "url", Url.AbsoluteUri }
			};
		}

		public Task SaveAsync()
		{
			return SaveAsync(null, CancellationToken.None);
		}

		public Task SaveAsync(CancellationToken cancellationToken)
		{
			return SaveAsync(null, cancellationToken);
		}

		public Task SaveAsync(IProgress<ParseUploadProgressEventArgs> progress)
		{
			return SaveAsync(progress, CancellationToken.None);
		}

		public Task SaveAsync(IProgress<ParseUploadProgressEventArgs> progress, CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => FileController.SaveAsync(state, dataStream, ParseUser.CurrentSessionToken, progress, cancellationToken), cancellationToken).OnSuccess(delegate(Task<FileState> t)
			{
				state = t.Result;
			});
		}
	}
}
