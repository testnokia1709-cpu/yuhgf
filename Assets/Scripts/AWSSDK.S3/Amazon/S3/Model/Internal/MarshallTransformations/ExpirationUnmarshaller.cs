using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ExpirationUnmarshaller : IUnmarshaller<LifecycleRuleExpiration, XmlUnmarshallerContext>, IUnmarshaller<LifecycleRuleExpiration, JsonUnmarshallerContext>
	{
		private static ExpirationUnmarshaller _instance;

		public static ExpirationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ExpirationUnmarshaller();
				}
				return _instance;
			}
		}

		public LifecycleRuleExpiration Unmarshall(XmlUnmarshallerContext context)
		{
			LifecycleRuleExpiration lifecycleRuleExpiration = new LifecycleRuleExpiration();
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
					if (context.TestExpression("Date", num))
					{
						lifecycleRuleExpiration.Date = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Days", num))
					{
						lifecycleRuleExpiration.Days = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ExpiredObjectDeleteMarker", num))
					{
						lifecycleRuleExpiration.ExpiredObjectDeleteMarker = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return lifecycleRuleExpiration;
				}
			}
			return lifecycleRuleExpiration;
		}

		public LifecycleRuleExpiration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
