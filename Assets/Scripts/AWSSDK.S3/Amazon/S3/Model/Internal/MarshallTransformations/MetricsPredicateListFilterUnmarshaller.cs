using System.Collections.Generic;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class MetricsPredicateListFilterUnmarshaller : IUnmarshaller<List<MetricsFilterPredicate>, XmlUnmarshallerContext>, IUnmarshaller<List<MetricsFilterPredicate>, JsonUnmarshallerContext>
	{
		private static MetricsPredicateListFilterUnmarshaller _instance;

		public static MetricsPredicateListFilterUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MetricsPredicateListFilterUnmarshaller();
				}
				return _instance;
			}
		}

		public List<MetricsFilterPredicate> Unmarshall(XmlUnmarshallerContext context)
		{
			List<MetricsFilterPredicate> list = new List<MetricsFilterPredicate>();
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
						list.Add(new MetricsPrefixPredicate(StringUnmarshaller.Instance.Unmarshall(context)));
					}
					else if (context.TestExpression("Tag", num))
					{
						list.Add(new MetricsTagPredicate(TagUnmarshaller.Instance.Unmarshall(context)));
					}
					else if (context.TestExpression("And", num))
					{
						list.Add(new MetricsAndOperator(Unmarshall(context)));
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return list;
				}
			}
			return list;
		}

		public List<MetricsFilterPredicate> Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
