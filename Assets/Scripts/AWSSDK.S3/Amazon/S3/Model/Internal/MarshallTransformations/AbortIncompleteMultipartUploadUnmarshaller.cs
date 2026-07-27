using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class AbortIncompleteMultipartUploadUnmarshaller : IUnmarshaller<LifecycleRuleAbortIncompleteMultipartUpload, XmlUnmarshallerContext>, IUnmarshaller<LifecycleRuleAbortIncompleteMultipartUpload, JsonUnmarshallerContext>
	{
		private static AbortIncompleteMultipartUploadUnmarshaller _instance;

		public static AbortIncompleteMultipartUploadUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AbortIncompleteMultipartUploadUnmarshaller();
				}
				return _instance;
			}
		}

		public LifecycleRuleAbortIncompleteMultipartUpload Unmarshall(XmlUnmarshallerContext context)
		{
			LifecycleRuleAbortIncompleteMultipartUpload lifecycleRuleAbortIncompleteMultipartUpload = new LifecycleRuleAbortIncompleteMultipartUpload();
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
					if (context.TestExpression("DaysAfterInitiation", num))
					{
						lifecycleRuleAbortIncompleteMultipartUpload.DaysAfterInitiation = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return lifecycleRuleAbortIncompleteMultipartUpload;
				}
			}
			return lifecycleRuleAbortIncompleteMultipartUpload;
		}

		public LifecycleRuleAbortIncompleteMultipartUpload Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
