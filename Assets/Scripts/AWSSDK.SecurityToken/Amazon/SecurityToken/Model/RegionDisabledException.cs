using System;
using System.Net;
using System.Runtime.Serialization;
using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	[Serializable]
	public class RegionDisabledException : AmazonSecurityTokenServiceException
	{
		public RegionDisabledException(string message)
			: base(message)
		{
		}

		public RegionDisabledException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public RegionDisabledException(Exception innerException)
			: base(innerException)
		{
		}

		public RegionDisabledException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public RegionDisabledException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected RegionDisabledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
