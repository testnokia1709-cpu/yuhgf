using System;
using System.Net;
using System.Runtime.Serialization;
using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	[Serializable]
	public class PackedPolicyTooLargeException : AmazonSecurityTokenServiceException
	{
		public PackedPolicyTooLargeException(string message)
			: base(message)
		{
		}

		public PackedPolicyTooLargeException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public PackedPolicyTooLargeException(Exception innerException)
			: base(innerException)
		{
		}

		public PackedPolicyTooLargeException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public PackedPolicyTooLargeException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected PackedPolicyTooLargeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
