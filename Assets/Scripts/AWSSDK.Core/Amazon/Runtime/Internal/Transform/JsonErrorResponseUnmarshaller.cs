using System;

namespace Amazon.Runtime.Internal.Transform
{
	public class JsonErrorResponseUnmarshaller : IUnmarshaller<ErrorResponse, JsonUnmarshallerContext>
	{
		private static JsonErrorResponseUnmarshaller instance;

		public ErrorResponse Unmarshall(JsonUnmarshallerContext context)
		{
			if (context.Peek() == 60)
			{
				ErrorResponseUnmarshaller errorResponseUnmarshaller = new ErrorResponseUnmarshaller();
				XmlUnmarshallerContext context2 = new XmlUnmarshallerContext(context.Stream, false, null);
				return errorResponseUnmarshaller.Unmarshall(context2);
			}
			string text = null;
			string message = null;
			string text2 = null;
			while (context.Read())
			{
				if (context.TestExpression("__type"))
				{
					text = StringUnmarshaller.GetInstance().Unmarshall(context);
				}
				else if (context.TestExpression("message"))
				{
					message = StringUnmarshaller.GetInstance().Unmarshall(context);
				}
				else if (context.TestExpression("code"))
				{
					text2 = StringUnmarshaller.GetInstance().Unmarshall(context);
				}
			}
			if (string.IsNullOrEmpty(text) && context.ResponseData.IsHeaderPresent("x-amzn-ErrorType"))
			{
				string text3 = context.ResponseData.GetHeaderValue("x-amzn-ErrorType");
				if (!string.IsNullOrEmpty(text3))
				{
					int num = text3.IndexOf(":", StringComparison.Ordinal);
					if (num != -1)
					{
						text3 = text3.Substring(0, num);
					}
					text = text3;
				}
			}
			if (context.ResponseData.IsHeaderPresent("x-amzn-error-message"))
			{
				string headerValue = context.ResponseData.GetHeaderValue("x-amzn-error-message");
				if (!string.IsNullOrEmpty(headerValue))
				{
					message = headerValue;
				}
			}
			if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
			text = text.Substring(text.LastIndexOf("#", StringComparison.Ordinal) + 1);
			return new ErrorResponse
			{
				Code = text,
				Message = message,
				Type = ErrorType.Unknown
			};
		}

		public static JsonErrorResponseUnmarshaller GetInstance()
		{
			if (instance == null)
			{
				instance = new JsonErrorResponseUnmarshaller();
			}
			return instance;
		}
	}
}
