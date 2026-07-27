using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class FilterRuleUnmarshaller : IUnmarshaller<FilterRule, XmlUnmarshallerContext>, IUnmarshaller<FilterRule, JsonUnmarshallerContext>
	{
		private static FilterRuleUnmarshaller _instance;

		public static FilterRuleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new FilterRuleUnmarshaller();
				}
				return _instance;
			}
		}

		public FilterRule Unmarshall(XmlUnmarshallerContext context)
		{
			FilterRule filterRule = new FilterRule();
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
					if (context.TestExpression("Name", num))
					{
						filterRule.Name = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Value", num))
					{
						filterRule.Value = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return filterRule;
				}
			}
			return filterRule;
		}

		public FilterRule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
