using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketLocationResponse : AmazonWebServiceResponse
	{
		private string location;

		public S3Region Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
			}
		}
	}
}
