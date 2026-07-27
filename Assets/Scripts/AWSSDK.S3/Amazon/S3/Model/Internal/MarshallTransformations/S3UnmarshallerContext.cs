using System.IO;
using System.Net;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class S3UnmarshallerContext : XmlUnmarshallerContext
	{
		private bool _checkedForErrorResponse;

		public S3UnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData responseData)
			: base(responseStream, maintainResponseBody, responseData)
		{
		}

		public override bool Read()
		{
			bool result = base.Read();
			if (base.ResponseData.StatusCode == HttpStatusCode.OK && !_checkedForErrorResponse && IsStartElement)
			{
				if (TestExpression("Error", 1))
				{
					S3ErrorResponse s3ErrorResponse = new S3ErrorResponseUnmarshaller().Unmarshall(this);
					throw new AmazonS3Exception(s3ErrorResponse.Message, null, s3ErrorResponse.Type, s3ErrorResponse.Code, s3ErrorResponse.RequestId, base.ResponseData.StatusCode, s3ErrorResponse.Id2, s3ErrorResponse.AmzCfId)
					{
						Region = s3ErrorResponse.Region
					};
				}
				_checkedForErrorResponse = true;
			}
			return result;
		}
	}
}
