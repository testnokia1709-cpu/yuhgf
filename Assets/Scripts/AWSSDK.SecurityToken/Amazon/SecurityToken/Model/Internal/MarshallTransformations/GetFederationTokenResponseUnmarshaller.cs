using System;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetFederationTokenResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static GetFederationTokenResponseUnmarshaller _instance = new GetFederationTokenResponseUnmarshaller();

		public static GetFederationTokenResponseUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetFederationTokenResponse getFederationTokenResponse = new GetFederationTokenResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement)
				{
					if (context.TestExpression("GetFederationTokenResult", 2))
					{
						UnmarshallResult(context, getFederationTokenResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						getFederationTokenResponse.ResponseMetadata = ResponseMetadataUnmarshaller.Instance.Unmarshall(context);
					}
				}
			}
			return getFederationTokenResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetFederationTokenResponse response)
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
					if (context.TestExpression("Credentials", num))
					{
						CredentialsUnmarshaller instance = CredentialsUnmarshaller.Instance;
						response.Credentials = instance.Unmarshall(context);
					}
					else if (context.TestExpression("FederatedUser", num))
					{
						FederatedUserUnmarshaller instance2 = FederatedUserUnmarshaller.Instance;
						response.FederatedUser = instance2.Unmarshall(context);
					}
					else if (context.TestExpression("PackedPolicySize", num))
					{
						IntUnmarshaller instance3 = IntUnmarshaller.Instance;
						response.PackedPolicySize = instance3.Unmarshall(context);
					}
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			ErrorResponse errorResponse = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (errorResponse.Code != null && errorResponse.Code.Equals("MalformedPolicyDocument"))
			{
				return new MalformedPolicyDocumentException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("PackedPolicyTooLarge"))
			{
				return new PackedPolicyTooLargeException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("RegionDisabledException"))
			{
				return new RegionDisabledException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			return new AmazonSecurityTokenServiceException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
		}

		internal static GetFederationTokenResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
