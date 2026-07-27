using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class StorageClassAnalysisUnmarshaller : IUnmarshaller<StorageClassAnalysis, XmlUnmarshallerContext>, IUnmarshaller<StorageClassAnalysis, JsonUnmarshallerContext>
	{
		private static StorageClassAnalysisUnmarshaller _instance;

		public static StorageClassAnalysisUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StorageClassAnalysisUnmarshaller();
				}
				return _instance;
			}
		}

		public StorageClassAnalysis Unmarshall(XmlUnmarshallerContext context)
		{
			StorageClassAnalysis storageClassAnalysis = new StorageClassAnalysis();
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
					if (context.TestExpression("DataExport", num))
					{
						storageClassAnalysis.DataExport = StorageClassAnalysisDataExportUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return storageClassAnalysis;
				}
			}
			return storageClassAnalysis;
		}

		public StorageClassAnalysis Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
