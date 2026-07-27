using System;
using Amazon.Runtime.Internal.Auth;

namespace Amazon.Runtime.Internal
{
	public class AsyncRequestContext : RequestContext, IAsyncRequestContext, IRequestContext
	{
		public AsyncCallback Callback { get; set; }

		public object State { get; set; }

		public AsyncOptions AsyncOptions { get; set; }

		public Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> Action { get; set; }

		public AsyncRequestContext(bool enableMetrics, AbstractAWSSigner clientSigner)
			: base(enableMetrics, clientSigner)
		{
		}
	}
}
