using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetLifecycleConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetLifecycleConfigurationResponseUnmarshaller _instance;

		public static GetLifecycleConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetLifecycleConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetLifecycleConfigurationResponse getLifecycleConfigurationResponse = new GetLifecycleConfigurationResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getLifecycleConfigurationResponse);
				}
			}
			return getLifecycleConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetLifecycleConfigurationResponse response)
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
					if (context.TestExpression("Rule", num))
					{
						response.Configuration.Rules.Add(RulesItemUnmarshaller.Instance.Unmarshall(context));
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
