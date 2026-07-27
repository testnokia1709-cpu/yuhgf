using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetCORSConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetCORSConfigurationResponseUnmarshaller _instance;

		public static GetCORSConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetCORSConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetCORSConfigurationResponse getCORSConfigurationResponse = new GetCORSConfigurationResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getCORSConfigurationResponse);
				}
			}
			return getCORSConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetCORSConfigurationResponse response)
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
					if (context.TestExpression("CORSRule", num))
					{
						if (response.Configuration == null)
						{
							response.Configuration = new CORSConfiguration();
						}
						response.Configuration.Rules.Add(CORSRuleUnmarshaller.Instance.Unmarshall(context));
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
