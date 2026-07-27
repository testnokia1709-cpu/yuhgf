using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class RoutingRuleUnmarshaller : IUnmarshaller<RoutingRule, XmlUnmarshallerContext>, IUnmarshaller<RoutingRule, JsonUnmarshallerContext>
	{
		private static RoutingRuleUnmarshaller _instance;

		public static RoutingRuleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RoutingRuleUnmarshaller();
				}
				return _instance;
			}
		}

		public RoutingRule Unmarshall(XmlUnmarshallerContext context)
		{
			RoutingRule routingRule = new RoutingRule();
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
					if (context.TestExpression("Condition", num))
					{
						routingRule.Condition = RoutingRuleConditionUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Redirect", num))
					{
						routingRule.Redirect = RoutingRuleRedirectUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return routingRule;
				}
			}
			return routingRule;
		}

		public RoutingRule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
