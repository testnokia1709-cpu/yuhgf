using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class AccessControlTranslationUnmarshaller : IUnmarshaller<AccessControlTranslation, XmlUnmarshallerContext>, IUnmarshaller<AccessControlTranslation, JsonUnmarshallerContext>
	{
		private static AccessControlTranslationUnmarshaller _instance;

		public static AccessControlTranslationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AccessControlTranslationUnmarshaller();
				}
				return _instance;
			}
		}

		public AccessControlTranslation Unmarshall(XmlUnmarshallerContext context)
		{
			AccessControlTranslation accessControlTranslation = new AccessControlTranslation();
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
					if (context.TestExpression("Owner", num))
					{
						accessControlTranslation.Owner = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return accessControlTranslation;
				}
			}
			return accessControlTranslation;
		}

		public AccessControlTranslation Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
