using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleWithWebIdentityRequestMarshaller : IMarshaller<IRequest, AssumeRoleWithWebIdentityRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static AssumeRoleWithWebIdentityRequestMarshaller _instance = new AssumeRoleWithWebIdentityRequestMarshaller();

		public static AssumeRoleWithWebIdentityRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((AssumeRoleWithWebIdentityRequest)input);
		}

		public IRequest Marshall(AssumeRoleWithWebIdentityRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			request.Parameters.Add("Action", "AssumeRoleWithWebIdentity");
			request.Parameters.Add("Version", "2011-06-15");
			if (publicRequest != null)
			{
				if (publicRequest.IsSetDurationSeconds())
				{
					request.Parameters.Add("DurationSeconds", StringUtils.FromInt(publicRequest.DurationSeconds));
				}
				if (publicRequest.IsSetPolicy())
				{
					request.Parameters.Add("Policy", StringUtils.FromString(publicRequest.Policy));
				}
				if (publicRequest.IsSetProviderId())
				{
					request.Parameters.Add("ProviderId", StringUtils.FromString(publicRequest.ProviderId));
				}
				if (publicRequest.IsSetRoleArn())
				{
					request.Parameters.Add("RoleArn", StringUtils.FromString(publicRequest.RoleArn));
				}
				if (publicRequest.IsSetRoleSessionName())
				{
					request.Parameters.Add("RoleSessionName", StringUtils.FromString(publicRequest.RoleSessionName));
				}
				if (publicRequest.IsSetWebIdentityToken())
				{
					request.Parameters.Add("WebIdentityToken", StringUtils.FromString(publicRequest.WebIdentityToken));
				}
			}
			return request;
		}

		internal static AssumeRoleWithWebIdentityRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
