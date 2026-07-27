using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListMultipartUploadsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListMultipartUploadsResponseUnmarshaller _instance;

		public static ListMultipartUploadsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListMultipartUploadsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListMultipartUploadsResponse listMultipartUploadsResponse = new ListMultipartUploadsResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, listMultipartUploadsResponse);
				}
			}
			return listMultipartUploadsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListMultipartUploadsResponse response)
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
					if (context.TestExpression("Bucket", num))
					{
						response.BucketName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("KeyMarker", num))
					{
						response.KeyMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("UploadIdMarker", num))
					{
						response.UploadIdMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("NextKeyMarker", num))
					{
						response.NextKeyMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("NextUploadIdMarker", num))
					{
						response.NextUploadIdMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("MaxUploads", num))
					{
						response.MaxUploads = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Upload", num))
					{
						response.MultipartUploads.Add(MultipartUploadUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("Delimiter", num))
					{
						response.Delimiter = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Prefix", num))
					{
						response.Prefix = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("CommonPrefixes", num))
					{
						string text = CommonPrefixesItemUnmarshaller.Instance.Unmarshall(context);
						if (text != null)
						{
							response.CommonPrefixes.Add(text);
						}
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
