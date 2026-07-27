using System.Collections.Generic;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class LifecycleFilterPredicateListUnmarshaller : IUnmarshaller<List<LifecycleFilterPredicate>, XmlUnmarshallerContext>, IUnmarshaller<List<LifecycleFilterPredicate>, JsonUnmarshallerContext>
	{
		private static LifecycleFilterPredicateListUnmarshaller _instance;

		public static LifecycleFilterPredicateListUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LifecycleFilterPredicateListUnmarshaller();
				}
				return _instance;
			}
		}

		public List<LifecycleFilterPredicate> Unmarshall(XmlUnmarshallerContext context)
		{
			List<LifecycleFilterPredicate> list = new List<LifecycleFilterPredicate>();
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
						string prefix = StringUnmarshaller.Instance.Unmarshall(context);
						list.Add(new LifecyclePrefixPredicate
						{
							Prefix = prefix
						});
					}
					if (context.TestExpression("Tag", num))
					{
						Tag tag = TagUnmarshaller.Instance.Unmarshall(context);
						list.Add(new LifecycleTagPredicate
						{
							Tag = tag
						});
					}
					if (context.TestExpression("And", num))
					{
						list.Add(new LifecycleAndOperator
						{
							Operands = Unmarshall(context)
						});
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return list;
				}
			}
			return list;
		}

		public List<LifecycleFilterPredicate> Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
