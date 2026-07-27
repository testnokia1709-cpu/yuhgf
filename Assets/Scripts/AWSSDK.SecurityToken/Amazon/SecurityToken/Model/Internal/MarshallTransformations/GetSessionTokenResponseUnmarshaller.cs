using System;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetSessionTokenResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static GetSessionTokenResponseUnmarshaller _instance = new GetSessionTokenResponseUnmarshaller();

		public static GetSessionTokenResponseUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetSessionTokenResponse getSessionTokenResponse = new GetSessionTokenResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement)
				{
					if (context.TestExpression("GetSessionTokenResult", 2))
					{
						UnmarshallResult(context, getSessionTokenResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						getSessionTokenResponse.ResponseMetadata = ResponseMetadataUnmarshaller.Instance.Unmarshall(context);
					}
				}
			}
			return getSessionTokenResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetSessionTokenResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			while (context.ReadAtDepth(currentDepth))
			{
				if ((context.IsStartElement || context.IsAttribute) && context.TestExpression("Credentials", num))
				{
					CredentialsUnmarshaller instance = CredentialsUnmarshaller.Instance;
					response.Credentials = instance.Unmarshall(context);
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			ErrorResponse errorResponse = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (errorResponse.Code != null && errorResponse.Code.Equals("RegionDisabledException"))
			{
				return new RegionDisabledException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			return new AmazonSecurityTokenServiceException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
		}

		internal static GetSessionTokenResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
