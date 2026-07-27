using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class FilterUnmarshaller : IUnmarshaller<Filter, XmlUnmarshallerContext>, IUnmarshaller<Filter, JsonUnmarshallerContext>
	{
		private static FilterUnmarshaller _instance;

		public static FilterUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new FilterUnmarshaller();
				}
				return _instance;
			}
		}

		public Filter Unmarshall(XmlUnmarshallerContext context)
		{
			Filter filter = new Filter();
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
					if (context.TestExpression("S3Key", num))
					{
						filter.S3KeyFilter = S3KeyFilterUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return filter;
				}
			}
			return filter;
		}

		public Filter Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
