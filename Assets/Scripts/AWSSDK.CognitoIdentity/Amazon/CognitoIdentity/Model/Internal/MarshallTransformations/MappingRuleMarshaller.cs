using Amazon.Runtime.Internal.Transform;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class MappingRuleMarshaller : IRequestMarshaller<MappingRule, JsonMarshallerContext>
	{
		public static readonly MappingRuleMarshaller Instance = new MappingRuleMarshaller();

		public void Marshall(MappingRule requestObject, JsonMarshallerContext context)
		{
			if (requestObject.IsSetClaim())
			{
				context.Writer.WritePropertyName("Claim");
				context.Writer.Write(requestObject.Claim);
			}
			if (requestObject.IsSetMatchType())
			{
				context.Writer.WritePropertyName("MatchType");
				context.Writer.Write(requestObject.MatchType);
			}
			if (requestObject.IsSetRoleARN())
			{
				context.Writer.WritePropertyName("RoleARN");
				context.Writer.Write(requestObject.RoleARN);
			}
			if (requestObject.IsSetValue())
			{
				context.Writer.WritePropertyName("Value");
				context.Writer.Write(requestObject.Value);
			}
		}
	}
}
