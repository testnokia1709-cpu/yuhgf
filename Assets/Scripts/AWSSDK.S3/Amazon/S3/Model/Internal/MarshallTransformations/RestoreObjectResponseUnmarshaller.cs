using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class RestoreObjectResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static RestoreObjectResponseUnmarshaller _instance;

		public static RestoreObjectResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RestoreObjectResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			RestoreObjectResponse restoreObjectResponse = new RestoreObjectResponse();
			IWebResponseData responseData = context.ResponseData;
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				restoreObjectResponse.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRestoreOutputPath))
			{
				restoreObjectResponse.RestoreOutputPath = responseData.GetHeaderValue(S3Constants.AmzHeaderRestoreOutputPath);
			}
			return restoreObjectResponse;
		}
	}
}
