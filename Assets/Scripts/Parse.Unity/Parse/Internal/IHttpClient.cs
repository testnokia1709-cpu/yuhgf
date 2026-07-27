using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal interface IHttpClient
	{
		Task<Tuple<HttpStatusCode, string>> ExecuteAsync(HttpRequest httpRequest, IProgress<ParseUploadProgressEventArgs> uploadProgress, IProgress<ParseDownloadProgressEventArgs> downloadProgress, CancellationToken cancellationToken);
	}
}
