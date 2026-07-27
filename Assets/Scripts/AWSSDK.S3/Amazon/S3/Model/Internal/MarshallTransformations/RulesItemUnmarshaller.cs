using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class RulesItemUnmarshaller : IUnmarshaller<LifecycleRule, XmlUnmarshallerContext>, IUnmarshaller<LifecycleRule, JsonUnmarshallerContext>
	{
		private static RulesItemUnmarshaller _instance;

		public static RulesItemUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RulesItemUnmarshaller();
				}
				return _instance;
			}
		}

		public LifecycleRule Unmarshall(XmlUnmarshallerContext context)
		{
			LifecycleRule lifecycleRule = new LifecycleRule();
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
					if (context.TestExpression("Expiration", num))
					{
						lifecycleRule.Expiration = ExpirationUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("ID", num))
					{
						lifecycleRule.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Prefix", num))
					{
						lifecycleRule.Prefix = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						List<LifecycleFilterPredicate> list = LifecycleFilterPredicateListUnmarshaller.Instance.Unmarshall(context);
						if (list.Count == 1)
						{
							lifecycleRule.Filter = new LifecycleFilter
							{
								LifecycleFilterPredicate = list[0]
							};
						}
						else if (list.Count > 1)
						{
							string headerValue = context.ResponseData.GetHeaderValue("x-amzn-RequestId");
							string message = "The Filter element must contain at most one 'Prefix', 'Tag', or 'And' predicate.";
							throw new AmazonUnmarshallingException(headerValue, context.CurrentPath, context.ResponseBody, message, null);
						}
					}
					else if (context.TestExpression("Status", num))
					{
						lifecycleRule.Status = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Transition", num))
					{
						lifecycleRule.Transitions.Add(TransitionUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("NoncurrentVersionTransition", num))
					{
						lifecycleRule.NoncurrentVersionTransitions.Add(LifecycleRuleNoncurrentVersionTransitionUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("NoncurrentVersionExpiration", num))
					{
						lifecycleRule.NoncurrentVersionExpiration = LifecycleRuleNoncurrentVersionExpirationUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("AbortIncompleteMultipartUpload", num))
					{
						lifecycleRule.AbortIncompleteMultipartUpload = AbortIncompleteMultipartUploadUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return lifecycleRule;
				}
			}
			return lifecycleRule;
		}

		public LifecycleRule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
