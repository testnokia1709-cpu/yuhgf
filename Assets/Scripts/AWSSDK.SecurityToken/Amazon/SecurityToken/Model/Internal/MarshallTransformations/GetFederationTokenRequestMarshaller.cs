using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetFederationTokenRequestMarshaller : IMarshaller<IRequest, GetFederationTokenRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetFederationTokenRequestMarshaller _instance = new GetFederationTokenRequestMarshaller();

		public static GetFederationTokenRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetFederationTokenRequest)input);
		}

		public IRequest Marshall(GetFederationTokenRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			request.Parameters.Add("Action", "GetFederationToken");
			request.Parameters.Add("Version", "2011-06-15");
			if (publicRequest != null)
			{
				if (publicRequest.IsSetDurationSeconds())
				{
					request.Parameters.Add("DurationSeconds", StringUtils.FromInt(publicRequest.DurationSeconds));
				}
				if (publicRequest.IsSetName())
				{
					request.Parameters.Add("Name", StringUtils.FromString(publicRequest.Name));
				}
				if (publicRequest.IsSetPolicy())
				{
					request.Parameters.Add("Policy", StringUtils.FromString(publicRequest.Policy));
				}
			}
			return request;
		}

		internal static GetFederationTokenRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
