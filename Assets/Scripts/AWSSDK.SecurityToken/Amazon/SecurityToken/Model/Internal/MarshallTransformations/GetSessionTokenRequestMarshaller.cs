using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetSessionTokenRequestMarshaller : IMarshaller<IRequest, GetSessionTokenRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetSessionTokenRequestMarshaller _instance = new GetSessionTokenRequestMarshaller();

		public static GetSessionTokenRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetSessionTokenRequest)input);
		}

		public IRequest Marshall(GetSessionTokenRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			request.Parameters.Add("Action", "GetSessionToken");
			request.Parameters.Add("Version", "2011-06-15");
			if (publicRequest != null)
			{
				if (publicRequest.IsSetDurationSeconds())
				{
					request.Parameters.Add("DurationSeconds", StringUtils.FromInt(publicRequest.DurationSeconds));
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

		internal static GetSessionTokenRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
