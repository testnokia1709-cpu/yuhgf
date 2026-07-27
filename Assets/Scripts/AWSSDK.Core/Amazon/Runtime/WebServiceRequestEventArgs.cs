using System;
using System.Collections.Generic;
using Amazon.Runtime.Internal;

namespace Amazon.Runtime
{
	public class WebServiceRequestEventArgs : RequestEventArgs
	{
		public IDictionary<string, string> Headers { get; protected set; }

		[Obsolete("Parameters property has been deprecated in favor of the ParameterCollection property")]
		public IDictionary<string, string> Parameters { get; protected set; }

		public ParameterCollection ParameterCollection { get; protected set; }

		public string ServiceName { get; protected set; }

		public Uri Endpoint { get; protected set; }

		public AmazonWebServiceRequest Request { get; protected set; }

		[Obsolete("OriginalRequest property has been deprecated in favor of the Request property")]
		public AmazonWebServiceRequest OriginalRequest
		{
			get
			{
				return Request;
			}
		}

		protected WebServiceRequestEventArgs()
		{
		}

		internal static WebServiceRequestEventArgs Create(IRequest request)
		{
			return new WebServiceRequestEventArgs
			{
				Headers = request.Headers,
				Parameters = request.Parameters,
				ParameterCollection = request.ParameterCollection,
				ServiceName = request.ServiceName,
				Request = request.OriginalRequest,
				Endpoint = request.Endpoint
			};
		}
	}
}
