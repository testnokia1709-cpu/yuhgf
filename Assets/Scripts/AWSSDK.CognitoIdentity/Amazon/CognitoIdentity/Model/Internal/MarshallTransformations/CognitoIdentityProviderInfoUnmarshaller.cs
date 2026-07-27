using System;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class CognitoIdentityProviderInfoUnmarshaller : IUnmarshaller<CognitoIdentityProviderInfo, XmlUnmarshallerContext>, IUnmarshaller<CognitoIdentityProviderInfo, JsonUnmarshallerContext>
	{
		private static CognitoIdentityProviderInfoUnmarshaller _instance = new CognitoIdentityProviderInfoUnmarshaller();

		public static CognitoIdentityProviderInfoUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		CognitoIdentityProviderInfo IUnmarshaller<CognitoIdentityProviderInfo, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public CognitoIdentityProviderInfo Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return null;
			}
			CognitoIdentityProviderInfo cognitoIdentityProviderInfo = new CognitoIdentityProviderInfo();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("ClientId", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					cognitoIdentityProviderInfo.ClientId = instance.Unmarshall(context);
				}
				else if (context.TestExpression("ProviderName", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					cognitoIdentityProviderInfo.ProviderName = instance2.Unmarshall(context);
				}
				else if (context.TestExpression("ServerSideTokenCheck", currentDepth))
				{
					BoolUnmarshaller instance3 = BoolUnmarshaller.Instance;
					cognitoIdentityProviderInfo.ServerSideTokenCheck = instance3.Unmarshall(context);
				}
			}
			return cognitoIdentityProviderInfo;
		}
	}
}
