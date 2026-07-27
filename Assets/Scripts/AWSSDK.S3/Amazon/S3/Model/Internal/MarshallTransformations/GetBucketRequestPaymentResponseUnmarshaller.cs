using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketRequestPaymentResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketRequestPaymentResponseUnmarshaller _instance;

		public static GetBucketRequestPaymentResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketRequestPaymentResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketRequestPaymentResponse getBucketRequestPaymentResponse = new GetBucketRequestPaymentResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketRequestPaymentResponse);
				}
			}
			return getBucketRequestPaymentResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketRequestPaymentResponse response)
		{
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
					if (context.TestExpression("Payer", num))
					{
						response.Payer = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					break;
				}
			}
		}
	}
}
