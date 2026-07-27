using System.Globalization;
using System.Text;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutObjectTaggingRequestMarshaller : IMarshaller<IRequest, PutObjectTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutObjectTaggingRequestMarshaller _instance;

		public static PutObjectTaggingRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutObjectTaggingRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutObjectTaggingRequest)input);
		}

		public IRequest Marshall(PutObjectTaggingRequest putObjectTaggingRequest)
		{
			IRequest request = new DefaultRequest(putObjectTaggingRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(putObjectTaggingRequest.BucketName), S3Transforms.ToStringValue(putObjectTaggingRequest.Key));
			request.AddSubResource("tagging");
			if (putObjectTaggingRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", putObjectTaggingRequest.VersionId);
			}
			try
			{
				string text = AmazonS3Util.SerializeTaggingToXml(putObjectTaggingRequest.Tagging);
				request.Content = Encoding.UTF8.GetBytes(text);
				request.Headers["Content-Type"] = "application/xml";
				string value = AmazonS3Util.GenerateChecksumForContent(text, true);
				request.Headers["Content-MD5"] = value;
				return request;
			}
			catch (EncoderFallbackException innerException)
			{
				throw new AmazonServiceException("Unable to marhsall request to XML", innerException);
			}
		}
	}
}
