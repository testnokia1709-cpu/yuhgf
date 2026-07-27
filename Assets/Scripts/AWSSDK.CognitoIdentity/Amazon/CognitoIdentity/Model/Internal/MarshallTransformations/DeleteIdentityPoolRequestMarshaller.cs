using System.Globalization;
using System.IO;
using System.Text;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class DeleteIdentityPoolRequestMarshaller : IMarshaller<IRequest, DeleteIdentityPoolRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteIdentityPoolRequestMarshaller _instance = new DeleteIdentityPoolRequestMarshaller();

		public static DeleteIdentityPoolRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteIdentityPoolRequest)input);
		}

		public IRequest Marshall(DeleteIdentityPoolRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.DeleteIdentityPool";
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

		internal static DeleteIdentityPoolRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
