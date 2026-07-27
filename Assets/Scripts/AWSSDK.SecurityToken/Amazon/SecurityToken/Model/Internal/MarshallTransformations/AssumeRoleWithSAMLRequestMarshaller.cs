using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleWithSAMLRequestMarshaller : IMarshaller<IRequest, AssumeRoleWithSAMLRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static AssumeRoleWithSAMLRequestMarshaller _instance = new AssumeRoleWithSAMLRequestMarshaller();

		public static AssumeRoleWithSAMLRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((AssumeRoleWithSAMLRequest)input);
		}

		public IRequest Marshall(AssumeRoleWithSAMLRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			request.Parameters.Add("Action", "AssumeRoleWithSAML");
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
				if (publicRequest.IsSetPrincipalArn())
				{
					request.Parameters.Add("PrincipalArn", StringUtils.FromString(publicRequest.PrincipalArn));
				}
				if (publicRequest.IsSetRoleArn())
				{
					request.Parameters.Add("RoleArn", StringUtils.FromString(publicRequest.RoleArn));
				}
				if (publicRequest.IsSetSAMLAssertion())
				{
					request.Parameters.Add("SAMLAssertion", StringUtils.FromString(publicRequest.SAMLAssertion));
				}
			}
			return request;
		}

		internal static AssumeRoleWithSAMLRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
