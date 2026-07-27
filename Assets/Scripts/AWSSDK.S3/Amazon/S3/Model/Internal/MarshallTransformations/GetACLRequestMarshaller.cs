using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetACLRequestMarshaller : IMarshaller<IRequest, GetACLRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetACLRequestMarshaller _instance;

		public static GetACLRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetACLRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetACLRequest)input);
		}

		public IRequest Marshall(GetACLRequest getObjectAclRequest)
		{
			IRequest request = new DefaultRequest(getObjectAclRequest, "AmazonS3");
			request.HttpMethod = "GET";
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(getObjectAclRequest.BucketName), S3Transforms.ToStringValue(getObjectAclRequest.Key));
			request.AddSubResource("acl");
			if (getObjectAclRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", S3Transforms.ToStringValue(getObjectAclRequest.VersionId));
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
