using System;
using System.Net;
using System.Runtime.Serialization;
using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	[Serializable]
	public class InvalidIdentityTokenException : AmazonSecurityTokenServiceException
	{
		public InvalidIdentityTokenException(string message)
			: base(message)
		{
		}

		public InvalidIdentityTokenException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public InvalidIdentityTokenException(Exception innerException)
			: base(innerException)
		{
		}

		public InvalidIdentityTokenException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public InvalidIdentityTokenException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected InvalidIdentityTokenException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
