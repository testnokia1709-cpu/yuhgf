using System;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class RoleMappingUnmarshaller : IUnmarshaller<RoleMapping, XmlUnmarshallerContext>, IUnmarshaller<RoleMapping, JsonUnmarshallerContext>
	{
		private static RoleMappingUnmarshaller _instance = new RoleMappingUnmarshaller();

		public static RoleMappingUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		RoleMapping IUnmarshaller<RoleMapping, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public RoleMapping Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return null;
			}
			RoleMapping roleMapping = new RoleMapping();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("AmbiguousRoleResolution", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					roleMapping.AmbiguousRoleResolution = instance.Unmarshall(context);
				}
				else if (context.TestExpression("RulesConfiguration", currentDepth))
				{
					RulesConfigurationTypeUnmarshaller instance2 = RulesConfigurationTypeUnmarshaller.Instance;
					roleMapping.RulesConfiguration = instance2.Unmarshall(context);
				}
				else if (context.TestExpression("Type", currentDepth))
				{
					StringUnmarshaller instance3 = StringUnmarshaller.Instance;
					roleMapping.Type = instance3.Unmarshall(context);
				}
			}
			return roleMapping;
		}
	}
}
