using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class TagUnmarshaller : IUnmarshaller<Tag, XmlUnmarshallerContext>, IUnmarshaller<Tag, JsonUnmarshallerContext>
	{
		private static TagUnmarshaller _instance;

		public static TagUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TagUnmarshaller();
				}
				return _instance;
			}
		}

		public Tag Unmarshall(XmlUnmarshallerContext context)
		{
			Tag tag = new Tag();
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
					if (context.TestExpression("Key", num))
					{
						tag.Key = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Value", num))
					{
						tag.Value = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return tag;
				}
			}
			return tag;
		}

		public Tag Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
