using System.Globalization;
using System.IO;
using System.Text;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class UnlinkDeveloperIdentityRequestMarshaller : IMarshaller<IRequest, UnlinkDeveloperIdentityRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static UnlinkDeveloperIdentityRequestMarshaller _instance = new UnlinkDeveloperIdentityRequestMarshaller();

		public static UnlinkDeveloperIdentityRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((UnlinkDeveloperIdentityRequest)input);
		}

		public IRequest Marshall(UnlinkDeveloperIdentityRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.UnlinkDeveloperIdentity";
			request.Headers["X-Amz-Target"] = value;
			request.Headers["Content-Type"] = "application/x-amz-json-1.1";
			request.HttpMethod = "POST";
			string resourcePath = "/";
			request.ResourcePath = resourcePath;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				JsonWriter jsonWriter = new JsonWriter(stringWriter);
				jsonWriter.WriteObjectStart();
				JsonMarshallerContext jsonMarshallerContext = new JsonMarshallerContext(request, jsonWriter);
				if (publicRequest.IsSetDeveloperProviderName())
				{
					jsonMarshallerContext.Writer.WritePropertyName("DeveloperProviderName");
					jsonMarshallerContext.Writer.Write(publicRequest.DeveloperProviderName);
				}
				if (publicRequest.IsSetDeveloperUserIdentifier())
				{
					jsonMarshallerContext.Writer.WritePropertyName("DeveloperUserIdentifier");
					jsonMarshallerContext.Writer.Write(publicRequest.DeveloperUserIdentifier);
				}
				if (publicRequest.IsSetIdentityId())
				{
					jsonMarshallerContext.Writer.WritePropertyName("IdentityId");
					jsonMarshallerContext.Writer.Write(publicRequest.IdentityId);
				}
				if (publicRequest.IsSetIdentityPoolId())
				{
					jsonMarshallerContext.Writer.WritePropertyName("IdentityPoolId");
					jsonMarshallerContext.Writer.Write(publicRequest.IdentityPoolId);
				}
				jsonWriter.WriteObjectEnd();
				string s = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(s);
				return request;
			}
		}

		internal static UnlinkDeveloperIdentityRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
