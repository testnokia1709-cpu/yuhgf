using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class SetIdentityPoolRolesRequestMarshaller : IMarshaller<IRequest, SetIdentityPoolRolesRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static SetIdentityPoolRolesRequestMarshaller _instance = new SetIdentityPoolRolesRequestMarshaller();

		public static SetIdentityPoolRolesRequestMarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((SetIdentityPoolRolesRequest)input);
		}

		public IRequest Marshall(SetIdentityPoolRolesRequest publicRequest)
		{
			IRequest request = new DefaultRequest(publicRequest, "Amazon.CognitoIdentity");
			string value = "AWSCognitoIdentityService.SetIdentityPoolRoles";
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
				if (publicRequest.IsSetRoleMappings())
				{
					jsonMarshallerContext.Writer.WritePropertyName("RoleMappings");
					jsonMarshallerContext.Writer.WriteObjectStart();
					foreach (KeyValuePair<string, RoleMapping> roleMapping in publicRequest.RoleMappings)
					{
						jsonMarshallerContext.Writer.WritePropertyName(roleMapping.Key);
						RoleMapping value2 = roleMapping.Value;
						jsonMarshallerContext.Writer.WriteObjectStart();
						RoleMappingMarshaller.Instance.Marshall(value2, jsonMarshallerContext);
						jsonMarshallerContext.Writer.WriteObjectEnd();
					}
					jsonMarshallerContext.Writer.WriteObjectEnd();
				}
				if (publicRequest.IsSetRoles())
				{
					jsonMarshallerContext.Writer.WritePropertyName("Roles");
					jsonMarshallerContext.Writer.WriteObjectStart();
					foreach (KeyValuePair<string, string> role in publicRequest.Roles)
					{
						jsonMarshallerContext.Writer.WritePropertyName(role.Key);
						string value3 = role.Value;
						jsonMarshallerContext.Writer.Write(value3);
					}
					jsonMarshallerContext.Writer.WriteObjectEnd();
				}
				jsonWriter.WriteObjectEnd();
				string s = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(s);
				return request;
			}
		}

		internal static SetIdentityPoolRolesRequestMarshaller GetInstance()
		{
			return _instance;
		}
	}
}
