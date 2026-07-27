using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketsRequestMarshaller : IMarshaller<IRequest, ListBucketsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static ListBucketsRequestMarshaller _instance;

		public static ListBucketsRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListBucketsRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListBucketsRequest)input);
		}

		public IRequest Marshall(ListBucketsRequest listBucketsRequest)
		{
			return new DefaultRequest(listBucketsRequest, "AmazonS3")
			{
				HttpMethod = "GET",
				ResourcePath = "/",
				UseQueryString = true
			};
		}
	}
}
