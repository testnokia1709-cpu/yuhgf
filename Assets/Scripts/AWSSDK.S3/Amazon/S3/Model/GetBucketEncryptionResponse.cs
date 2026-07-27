using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketEncryptionResponse : AmazonWebServiceResponse
	{
		private ServerSideEncryptionConfiguration serverSideEncryptionConfiguration;

		public ServerSideEncryptionConfiguration ServerSideEncryptionConfiguration
		{
			get
			{
				return serverSideEncryptionConfiguration;
			}
			set
			{
				serverSideEncryptionConfiguration = value;
			}
		}

		internal bool IsSetServerSideEncryptionConfiguration()
		{
			return serverSideEncryptionConfiguration != null;
		}
	}
}
