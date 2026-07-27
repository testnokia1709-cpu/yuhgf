using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GranteeUnmarshaller : IUnmarshaller<S3Grantee, XmlUnmarshallerContext>, IUnmarshaller<S3Grantee, JsonUnmarshallerContext>
	{
		private static GranteeUnmarshaller _instance;

		public static GranteeUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GranteeUnmarshaller();
				}
				return _instance;
			}
		}

		public S3Grantee Unmarshall(XmlUnmarshallerContext context)
		{
			S3Grantee s3Grantee = new S3Grantee();
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
					if (!context.TestExpression("@Type", num - 1))
					{
						if (context.TestExpression("DisplayName", num))
						{
							s3Grantee.DisplayName = StringUnmarshaller.GetInstance().Unmarshall(context);
						}
						else if (context.TestExpression("EmailAddress", num))
						{
							s3Grantee.EmailAddress = StringUnmarshaller.GetInstance().Unmarshall(context);
						}
						else if (context.TestExpression("ID", num))
						{
							s3Grantee.CanonicalUser = StringUnmarshaller.GetInstance().Unmarshall(context);
						}
						else if (context.TestExpression("URI", num))
						{
							s3Grantee.URI = StringUnmarshaller.GetInstance().Unmarshall(context);
						}
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return s3Grantee;
				}
			}
			return s3Grantee;
		}

		public S3Grantee Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
