using System;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleWithWebIdentityResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static AssumeRoleWithWebIdentityResponseUnmarshaller _instance = new AssumeRoleWithWebIdentityResponseUnmarshaller();

		public static AssumeRoleWithWebIdentityResponseUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			AssumeRoleWithWebIdentityResponse assumeRoleWithWebIdentityResponse = new AssumeRoleWithWebIdentityResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement)
				{
					if (context.TestExpression("AssumeRoleWithWebIdentityResult", 2))
					{
						UnmarshallResult(context, assumeRoleWithWebIdentityResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						assumeRoleWithWebIdentityResponse.ResponseMetadata = ResponseMetadataUnmarshaller.Instance.Unmarshall(context);
					}
				}
			}
			return assumeRoleWithWebIdentityResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, AssumeRoleWithWebIdentityResponse response)
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
					if (context.TestExpression("AssumedRoleUser", num))
					{
						AssumedRoleUserUnmarshaller instance = AssumedRoleUserUnmarshaller.Instance;
						response.AssumedRoleUser = instance.Unmarshall(context);
					}
					else if (context.TestExpression("Audience", num))
					{
						StringUnmarshaller instance2 = StringUnmarshaller.Instance;
						response.Audience = instance2.Unmarshall(context);
					}
					else if (context.TestExpression("Credentials", num))
					{
						CredentialsUnmarshaller instance3 = CredentialsUnmarshaller.Instance;
						response.Credentials = instance3.Unmarshall(context);
					}
					else if (context.TestExpression("PackedPolicySize", num))
					{
						IntUnmarshaller instance4 = IntUnmarshaller.Instance;
						response.PackedPolicySize = instance4.Unmarshall(context);
					}
					else if (context.TestExpression("Provider", num))
					{
						StringUnmarshaller instance5 = StringUnmarshaller.Instance;
						response.Provider = instance5.Unmarshall(context);
					}
					else if (context.TestExpression("SubjectFromWebIdentityToken", num))
					{
						StringUnmarshaller instance6 = StringUnmarshaller.Instance;
						response.SubjectFromWebIdentityToken = instance6.Unmarshall(context);
					}
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			ErrorResponse errorResponse = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (errorResponse.Code != null && errorResponse.Code.Equals("ExpiredTokenException"))
			{
				return new ExpiredTokenException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("IDPCommunicationError"))
			{
				return new IDPCommunicationErrorException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("IDPRejectedClaim"))
			{
				return new IDPRejectedClaimException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("InvalidIdentityToken"))
			{
				return new InvalidIdentityTokenException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
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

		internal static AssumeRoleWithWebIdentityResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
