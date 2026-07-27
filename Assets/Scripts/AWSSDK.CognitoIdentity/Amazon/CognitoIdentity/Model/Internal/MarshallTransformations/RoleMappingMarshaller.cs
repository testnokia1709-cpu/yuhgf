using Amazon.Runtime.Internal.Transform;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class RoleMappingMarshaller : IRequestMarshaller<RoleMapping, JsonMarshallerContext>
	{
		public static readonly RoleMappingMarshaller Instance = new RoleMappingMarshaller();

		public void Marshall(RoleMapping requestObject, JsonMarshallerContext context)
		{
			if (requestObject.IsSetAmbiguousRoleResolution())
			{
				context.Writer.WritePropertyName("AmbiguousRoleResolution");
				context.Writer.Write(requestObject.AmbiguousRoleResolution);
			}
			if (requestObject.IsSetRulesConfiguration())
			{
				context.Writer.WritePropertyName("RulesConfiguration");
				context.Writer.WriteObjectStart();
				RulesConfigurationTypeMarshaller.Instance.Marshall(requestObject.RulesConfiguration, context);
				context.Writer.WriteObjectEnd();
			}
			if (requestObject.IsSetType())
			{
				context.Writer.WritePropertyName("Type");
				context.Writer.Write(requestObject.Type);
			}
		}
	}
}
