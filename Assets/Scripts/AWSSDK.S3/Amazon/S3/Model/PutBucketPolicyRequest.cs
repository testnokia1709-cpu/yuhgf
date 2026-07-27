using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketPolicyRequest : AmazonWebServiceRequest
	{
		private bool confirmRemoveSelfBucketAccess;

		public string BucketName { get; set; }

		public string ContentMD5 { get; set; }

		public string Policy { get; set; }

		public bool ConfirmRemoveSelfBucketAccess
		{
			get
			{
				return confirmRemoveSelfBucketAccess;
			}
			set
			{
				confirmRemoveSelfBucketAccess = value;
			}
		}

		protected override bool IncludeSHA256Header
		{
			get
			{
				return false;
			}
		}

		internal bool IsSetBucket()
		{
			return BucketName != null;
		}

		internal bool IsSetContentMD5()
		{
			return ContentMD5 != null;
		}

		internal bool IsSetPolicy()
		{
			return Policy != null;
		}

		internal bool IsSetConfirmRemoveSelfBucketAccess()
		{
			bool confirmRemoveSelfBucketAccess2 = ConfirmRemoveSelfBucketAccess;
			return true;
		}
	}
}
