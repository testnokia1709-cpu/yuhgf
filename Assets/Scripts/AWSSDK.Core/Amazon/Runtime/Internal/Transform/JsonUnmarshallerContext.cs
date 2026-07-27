using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Amazon.Runtime.Internal.Util;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime.Internal.Transform
{
	public class JsonUnmarshallerContext : UnmarshallerContext
	{
		private class JsonPathStack
		{
			private Stack<string> stack = new Stack<string>();

			private int currentDepth;

			private StringBuilder stackStringBuilder = new StringBuilder(128);

			private string stackString;

			public int CurrentDepth
			{
				get
				{
					return currentDepth;
				}
			}

			public string CurrentPath
			{
				get
				{
					if (stackString == null)
					{
						stackString = stackStringBuilder.ToString();
					}
					return stackString;
				}
			}

			public int Count
			{
				get
				{
					return stack.Count;
				}
			}

			public void Push(string value)
			{
				if (value == "/")
				{
					currentDepth++;
				}
				stackStringBuilder.Append(value);
				stackString = null;
				stack.Push(value);
			}

			public string Pop()
			{
				string text = stack.Pop();
				if (text == "/")
				{
					currentDepth--;
				}
				stackStringBuilder.Remove(stackStringBuilder.Length - text.Length, text.Length);
				stackString = null;
				return text;
			}

			public string Peek()
			{
				return stack.Peek();
			}
		}

		private const string DELIMITER = "/";

		private StreamReader streamReader;

		private JsonReader jsonReader;

		private JsonPathStack stack = new JsonPathStack();

		private string currentField;

		private JsonToken? currentToken;

		private bool disposed;

		private bool wasPeeked;

		public override bool IsStartOfDocument
		{
			get
			{
				if (CurrentTokenType == JsonToken.None)
				{
					return !streamReader.EndOfStream;
				}
				return false;
			}
		}

		public override bool IsEndElement
		{
			get
			{
				return CurrentTokenType == JsonToken.ObjectEnd;
			}
		}

		public override bool IsStartElement
		{
			get
			{
				return CurrentTokenType == JsonToken.ObjectStart;
			}
		}

		public override int CurrentDepth
		{
			get
			{
				return stack.CurrentDepth;
			}
		}

		public override string CurrentPath
		{
			get
			{
				return stack.CurrentPath;
			}
		}

		public JsonToken CurrentTokenType
		{
			get
			{
				return currentToken.Value;
			}
		}

		public Stream Stream
		{
			get
			{
				return streamReader.BaseStream;
			}
		}

		public JsonUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData responseData)
		{
			if (maintainResponseBody)
			{
				base.WrappingStream = new CachingWrapperStream(responseStream, AWSConfigs.LoggingConfig.LogResponsesSizeLimit);
				responseStream = base.WrappingStream;
			}
			base.WebResponseData = responseData;
			base.MaintainResponseBody = maintainResponseBody;
			long result;
			if (responseData != null && long.TryParse(responseData.GetHeaderValue("Content-Length"), out result) && responseData.ContentLength.Equals(result) && string.IsNullOrEmpty(responseData.GetHeaderValue("Content-Encoding")))
			{
				SetupCRCStream(responseData, responseStream, result);
			}
			if (base.CrcStream != null)
			{
				streamReader = new StreamReader(base.CrcStream);
			}
			else
			{
				streamReader = new StreamReader(responseStream);
			}
			jsonReader = new JsonReader(streamReader);
		}

		public override bool Read()
		{
			if (wasPeeked)
			{
				wasPeeked = false;
				return !currentToken.HasValue;
			}
			bool num = jsonReader.Read();
			if (num)
			{
				currentToken = jsonReader.Token;
				UpdateContext();
			}
			else
			{
				currentToken = null;
			}
			wasPeeked = false;
			return num;
		}

		public bool Peek(JsonToken token)
		{
			if (wasPeeked)
			{
				if (currentToken.HasValue)
				{
					return currentToken == token;
				}
				return false;
			}
			if (Read())
			{
				wasPeeked = true;
				return currentToken == token;
			}
			return false;
		}

		public override string ReadText()
		{
			object value = jsonReader.Value;
			switch (currentToken)
			{
			case JsonToken.Null:
				return null;
			case JsonToken.PropertyName:
			case JsonToken.String:
				return value as string;
			case JsonToken.Int:
			case JsonToken.UInt:
			case JsonToken.Long:
			case JsonToken.ULong:
			case JsonToken.Boolean:
			{
				IFormattable formattable2 = value as IFormattable;
				if (formattable2 != null)
				{
					return formattable2.ToString(null, CultureInfo.InvariantCulture);
				}
				return value.ToString();
			}
			case JsonToken.Double:
			{
				IFormattable formattable = value as IFormattable;
				if (formattable != null)
				{
					return formattable.ToString("R", CultureInfo.InvariantCulture);
				}
				return value.ToString();
			}
			default:
				throw new AmazonClientException("We expected a VALUE token but got: " + currentToken);
			}
		}

		public int Peek()
		{
			while (char.IsWhiteSpace((char)StreamPeek()))
			{
				streamReader.Read();
			}
			return StreamPeek();
		}

		private int StreamPeek()
		{
			int num = streamReader.Peek();
			if (num == -1)
			{
				streamReader.DiscardBufferedData();
				num = streamReader.Peek();
			}
			return num;
		}

		private void UpdateContext()
		{
			if (!currentToken.HasValue)
			{
				return;
			}
			if (currentToken.Value == JsonToken.ObjectStart || currentToken.Value == JsonToken.ArrayStart)
			{
				stack.Push("/");
			}
			else if (currentToken.Value == JsonToken.ObjectEnd || currentToken.Value == JsonToken.ArrayEnd)
			{
				if ((object)stack.Peek() == "/")
				{
					stack.Pop();
					if (stack.Count > 0 && (object)stack.Peek() != "/")
					{
						stack.Pop();
					}
				}
				currentField = null;
			}
			else if (currentToken.Value == JsonToken.PropertyName)
			{
				string text = ReadText();
				currentField = text;
				stack.Push(currentField);
			}
			else if (currentToken.Value != JsonToken.None && !stack.CurrentPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				stack.Pop();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing && streamReader != null)
				{
					streamReader.Dispose();
					streamReader = null;
				}
				disposed = true;
			}
			base.Dispose(disposing);
		}
	}
}
