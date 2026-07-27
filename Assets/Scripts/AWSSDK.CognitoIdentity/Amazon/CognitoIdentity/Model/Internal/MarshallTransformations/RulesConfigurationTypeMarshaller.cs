using Amazon.Runtime.Internal.Transform;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class RulesConfigurationTypeMarshaller : IRequestMarshaller<RulesConfigurationType, JsonMarshallerContext>
	{
		public static readonly RulesConfigurationTypeMarshaller Instance = new RulesConfigurationTypeMarshaller();

		public void Marshall(RulesConfigurationType requestObject, JsonMarshallerContext context)
		{
			if (!requestObject.IsSetRules())
			{
				return;
			}
			context.Writer.WritePropertyName("Rules");
			context.Writer.WriteArrayStart();
			foreach (MappingRule rule in requestObject.Rules)
			{
				context.Writer.WriteObjectStart();
				MappingRuleMarshaller.Instance.Marshall(rule, context);
				context.Writer.WriteObjectEnd();
			}
			context.Writer.WriteArrayEnd();
		}
	}
}
