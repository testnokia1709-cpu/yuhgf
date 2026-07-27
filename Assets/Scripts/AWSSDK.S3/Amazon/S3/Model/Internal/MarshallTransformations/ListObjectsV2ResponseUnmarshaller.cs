using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListObjectsV2ResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListObjectsV2ResponseUnmarshaller _instance;

		public static ListObjectsV2ResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListObjectsV2ResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListObjectsV2Response listObjectsV2Response = new ListObjectsV2Response();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, listObjectsV2Response);
				}
			}
			return listObjectsV2Response;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListObjectsV2Response response)
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
					if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Contents", num))
					{
						S3Object s3Object = ContentsItemUnmarshaller.Instance.Unmarshall(context);
						s3Object.BucketName = response.Name;
						response.S3Objects.Add(s3Object);
					}
					else if (context.TestExpression("Name", num))
					{
						response.Name = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Prefix", num))
					{
						response.Prefix = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Delimiter", num))
					{
						response.Delimiter = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("MaxKeys", num))
					{
						response.MaxKeys = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("CommonPrefixes", num))
					{
						string text = CommonPrefixesItemUnmarshaller.Instance.Unmarshall(context);
						if (text != null)
						{
							response.CommonPrefixes.Add(text);
						}
					}
					else if (context.TestExpression("EncodingType", num))
					{
						response.Encoding = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("KeyCount", num))
					{
						response.KeyCount = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ContinuationToken", num))
					{
						response.ContinuationToken = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("NextContinuationToken", num))
					{
						response.NextContinuationToken = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("StartAfter", num))
					{
						response.StartAfter = StringUnmarshaller.GetInstance().Unmarshall(context);
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
