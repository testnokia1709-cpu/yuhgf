using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectTaggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetObjectTaggingResponseUnmarshaller _instance;

		public static GetObjectTaggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetObjectTaggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetObjectTaggingResponse getObjectTaggingResponse = new GetObjectTaggingResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getObjectTaggingResponse);
				}
			}
			return getObjectTaggingResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetObjectTaggingResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 2;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Tag", num))
					{
						response.Tagging.Add(TagUnmarshaller.Instance.Unmarshall(context));
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
