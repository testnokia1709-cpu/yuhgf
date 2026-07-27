using System;
using System.Net;
using System.Runtime.Serialization;
using Amazon.Runtime;

namespace Amazon.SecurityToken
{
	[Serializable]
	public class AmazonSecurityTokenServiceException : AmazonServiceException
	{
		public AmazonSecurityTokenServiceException(string message)
			: base(message)
		{
		}

		public AmazonSecurityTokenServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public AmazonSecurityTokenServiceException(Exception innerException)
			: base(innerException.Message, innerException)
		{
		}

		public AmazonSecurityTokenServiceException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		public AmazonSecurityTokenServiceException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		protected AmazonSecurityTokenServiceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
