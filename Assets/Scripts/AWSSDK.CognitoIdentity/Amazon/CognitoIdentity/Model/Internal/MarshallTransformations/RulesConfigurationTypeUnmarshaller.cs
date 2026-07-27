using System;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class RulesConfigurationTypeUnmarshaller : IUnmarshaller<RulesConfigurationType, XmlUnmarshallerContext>, IUnmarshaller<RulesConfigurationType, JsonUnmarshallerContext>
	{
		private static RulesConfigurationTypeUnmarshaller _instance = new RulesConfigurationTypeUnmarshaller();

		public static RulesConfigurationTypeUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		RulesConfigurationType IUnmarshaller<RulesConfigurationType, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public RulesConfigurationType Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return null;
			}
			RulesConfigurationType rulesConfigurationType = new RulesConfigurationType();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("Rules", currentDepth))
				{
					ListUnmarshaller<MappingRule, MappingRuleUnmarshaller> listUnmarshaller = new ListUnmarshaller<MappingRule, MappingRuleUnmarshaller>(MappingRuleUnmarshaller.Instance);
					rulesConfigurationType.Rules = listUnmarshaller.Unmarshall(context);
				}
			}
			return rulesConfigurationType;
		}
	}
}
