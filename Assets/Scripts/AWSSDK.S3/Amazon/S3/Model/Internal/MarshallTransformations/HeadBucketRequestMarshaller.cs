using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class HeadBucketRequestMarshaller : IMarshaller<IRequest, HeadBucketRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static HeadBucketRequestMarshaller _instance;

		public static HeadBucketRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new HeadBucketRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((HeadBucketRequest)input);
		}

		public IRequest Marshall(HeadBucketRequest headBucketRequest)
		{
			return new DefaultRequest(headBucketRequest, "AmazonS3")
			{
				HttpMethod = "HEAD",
				ResourcePath = "/" + S3Transforms.ToStringValue(headBucketRequest.BucketName),
				UseQueryString = true
			};
		}
	}
}
