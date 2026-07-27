using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class StorageClassAnalysisDataExportUnmarshaller : IUnmarshaller<StorageClassAnalysisDataExport, XmlUnmarshallerContext>, IUnmarshaller<StorageClassAnalysisDataExport, JsonUnmarshallerContext>
	{
		private static StorageClassAnalysisDataExportUnmarshaller _instance;

		public static StorageClassAnalysisDataExportUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StorageClassAnalysisDataExportUnmarshaller();
				}
				return _instance;
			}
		}

		public StorageClassAnalysisDataExport Unmarshall(XmlUnmarshallerContext context)
		{
			StorageClassAnalysisDataExport storageClassAnalysisDataExport = new StorageClassAnalysisDataExport();
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
					if (context.TestExpression("OutputSchemaVersion", num))
					{
						storageClassAnalysisDataExport.OutputSchemaVersion = StorageClassAnalysisSchemaVersion.FindValue(StringUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("Destination", num))
					{
						storageClassAnalysisDataExport.Destination = AnalyticsExportDestinationUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return storageClassAnalysisDataExport;
				}
			}
			return storageClassAnalysisDataExport;
		}

		public StorageClassAnalysisDataExport Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
