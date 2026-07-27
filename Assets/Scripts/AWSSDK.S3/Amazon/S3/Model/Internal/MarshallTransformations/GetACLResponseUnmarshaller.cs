using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetACLResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetACLResponseUnmarshaller _instance;

		public static GetACLResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetACLResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetACLResponse getACLResponse = new GetACLResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getACLResponse);
				}
			}
			return getACLResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetACLResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Owner", num))
					{
						if (response.AccessControlList == null)
						{
							response.AccessControlList = new S3AccessControlList();
						}
						response.AccessControlList.Owner = OwnerUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Grant", num + 1))
					{
						if (response.AccessControlList == null)
						{
							response.AccessControlList = new S3AccessControlList();
						}
						response.AccessControlList.Grants.Add(GrantUnmarshaller.Instance.Unmarshall(context));
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					break;
				}
			}
		}
	}
}
