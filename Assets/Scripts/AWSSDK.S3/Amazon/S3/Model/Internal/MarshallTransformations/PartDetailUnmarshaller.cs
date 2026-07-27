using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PartDetailUnmarshaller : IUnmarshaller<PartDetail, XmlUnmarshallerContext>, IUnmarshaller<PartDetail, JsonUnmarshallerContext>
	{
		private static PartDetailUnmarshaller _instance;

		public static PartDetailUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PartDetailUnmarshaller();
				}
				return _instance;
			}
		}

		public PartDetail Unmarshall(XmlUnmarshallerContext context)
		{
			PartDetail partDetail = new PartDetail();
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
					if (context.TestExpression("ETag", num))
					{
						partDetail.ETag = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("LastModified", num))
					{
						partDetail.LastModified = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("PartNumber", num))
					{
						partDetail.PartNumber = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Size", num))
					{
						partDetail.Size = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return partDetail;
				}
			}
			return partDetail;
		}

		public PartDetail Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
