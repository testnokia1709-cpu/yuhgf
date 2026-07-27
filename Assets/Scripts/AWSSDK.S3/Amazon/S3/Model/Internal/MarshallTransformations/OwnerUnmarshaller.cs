using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class OwnerUnmarshaller : IUnmarshaller<Owner, XmlUnmarshallerContext>, IUnmarshaller<Owner, JsonUnmarshallerContext>
	{
		private static OwnerUnmarshaller _instance;

		public static OwnerUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new OwnerUnmarshaller();
				}
				return _instance;
			}
		}

		public Owner Unmarshall(XmlUnmarshallerContext context)
		{
			Owner owner = new Owner();
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
					if (context.TestExpression("DisplayName", num))
					{
						owner.DisplayName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ID", num))
					{
						owner.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return owner;
				}
			}
			return owner;
		}

		public Owner Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
