using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class RestoreObjectResponse : AmazonWebServiceResponse
	{
		private RequestCharged requestCharged;

		private string restoreOutputPath;

		public RequestCharged RequestCharged
		{
			get
			{
				return requestCharged;
			}
			set
			{
				requestCharged = value;
			}
		}

		public string RestoreOutputPath
		{
			get
			{
				return restoreOutputPath;
			}
			set
			{
				restoreOutputPath = value;
			}
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}

		internal bool IsSetRestoreOutputPath()
		{
			return RestoreOutputPath != null;
		}
	}
}
