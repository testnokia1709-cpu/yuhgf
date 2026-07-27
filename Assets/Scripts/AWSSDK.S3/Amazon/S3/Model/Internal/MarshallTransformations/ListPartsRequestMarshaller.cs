using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListPartsRequestMarshaller : IMarshaller<IRequest, ListPartsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static ListPartsRequestMarshaller _instance;

		public static ListPartsRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListPartsRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListPartsRequest)input);
		}

		public IRequest Marshall(ListPartsRequest listPartsRequest)
		{
			IRequest request = new DefaultRequest(listPartsRequest, "AmazonS3");
			if (listPartsRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(listPartsRequest.RequestPayer.ToString()));
			}
			request.HttpMethod = "GET";
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(listPartsRequest.BucketName), S3Transforms.ToStringValue(listPartsRequest.Key));
			if (listPartsRequest.IsSetUploadId())
			{
				request.AddSubResource("uploadId", S3Transforms.ToStringValue(listPartsRequest.UploadId));
			}
			if (listPartsRequest.IsSetMaxParts())
			{
				request.Parameters.Add("max-parts", S3Transforms.ToStringValue(listPartsRequest.MaxParts));
			}
			if (listPartsRequest.IsSetPartNumberMarker())
			{
				request.Parameters.Add("part-number-marker", S3Transforms.ToStringValue(listPartsRequest.PartNumberMarker));
			}
			if (listPartsRequest.IsSetEncoding())
			{
				request.Parameters.Add("encoding-type", S3Transforms.ToStringValue(listPartsRequest.Encoding));
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
