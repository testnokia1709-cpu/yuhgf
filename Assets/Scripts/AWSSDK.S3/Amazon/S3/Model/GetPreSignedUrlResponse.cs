using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetPreSignedUrlResponse : AmazonWebServiceResponse
	{
		public string Url { get; internal set; }

		public GetPreSignedUrlResponse(string url)
		{
			Url = url;
		}
	}
}
