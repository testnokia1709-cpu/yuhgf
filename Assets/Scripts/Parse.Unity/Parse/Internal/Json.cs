using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Parse.Internal
{
	internal class Json
	{
		private class JsonStringParser
		{
			private int currentIndex;

			public string Input { get; private set; }

			public char[] InputAsArray { get; private set; }

			public int CurrentIndex
			{
				get
				{
					return currentIndex;
				}
			}

			public void Skip(int skip)
			{
				currentIndex += skip;
			}

			public JsonStringParser(string input)
			{
				Input = input;
				InputAsArray = input.ToCharArray();
			}

			internal bool ParseObject(out object output)
			{
				output = null;
				int currentIndex2 = CurrentIndex;
				if (!Accept(startObject))
				{
					return false;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				object output2;
				while (ParseMember(out output2))
				{
					Tuple<string, object> tuple = output2 as Tuple<string, object>;
					dictionary[tuple.Item1] = tuple.Item2;
					if (!Accept(valueSeparator))
					{
						break;
					}
				}
				if (!Accept(endObject))
				{
					return false;
				}
				output = dictionary;
				return true;
			}

			private bool ParseMember(out object output)
			{
				output = null;
				object output2;
				if (!ParseString(out output2))
				{
					return false;
				}
				if (!Accept(nameSeparator))
				{
					return false;
				}
				object output3;
				if (!ParseValue(out output3))
				{
					return false;
				}
				output = new Tuple<string, object>((string)output2, output3);
				return true;
			}

			internal bool ParseArray(out object output)
			{
				output = null;
				if (!Accept(startArray))
				{
					return false;
				}
				List<object> list = new List<object>();
				object output2;
				while (ParseValue(out output2))
				{
					list.Add(output2);
					if (!Accept(valueSeparator))
					{
						break;
					}
				}
				if (!Accept(endArray))
				{
					return false;
				}
				output = list;
				return true;
			}

			private bool ParseValue(out object output)
			{
				if (Accept(falseValue))
				{
					output = false;
					return true;
				}
				if (Accept(nullValue))
				{
					output = null;
					return true;
				}
				if (Accept(trueValue))
				{
					output = true;
					return true;
				}
				if (!ParseObject(out output) && !ParseArray(out output) && !ParseNumber(out output))
				{
					return ParseString(out output);
				}
				return true;
			}

			private bool ParseString(out object output)
			{
				output = null;
				Match match;
				if (!Accept(stringValue, out match))
				{
					return false;
				}
				int num = 0;
				Group obj = match.Groups["content"];
				StringBuilder stringBuilder = new StringBuilder(obj.Value);
				foreach (Capture capture in match.Groups["escape"].Captures)
				{
					int num2 = capture.Index - obj.Index - num;
					num += capture.Length - 1;
					stringBuilder.Remove(num2 + 1, capture.Length - 1);
					switch (capture.Value[1])
					{
					case '"':
						stringBuilder[num2] = '"';
						break;
					case '\\':
						stringBuilder[num2] = '\\';
						break;
					case '/':
						stringBuilder[num2] = '/';
						break;
					case 'b':
						stringBuilder[num2] = '\b';
						break;
					case 'f':
						stringBuilder[num2] = '\f';
						break;
					case 'n':
						stringBuilder[num2] = '\n';
						break;
					case 'r':
						stringBuilder[num2] = '\r';
						break;
					case 't':
						stringBuilder[num2] = '\t';
						break;
					case 'u':
						stringBuilder[num2] = (char)ushort.Parse(capture.Value.Substring(2), NumberStyles.AllowHexSpecifier);
						break;
					default:
						throw new ArgumentException("Unexpected escape character in string: " + capture.Value);
					}
				}
				output = stringBuilder.ToString();
				return true;
			}

			private bool ParseNumber(out object output)
			{
				output = null;
				Match match;
				if (!Accept(numberValue, out match))
				{
					return false;
				}
				if (match.Groups["frac"].Length > 0 || match.Groups["exp"].Length > 0)
				{
					output = double.Parse(match.Value, CultureInfo.InvariantCulture);
					return true;
				}
				output = long.Parse(match.Value, CultureInfo.InvariantCulture);
				return true;
			}

			private bool Accept(Regex matcher, out Match match)
			{
				match = matcher.Match(Input, CurrentIndex);
				if (match.Success)
				{
					Skip(match.Length);
				}
				return match.Success;
			}

			private bool Accept(char condition)
			{
				int num = 0;
				int num2 = InputAsArray.Length;
				int i;
				for (i = currentIndex; i < num2; i++)
				{
					char c;
					if ((c = InputAsArray[i]) != ' ' && c != '\r' && c != '\t' && c != '\n')
					{
						break;
					}
					num++;
				}
				bool flag = i < num2 && InputAsArray[i] == condition;
				if (flag)
				{
					num++;
					for (i++; i < num2; i++)
					{
						char c;
						if ((c = InputAsArray[i]) != ' ' && c != '\r' && c != '\t' && c != '\n')
						{
							break;
						}
						num++;
					}
					Skip(num);
				}
				return flag;
			}

			private bool Accept(char[] condition)
			{
				int num = 0;
				int num2 = InputAsArray.Length;
				int i;
				for (i = currentIndex; i < num2; i++)
				{
					char c;
					if ((c = InputAsArray[i]) != ' ' && c != '\r' && c != '\t' && c != '\n')
					{
						break;
					}
					num++;
				}
				bool flag = true;
				int num3 = 0;
				for (; i < num2; i++)
				{
					if (num3 >= condition.Length)
					{
						break;
					}
					if (InputAsArray[i] != condition[num3])
					{
						flag = false;
						break;
					}
					num3++;
				}
				bool num4 = i < num2 && flag;
				if (num4)
				{
					Skip(num + condition.Length);
				}
				return num4;
			}
		}

		private static readonly string startOfString = "\\G";

		private static readonly char startObject = '{';

		private static readonly char endObject = '}';

		private static readonly char startArray = '[';

		private static readonly char endArray = ']';

		private static readonly char valueSeparator = ',';

		private static readonly char nameSeparator = ':';

		private static readonly char[] falseValue = "false".ToCharArray();

		private static readonly char[] trueValue = "true".ToCharArray();

		private static readonly char[] nullValue = "null".ToCharArray();

		private static readonly Regex numberValue = new Regex(startOfString + "-?(?:0|[1-9]\\d*)(?<frac>\\.\\d+)?(?<exp>(?:e|E)(?:-|\\+)?\\d+)?");

		private static readonly Regex stringValue = new Regex(startOfString + "\"(?<content>(?:[^\\\\\"]|(?<escape>\\\\(?:[\\\\\"/bfnrt]|u[0-9a-fA-F]{4})))*)\"", RegexOptions.Multiline);

		private static readonly Regex escapePattern = new Regex("\\\\|\"|[\0-\u001f]");

		public static object Parse(string input)
		{
			input = input.Trim();
			JsonStringParser jsonStringParser = new JsonStringParser(input);
			object output;
			if ((jsonStringParser.ParseObject(out output) || jsonStringParser.ParseArray(out output)) && jsonStringParser.CurrentIndex == input.Length)
			{
				return output;
			}
			throw new ArgumentException("Input JSON was invalid.");
		}

		public static string Encode(IDictionary<string, object> dict)
		{
			if (dict == null)
			{
				throw new ArgumentNullException();
			}
			if (dict.Count == 0)
			{
				return "{}";
			}
			StringBuilder stringBuilder = new StringBuilder("{");
			foreach (KeyValuePair<string, object> item in dict)
			{
				stringBuilder.Append(Encode(item.Key));
				stringBuilder.Append(":");
				stringBuilder.Append(Encode(item.Value));
				stringBuilder.Append(",");
			}
			stringBuilder[stringBuilder.Length - 1] = '}';
			return stringBuilder.ToString();
		}

		public static string Encode(IList<object> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException();
			}
			if (list.Count == 0)
			{
				return "[]";
			}
			StringBuilder stringBuilder = new StringBuilder("[");
			foreach (object item in list)
			{
				stringBuilder.Append(Encode(item));
				stringBuilder.Append(",");
			}
			stringBuilder[stringBuilder.Length - 1] = ']';
			return stringBuilder.ToString();
		}

		public static string Encode(object obj)
		{
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary != null)
			{
				return Encode(dictionary);
			}
			IList<object> list = obj as IList<object>;
			if (list != null)
			{
				return Encode(list);
			}
			string text = obj as string;
			if (text != null)
			{
				text = escapePattern.Replace(text, delegate(Match m)
				{
					switch (m.Value[0])
					{
					case '\\':
						return "\\\\";
					case '"':
						return "\\\"";
					case '\b':
						return "\\b";
					case '\f':
						return "\\f";
					case '\n':
						return "\\n";
					case '\r':
						return "\\r";
					case '\t':
						return "\\t";
					default:
						return "\\u" + ((ushort)m.Value[0]).ToString("x4");
					}
				});
				return "\"" + text + "\"";
			}
			if (obj == null)
			{
				return "null";
			}
			if (obj is bool)
			{
				if ((bool)obj)
				{
					return "true";
				}
				return "false";
			}
			if (!ReflectionHelpers.IsPrimitive(obj.GetType()))
			{
				throw new ArgumentException("Unable to encode objects of type " + obj.GetType());
			}
			return Convert.ToString(obj, CultureInfo.InvariantCulture);
		}
	}
}
