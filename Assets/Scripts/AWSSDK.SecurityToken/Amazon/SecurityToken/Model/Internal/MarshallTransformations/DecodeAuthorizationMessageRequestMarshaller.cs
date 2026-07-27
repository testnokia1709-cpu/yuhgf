using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class DecodeAuthorizationMessageRequestMarshaller : IMarshaller<IRequest, DecodeAuthorizationMessageRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DecodeAuthorizationMessageRequestMarshaller _instance = new DecodeAuthorizationMessageRequestMarshaller();

		public static DecodeAuthorizationMessageRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DecodeAuthorizationMessageRequest)input);
		}

		public IRequest Marshall(DecodeAuthorizationMessageRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.SecurityToken");
			request.Parameters.Add("Action", "DecodeAuthorizationMessage");
			request.Parameters.Add("Version", "2011-06-15");
			if (publicRequest != null && publicRequest.IsSetEncodedMessage())
			{
				request.Parameters.Add("EncodedMessage", StringUtils.FromString(publicRequest.EncodedMessage));
			}
			return request;
		}

		internal static DecodeAuthorizationMessageRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
