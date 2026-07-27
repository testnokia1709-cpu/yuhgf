using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListVersionsRequestMarshaller : IMarshaller<IRequest, ListVersionsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static ListVersionsRequestMarshaller _instance;

		public static ListVersionsRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListVersionsRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListVersionsRequest)input);
		}

		public IRequest Marshall(ListVersionsRequest listVersionsRequest)
		{
			IRequest request = new DefaultRequest(listVersionsRequest, "AmazonS3");
			request.HttpMethod = "GET";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(listVersionsRequest.BucketName);
			request.AddSubResource("versions");
			if (listVersionsRequest.IsSetDelimiter())
			{
				request.Parameters.Add("delimiter", S3Transforms.ToStringValue(listVersionsRequest.Delimiter));
			}
			if (listVersionsRequest.IsSetKeyMarker())
			{
				request.Parameters.Add("key-marker", S3Transforms.ToStringValue(listVersionsRequest.KeyMarker));
			}
			if (listVersionsRequest.IsSetMaxKeys())
			{
				request.Parameters.Add("max-keys", S3Transforms.ToStringValue(listVersionsRequest.MaxKeys));
			}
			if (listVersionsRequest.IsSetPrefix())
			{
				request.Parameters.Add("prefix", S3Transforms.ToStringValue(listVersionsRequest.Prefix));
			}
			if (listVersionsRequest.IsSetVersionIdMarker())
			{
				request.Parameters.Add("version-id-marker", S3Transforms.ToStringValue(listVersionsRequest.VersionIdMarker));
			}
			if (listVersionsRequest.IsSetEncoding())
			{
				request.Parameters.Add("encoding-type", S3Transforms.ToStringValue(listVersionsRequest.Encoding));
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
