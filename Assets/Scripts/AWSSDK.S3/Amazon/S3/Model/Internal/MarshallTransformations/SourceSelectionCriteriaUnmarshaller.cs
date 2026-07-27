using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class SourceSelectionCriteriaUnmarshaller : IUnmarshaller<SourceSelectionCriteria, XmlUnmarshallerContext>, IUnmarshaller<SourceSelectionCriteria, JsonUnmarshallerContext>
	{
		private static SourceSelectionCriteriaUnmarshaller _instance;

		public static SourceSelectionCriteriaUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SourceSelectionCriteriaUnmarshaller();
				}
				return _instance;
			}
		}

		public SourceSelectionCriteria Unmarshall(XmlUnmarshallerContext context)
		{
			SourceSelectionCriteria sourceSelectionCriteria = new SourceSelectionCriteria();
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
					if (context.TestExpression("SseKmsEncryptedObjects", num))
					{
						sourceSelectionCriteria.SseKmsEncryptedObjects = SseKmsEncryptedObjectsUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return sourceSelectionCriteria;
				}
			}
			return sourceSelectionCriteria;
		}

		public SourceSelectionCriteria Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
