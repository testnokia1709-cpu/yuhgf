using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListObjectsV2RequestMarshaller : IMarshaller<IRequest, ListObjectsV2Request>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static ListObjectsV2RequestMarshaller _instance;

		public static ListObjectsV2RequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListObjectsV2RequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListObjectsV2Request)input);
		}

		public IRequest Marshall(ListObjectsV2Request listObjectsRequest)
		{
			IRequest request = new DefaultRequest(listObjectsRequest, "AmazonS3");
			if (listObjectsRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(listObjectsRequest.RequestPayer.ToString()));
			}
			request.HttpMethod = "GET";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(listObjectsRequest.BucketName);
			if (listObjectsRequest.IsSetDelimiter())
			{
				request.Parameters.Add("delimiter", S3Transforms.ToStringValue(listObjectsRequest.Delimiter));
			}
			if (listObjectsRequest.IsSetEncoding())
			{
				request.Parameters.Add("encoding-type", S3Transforms.ToStringValue(listObjectsRequest.Encoding));
			}
			if (listObjectsRequest.IsSetMaxKeys())
			{
				request.Parameters.Add("max-keys", S3Transforms.ToStringValue(listObjectsRequest.MaxKeys));
			}
			if (listObjectsRequest.IsSetPrefix())
			{
				request.Parameters.Add("prefix", S3Transforms.ToStringValue(listObjectsRequest.Prefix));
			}
			if (listObjectsRequest.IsSetContinuationToken())
			{
				request.Parameters.Add("continuation-token", S3Transforms.ToStringValue(listObjectsRequest.ContinuationToken));
			}
			if (listObjectsRequest.IsSetFetchOwner())
			{
				request.Parameters.Add("fetch-owner", listObjectsRequest.FetchOwner.ToString().ToLowerInvariant());
			}
			if (listObjectsRequest.IsSetStartAfter())
			{
				request.Parameters.Add("start-after", S3Transforms.ToStringValue(listObjectsRequest.StartAfter));
			}
			request.Parameters.Add("list-type", "2");
			request.UseQueryString = true;
			return request;
		}
	}
}
