using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketWebsiteResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketWebsiteResponseUnmarshaller _instance;

		public static GetBucketWebsiteResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketWebsiteResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketWebsiteResponse getBucketWebsiteResponse = new GetBucketWebsiteResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketWebsiteResponse);
				}
			}
			return getBucketWebsiteResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketWebsiteResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			response.WebsiteConfiguration = new WebsiteConfiguration();
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("RedirectAllRequestsTo", num))
					{
						response.WebsiteConfiguration.RedirectAllRequestsTo = RoutingRuleRedirectUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("IndexDocument/Suffix", num))
					{
						response.WebsiteConfiguration.IndexDocumentSuffix = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("ErrorDocument/Key", num))
					{
						response.WebsiteConfiguration.ErrorDocument = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("RoutingRule", num + 1))
					{
						response.WebsiteConfiguration.RoutingRules.Add(RoutingRuleUnmarshaller.Instance.Unmarshall(context));
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
