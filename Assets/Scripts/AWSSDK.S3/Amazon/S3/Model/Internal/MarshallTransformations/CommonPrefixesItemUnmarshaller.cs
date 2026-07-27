using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class CommonPrefixesItemUnmarshaller : IUnmarshaller<string, XmlUnmarshallerContext>, IUnmarshaller<string, JsonUnmarshallerContext>
	{
		private static CommonPrefixesItemUnmarshaller _instance;

		public static CommonPrefixesItemUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CommonPrefixesItemUnmarshaller();
				}
				return _instance;
			}
		}

		public string Unmarshall(XmlUnmarshallerContext context)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			string result = null;
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Prefix", num))
					{
						result = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return result;
				}
			}
			return result;
		}

		public string Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
