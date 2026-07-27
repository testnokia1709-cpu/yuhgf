using System;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleWithSAMLResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static AssumeRoleWithSAMLResponseUnmarshaller _instance = new AssumeRoleWithSAMLResponseUnmarshaller();

		public static AssumeRoleWithSAMLResponseUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			AssumeRoleWithSAMLResponse assumeRoleWithSAMLResponse = new AssumeRoleWithSAMLResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement)
				{
					if (context.TestExpression("AssumeRoleWithSAMLResult", 2))
					{
						UnmarshallResult(context, assumeRoleWithSAMLResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						assumeRoleWithSAMLResponse.ResponseMetadata = ResponseMetadataUnmarshaller.Instance.Unmarshall(context);
					}
				}
			}
			return assumeRoleWithSAMLResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, AssumeRoleWithSAMLResponse response)
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
					else if (context.TestExpression("Issuer", num))
					{
						StringUnmarshaller instance4 = StringUnmarshaller.Instance;
						response.Issuer = instance4.Unmarshall(context);
					}
					else if (context.TestExpression("NameQualifier", num))
					{
						StringUnmarshaller instance5 = StringUnmarshaller.Instance;
						response.NameQualifier = instance5.Unmarshall(context);
					}
					else if (context.TestExpression("PackedPolicySize", num))
					{
						IntUnmarshaller instance6 = IntUnmarshaller.Instance;
						response.PackedPolicySize = instance6.Unmarshall(context);
					}
					else if (context.TestExpression("Subject", num))
					{
						StringUnmarshaller instance7 = StringUnmarshaller.Instance;
						response.Subject = instance7.Unmarshall(context);
					}
					else if (context.TestExpression("SubjectType", num))
					{
						StringUnmarshaller instance8 = StringUnmarshaller.Instance;
						response.SubjectType = instance8.Unmarshall(context);
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

		internal static AssumeRoleWithSAMLResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
