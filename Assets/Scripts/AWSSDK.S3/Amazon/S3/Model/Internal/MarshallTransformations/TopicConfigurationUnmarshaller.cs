using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class TopicConfigurationUnmarshaller : IUnmarshaller<TopicConfiguration, XmlUnmarshallerContext>, IUnmarshaller<TopicConfiguration, JsonUnmarshallerContext>
	{
		private static TopicConfigurationUnmarshaller _instance;

		public static TopicConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TopicConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public TopicConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			TopicConfiguration topicConfiguration = new TopicConfiguration();
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
					if (context.TestExpression("Id", num))
					{
						topicConfiguration.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Event", num))
					{
						topicConfiguration.Events.Add(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("Topic", num))
					{
						topicConfiguration.Topic = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						topicConfiguration.Filter = FilterUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return topicConfiguration;
				}
			}
			return topicConfiguration;
		}

		public TopicConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
