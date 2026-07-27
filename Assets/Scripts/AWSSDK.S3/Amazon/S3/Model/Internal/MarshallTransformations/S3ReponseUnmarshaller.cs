using System;
using System.IO;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public abstract class S3ReponseUnmarshaller : XmlResponseUnmarshaller
	{
		public override UnmarshallerContext CreateContext(IWebResponseData response, bool readEntireResponse, Stream stream, RequestMetrics metrics)
		{
			if (response.IsHeaderPresent("x-amz-id-2"))
			{
				metrics.AddProperty(Metric.AmzId2, response.GetHeaderValue("x-amz-id-2"));
			}
			if (response.IsHeaderPresent("X-Amz-Cf-Id"))
			{
				metrics.AddProperty(Metric.AmzCfId, response.GetHeaderValue("X-Amz-Cf-Id"));
			}
			return base.CreateContext(response, readEntireResponse, stream, metrics);
		}

		public override AmazonWebServiceResponse Unmarshall(UnmarshallerContext input)
		{
			AmazonWebServiceResponse amazonWebServiceResponse = base.Unmarshall(input);
			if (amazonWebServiceResponse.ResponseMetadata == null)
			{
				amazonWebServiceResponse.ResponseMetadata = new ResponseMetadata();
			}
			amazonWebServiceResponse.ResponseMetadata.Metadata.Add("x-amz-id-2", input.ResponseData.GetHeaderValue("x-amz-id-2"));
			if (input.ResponseData.IsHeaderPresent("X-Amz-Cf-Id"))
			{
				amazonWebServiceResponse.ResponseMetadata.Metadata.Add("X-Amz-Cf-Id", input.ResponseData.GetHeaderValue("X-Amz-Cf-Id"));
			}
			return amazonWebServiceResponse;
		}

		protected override UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData response)
		{
			return new S3UnmarshallerContext(responseStream, maintainResponseBody, response);
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			S3ErrorResponse s3ErrorResponse = S3ErrorResponseUnmarshaller.Instance.Unmarshall(context);
			AmazonS3Exception ex = new AmazonS3Exception(s3ErrorResponse.Message, innerException, s3ErrorResponse.Type, s3ErrorResponse.Code, s3ErrorResponse.RequestId, statusCode, s3ErrorResponse.Id2, s3ErrorResponse.AmzCfId);
			ex.Region = s3ErrorResponse.Region;
			if (s3ErrorResponse.ParsingException != null)
			{
				string responseBody = context.ResponseBody;
				if (!string.IsNullOrEmpty(responseBody))
				{
					ex.ResponseBody = responseBody;
				}
			}
			return ex;
		}
	}
}
