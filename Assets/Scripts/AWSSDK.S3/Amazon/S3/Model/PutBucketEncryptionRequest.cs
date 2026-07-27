using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketEncryptionRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string contentMD5;

		private ServerSideEncryptionConfiguration serverSideEncryptionConfiguration;

		public string BucketName
		{
			get
			{
				return bucketName;
			}
			set
			{
				bucketName = value;
			}
		}

		public string ContentMD5
		{
			get
			{
				return contentMD5;
			}
			set
			{
				contentMD5 = value;
			}
		}

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

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetContentMD5()
		{
			return contentMD5 != null;
		}

		internal bool IsSetServerSideEncryptionConfiguration()
		{
			return serverSideEncryptionConfiguration != null;
		}
	}
}
