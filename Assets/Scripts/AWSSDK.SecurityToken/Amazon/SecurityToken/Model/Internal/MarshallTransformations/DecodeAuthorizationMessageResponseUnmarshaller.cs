using System;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class DecodeAuthorizationMessageResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static DecodeAuthorizationMessageResponseUnmarshaller _instance = new DecodeAuthorizationMessageResponseUnmarshaller();

		public static DecodeAuthorizationMessageResponseUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			DecodeAuthorizationMessageResponse decodeAuthorizationMessageResponse = new DecodeAuthorizationMessageResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement)
				{
					if (context.TestExpression("DecodeAuthorizationMessageResult", 2))
					{
						UnmarshallResult(context, decodeAuthorizationMessageResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						decodeAuthorizationMessageResponse.ResponseMetadata = ResponseMetadataUnmarshaller.Instance.Unmarshall(context);
					}
				}
			}
			return decodeAuthorizationMessageResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, DecodeAuthorizationMessageResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			while (context.ReadAtDepth(currentDepth))
			{
				if ((context.IsStartElement || context.IsAttribute) && context.TestExpression("DecodedMessage", num))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					response.DecodedMessage = instance.Unmarshall(context);
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			ErrorResponse errorResponse = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (errorResponse.Code != null && errorResponse.Code.Equals("InvalidAuthorizationMessageException"))
			{
				return new InvalidAuthorizationMessageException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			return new AmazonSecurityTokenServiceException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
		}

		internal static DecodeAuthorizationMessageResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
