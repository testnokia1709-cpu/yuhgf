using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleRequestMarshaller : IMarshaller<IRequest, AssumeRoleRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static AssumeRoleRequestMarshaller _instance = new AssumeRoleRequestMarshaller();

		public static AssumeRoleRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((AssumeRoleRequest)input);
		}

		public IRequest Marshall(AssumeRoleRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			request.Parameters.Add("Action", "AssumeRole");
			request.Parameters.Add("Version", "2011-06-15");
			if (publicRequest != null)
			{
				if (publicRequest.IsSetDurationSeconds())
				{
					request.Parameters.Add("DurationSeconds", StringUtils.FromInt(publicRequest.DurationSeconds));
				}
				if (publicRequest.IsSetExternalId())
				{
					request.Parameters.Add("ExternalId", StringUtils.FromString(publicRequest.ExternalId));
				}
				if (publicRequest.IsSetPolicy())
				{
					request.Parameters.Add("Policy", StringUtils.FromString(publicRequest.Policy));
				}
				if (publicRequest.IsSetRoleArn())
				{
					request.Parameters.Add("RoleArn", StringUtils.FromString(publicRequest.RoleArn));
				}
				if (publicRequest.IsSetRoleSessionName())
				{
					request.Parameters.Add("RoleSessionName", StringUtils.FromString(publicRequest.RoleSessionName));
				}
				if (publicRequest.IsSetSerialNumber())
				{
					request.Parameters.Add("SerialNumber", StringUtils.FromString(publicRequest.SerialNumber));
				}
				if (publicRequest.IsSetTokenCode())
				{
					request.Parameters.Add("TokenCode", StringUtils.FromString(publicRequest.TokenCode));
				}
			}
			return request;
		}

		internal static AssumeRoleRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
