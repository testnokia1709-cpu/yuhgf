using System.IO;
using System.Text;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutBucketPolicyRequestMarshaller : IMarshaller<IRequest, PutBucketPolicyRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketPolicyRequestMarshaller _instance;

		public static PutBucketPolicyRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketPolicyRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketPolicyRequest)input);
		}

		public IRequest Marshall(PutBucketPolicyRequest putBucketPolicyRequest)
		{
			IRequest request = new DefaultRequest(putBucketPolicyRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			if (putBucketPolicyRequest.IsSetContentMD5())
			{
				request.Headers.Add("Content-MD5", S3Transforms.ToStringValue(putBucketPolicyRequest.ContentMD5));
			}
			if (!request.Headers.ContainsKey("Content-Type"))
			{
				request.Headers.Add("Content-Type", "text/plain");
			}
			if (putBucketPolicyRequest.IsSetConfirmRemoveSelfBucketAccess())
			{
				request.Headers.Add("x-amz-confirm-remove-self-bucket-access", putBucketPolicyRequest.ConfirmRemoveSelfBucketAccess.ToString().ToLowerInvariant());
			}
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketPolicyRequest.BucketName);
			request.AddSubResource("policy");
			request.ContentStream = new MemoryStream(Encoding.UTF8.GetBytes(putBucketPolicyRequest.Policy));
			return request;
		}
	}
}
