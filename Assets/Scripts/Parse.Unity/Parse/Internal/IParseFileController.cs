using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal interface IParseFileController
	{
		Task<FileState> SaveAsync(FileState state, Stream dataStream, string sessionToken, IProgress<ParseUploadProgressEventArgs> progress, CancellationToken cancellationToken);
	}
}
