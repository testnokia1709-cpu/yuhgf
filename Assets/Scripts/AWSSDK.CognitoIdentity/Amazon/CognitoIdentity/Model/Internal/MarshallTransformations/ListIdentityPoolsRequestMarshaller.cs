using System.Globalization;
using System.IO;
using System.Text;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class ListIdentityPoolsRequestMarshaller : IMarshaller<IRequest, ListIdentityPoolsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static ListIdentityPoolsRequestMarshaller _instance = new ListIdentityPoolsRequestMarshaller();

		public static ListIdentityPoolsRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListIdentityPoolsRequest)input);
		}

		public IRequest Marshall(ListIdentityPoolsRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.ListIdentityPools";
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
				if (publicRequest.IsSetMaxResults())
				{
					jsonMarshallerContext.Writer.WritePropertyName("MaxResults");
					jsonMarshallerContext.Writer.Write(publicRequest.MaxResults);
				}
				if (publicRequest.IsSetNextToken())
				{
					jsonMarshallerContext.Writer.WritePropertyName("NextToken");
					jsonMarshallerContext.Writer.Write(publicRequest.NextToken);
				}
				jsonWriter.WriteObjectEnd();
				string s = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(s);
				return request;
			}
		}

		internal static ListIdentityPoolsRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
