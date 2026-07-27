using System;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class MappingRuleUnmarshaller : IUnmarshaller<MappingRule, XmlUnmarshallerContext>, IUnmarshaller<MappingRule, JsonUnmarshallerContext>
	{
		private static MappingRuleUnmarshaller _instance = new MappingRuleUnmarshaller();

		public static MappingRuleUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		MappingRule IUnmarshaller<MappingRule, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public MappingRule Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return null;
			}
			MappingRule mappingRule = new MappingRule();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("Claim", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					mappingRule.Claim = instance.Unmarshall(context);
				}
				else if (context.TestExpression("MatchType", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					mappingRule.MatchType = instance2.Unmarshall(context);
				}
				else if (context.TestExpression("RoleARN", currentDepth))
				{
					StringUnmarshaller instance3 = StringUnmarshaller.Instance;
					mappingRule.RoleARN = instance3.Unmarshall(context);
				}
				else if (context.TestExpression("Value", currentDepth))
				{
					StringUnmarshaller instance4 = StringUnmarshaller.Instance;
					mappingRule.Value = instance4.Unmarshall(context);
				}
			}
			return mappingRule;
		}
	}
}
