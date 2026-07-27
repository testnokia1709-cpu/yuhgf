using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class RoutingRuleConditionUnmarshaller : IUnmarshaller<RoutingRuleCondition, XmlUnmarshallerContext>, IUnmarshaller<RoutingRuleCondition, JsonUnmarshallerContext>
	{
		private static RoutingRuleConditionUnmarshaller _instance;

		public static RoutingRuleConditionUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RoutingRuleConditionUnmarshaller();
				}
				return _instance;
			}
		}

		public RoutingRuleCondition Unmarshall(XmlUnmarshallerContext context)
		{
			RoutingRuleCondition routingRuleCondition = new RoutingRuleCondition();
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("HttpErrorCodeReturnedEquals", num))
					{
						routingRuleCondition.HttpErrorCodeReturnedEquals = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("KeyPrefixEquals", num))
					{
						routingRuleCondition.KeyPrefixEquals = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return routingRuleCondition;
				}
			}
			return routingRuleCondition;
		}

		public RoutingRuleCondition Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
