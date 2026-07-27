using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ErrorsItemUnmarshaller : IUnmarshaller<DeleteError, XmlUnmarshallerContext>, IUnmarshaller<DeleteError, JsonUnmarshallerContext>
	{
		private static ErrorsItemUnmarshaller _instance;

		public static ErrorsItemUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ErrorsItemUnmarshaller();
				}
				return _instance;
			}
		}

		public DeleteError Unmarshall(XmlUnmarshallerContext context)
		{
			DeleteError deleteError = new DeleteError();
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
					if (context.TestExpression("Code", num))
					{
						deleteError.Code = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Key", num))
					{
						deleteError.Key = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Message", num))
					{
						deleteError.Message = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("VersionId", num))
					{
						deleteError.VersionId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return deleteError;
				}
			}
			return deleteError;
		}

		public DeleteError Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
