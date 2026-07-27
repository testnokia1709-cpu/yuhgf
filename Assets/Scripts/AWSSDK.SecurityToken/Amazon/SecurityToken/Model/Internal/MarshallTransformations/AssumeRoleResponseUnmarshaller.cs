using System;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static AssumeRoleResponseUnmarshaller _instance = new AssumeRoleResponseUnmarshaller();

		public static AssumeRoleResponseUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			AssumeRoleResponse assumeRoleResponse = new AssumeRoleResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement)
				{
					if (context.TestExpression("AssumeRoleResult", 2))
					{
						UnmarshallResult(context, assumeRoleResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						assumeRoleResponse.ResponseMetadata = ResponseMetadataUnmarshaller.Instance.Unmarshall(context);
					}
				}
			}
			return assumeRoleResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, AssumeRoleResponse response)
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
					else if (context.TestExpression("Credentials", num))
					{
						CredentialsUnmarshaller instance2 = CredentialsUnmarshaller.Instance;
						response.Credentials = instance2.Unmarshall(context);
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

		internal static AssumeRoleResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
