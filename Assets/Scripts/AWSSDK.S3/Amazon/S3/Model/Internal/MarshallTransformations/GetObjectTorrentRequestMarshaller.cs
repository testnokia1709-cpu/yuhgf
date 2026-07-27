using System;
using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectTorrentRequestMarshaller : IMarshaller<IRequest, GetObjectTorrentRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetObjectTorrentRequestMarshaller _instance;

		public static GetObjectTorrentRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetObjectTorrentRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetObjectTorrentRequest)input);
		}

		public IRequest Marshall(GetObjectTorrentRequest getObjectTorrentRequest)
		{
			if (string.IsNullOrEmpty(getObjectTorrentRequest.Key))
			{
				throw new ArgumentException("Key is a required property and must be set before making this call.", "GetObjectTorrentRequest.Key");
			}
			IRequest request = new DefaultRequest(getObjectTorrentRequest, "AmazonS3");
			request.HttpMethod = "GET";
			if (getObjectTorrentRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(getObjectTorrentRequest.RequestPayer.ToString()));
			}
			if (getObjectTorrentRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(getObjectTorrentRequest.RequestPayer.ToString()));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(getObjectTorrentRequest.BucketName), S3Transforms.ToStringValue(getObjectTorrentRequest.Key));
			request.AddSubResource("torrent");
			request.UseQueryString = true;
			return request;
		}
	}
}
