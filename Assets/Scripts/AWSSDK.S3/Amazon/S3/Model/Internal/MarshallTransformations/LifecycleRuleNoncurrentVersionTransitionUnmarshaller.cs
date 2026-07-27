using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class LifecycleRuleNoncurrentVersionTransitionUnmarshaller : IUnmarshaller<LifecycleRuleNoncurrentVersionTransition, XmlUnmarshallerContext>, IUnmarshaller<LifecycleRuleNoncurrentVersionTransition, JsonUnmarshallerContext>
	{
		private static LifecycleRuleNoncurrentVersionTransitionUnmarshaller _instance;

		public static LifecycleRuleNoncurrentVersionTransitionUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LifecycleRuleNoncurrentVersionTransitionUnmarshaller();
				}
				return _instance;
			}
		}

		public LifecycleRuleNoncurrentVersionTransition Unmarshall(XmlUnmarshallerContext context)
		{
			LifecycleRuleNoncurrentVersionTransition lifecycleRuleNoncurrentVersionTransition = new LifecycleRuleNoncurrentVersionTransition();
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
					if (context.TestExpression("NoncurrentDays", num))
					{
						lifecycleRuleNoncurrentVersionTransition.NoncurrentDays = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						lifecycleRuleNoncurrentVersionTransition.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return lifecycleRuleNoncurrentVersionTransition;
				}
			}
			return lifecycleRuleNoncurrentVersionTransition;
		}

		public LifecycleRuleNoncurrentVersionTransition Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
