using System;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetCallerIdentityResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static GetCallerIdentityResponseUnmarshaller _instance = new GetCallerIdentityResponseUnmarshaller();

		public static GetCallerIdentityResponseUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetCallerIdentityResponse getCallerIdentityResponse = new GetCallerIdentityResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement)
				{
					if (context.TestExpression("GetCallerIdentityResult", 2))
					{
						UnmarshallResult(context, getCallerIdentityResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						getCallerIdentityResponse.ResponseMetadata = ResponseMetadataUnmarshaller.Instance.Unmarshall(context);
					}
				}
			}
			return getCallerIdentityResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetCallerIdentityResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Account", num))
					{
						StringUnmarshaller instance = StringUnmarshaller.Instance;
						response.Account = instance.Unmarshall(context);
					}
					else if (context.TestExpression("Arn", num))
					{
						StringUnmarshaller instance2 = StringUnmarshaller.Instance;
						response.Arn = instance2.Unmarshall(context);
					}
					else if (context.TestExpression("UserId", num))
					{
						StringUnmarshaller instance3 = StringUnmarshaller.Instance;
						response.UserId = instance3.Unmarshall(context);
					}
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			ErrorResponse errorResponse = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			return new AmazonSecurityTokenServiceException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
		}

		internal static GetCallerIdentityResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
