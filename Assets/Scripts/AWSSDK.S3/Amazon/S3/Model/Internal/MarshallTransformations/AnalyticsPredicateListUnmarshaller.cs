using System.Collections.Generic;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class AnalyticsPredicateListUnmarshaller : IUnmarshaller<List<AnalyticsFilterPredicate>, XmlUnmarshallerContext>, IUnmarshaller<List<AnalyticsFilterPredicate>, JsonUnmarshallerContext>
	{
		private static AnalyticsPredicateListUnmarshaller _instance;

		public static AnalyticsPredicateListUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AnalyticsPredicateListUnmarshaller();
				}
				return _instance;
			}
		}

		public List<AnalyticsFilterPredicate> Unmarshall(XmlUnmarshallerContext context)
		{
			List<AnalyticsFilterPredicate> list = new List<AnalyticsFilterPredicate>();
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
					if (context.TestExpression("Prefix", num))
					{
						list.Add(new AnalyticsPrefixPredicate(StringUnmarshaller.Instance.Unmarshall(context)));
					}
					else if (context.TestExpression("Tag", num))
					{
						list.Add(new AnalyticsTagPredicate(TagUnmarshaller.Instance.Unmarshall(context)));
					}
					else if (context.TestExpression("And", num))
					{
						list.Add(new AnalyticsAndOperator(Unmarshall(context)));
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return list;
				}
			}
			return list;
		}

		public List<AnalyticsFilterPredicate> Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
