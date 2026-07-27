using System;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.Runtime.Internal
{
	public class RequestContext : IRequestContext
	{
		private AbstractAWSSigner clientSigner;

		public IRequest Request { get; set; }

		public RequestMetrics Metrics { get; private set; }

		public IClientConfig ClientConfig { get; set; }

		public int Retries { get; set; }

		public bool IsSigned { get; set; }

		public bool IsAsync { get; set; }

		public AmazonWebServiceRequest OriginalRequest { get; set; }

		public IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; set; }

		public ResponseUnmarshaller Unmarshaller { get; set; }

		public ImmutableCredentials ImmutableCredentials { get; set; }

		public AbstractAWSSigner Signer
		{
			get
			{
				AbstractAWSSigner abstractAWSSigner = ((OriginalRequest == null) ? null : OriginalRequest.GetSigner());
				if (abstractAWSSigner == null)
				{
					return clientSigner;
				}
				return abstractAWSSigner;
			}
		}

		public string RequestName
		{
			get
			{
				return OriginalRequest.GetType().Name;
			}
		}

		public RequestContext(bool enableMetrics, AbstractAWSSigner clientSigner)
		{
			if (clientSigner == null)
			{
				throw new ArgumentNullException("clientSigner");
			}
			this.clientSigner = clientSigner;
			Metrics = new RequestMetrics();
			Metrics.IsEnabled = enableMetrics;
		}
	}
}
