using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CloudOnce.Internal
{
	public class JSONObject
	{
		public delegate void AddJsonConents(JSONObject self);

		public enum Type
		{
			Null = 0,
			String = 1,
			Number = 2,
			Object = 3,
			Array = 4,
			Bool = 5,
			Baked = 6
		}

		private const int c_maxDepth = 100;

		private const string c_infinity = "\"INFINITY\"";

		private const string c_negInfinity = "\"NEGINFINITY\"";

		private const string c_nan = "\"NaN\"";

		private static readonly char[] s_whitespace = new char[4] { ' ', '\r', '\n', '\t' };

		public List<JSONObject> List { get; private set; }

		public bool IsContainer
		{
			get
			{
				return ObjectType == Type.Array || ObjectType == Type.Object;
			}
		}

		public Type ObjectType { get; set; }

		public List<string> Keys { get; private set; }

		public string String { get; private set; }

		public float F { get; private set; }

		public bool B { get; private set; }

		public JSONObject this[int index]
		{
			get
			{
				return (List.Count <= index) ? null : List[index];
			}
			set
			{
				if (List.Count > index)
				{
					List[index] = value;
				}
			}
		}

		public JSONObject this[string index]
		{
			get
			{
				return GetField(index);
			}
			set
			{
				SetField(index, value);
			}
		}

		public JSONObject(Type t)
		{
			ObjectType = t;
			switch (t)
			{
			case Type.Array:
				List = new List<JSONObject>();
				break;
			case Type.Object:
				List = new List<JSONObject>();
				Keys = new List<string>();
				break;
			}
		}

		public JSONObject(bool b)
		{
			ObjectType = Type.Bool;
			B = b;
		}

		public JSONObject(float f)
		{
			ObjectType = Type.Number;
			F = f;
		}

		public JSONObject(Dictionary<string, string> dic)
		{
			ObjectType = Type.Object;
			Keys = new List<string>();
			List = new List<JSONObject>();
			foreach (KeyValuePair<string, string> item in dic)
			{
				Keys.Add(item.Key);
				List.Add(CreateStringObject(item.Value));
			}
		}

		public JSONObject(Dictionary<string, JSONObject> dic)
		{
			ObjectType = Type.Object;
			Keys = new List<string>();
			List = new List<JSONObject>();
			foreach (KeyValuePair<string, JSONObject> item in dic)
			{
				Keys.Add(item.Key);
				List.Add(item.Value);
			}
		}

		public JSONObject(AddJsonConents content)
		{
			content(this);
		}

		public JSONObject(IEnumerable<JSONObject> objs)
		{
			ObjectType = Type.Array;
			List = new List<JSONObject>(objs);
		}

		public JSONObject()
		{
		}

		public JSONObject(string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{
			Parse(str, maxDepth, storeExcessLevels, strict);
		}

		public static implicit operator bool(JSONObject o)
		{
			return o != null;
		}

		public static JSONObject StringObject(string val)
		{
			return CreateStringObject(val);
		}

		public static JSONObject Create()
		{
			return new JSONObject();
		}

		public static JSONObject Create(Type t)
		{
			JSONObject jSONObject = Create();
			jSONObject.ObjectType = t;
			switch (t)
			{
			case Type.Array:
				jSONObject.List = new List<JSONObject>();
				break;
			case Type.Object:
				jSONObject.List = new List<JSONObject>();
				jSONObject.Keys = new List<string>();
				break;
			}
			return jSONObject;
		}

		public static JSONObject Create(bool val)
		{
			JSONObject jSONObject = Create();
			jSONObject.ObjectType = Type.Bool;
			jSONObject.B = val;
			return jSONObject;
		}

		public static JSONObject Create(float val)
		{
			JSONObject jSONObject = Create();
			jSONObject.ObjectType = Type.Number;
			jSONObject.F = val;
			return jSONObject;
		}

		public static JSONObject Create(int val)
		{
			JSONObject jSONObject = Create();
			jSONObject.ObjectType = Type.Number;
			jSONObject.F = val;
			return jSONObject;
		}

		public static JSONObject CreateStringObject(string val)
		{
			JSONObject jSONObject = Create();
			jSONObject.ObjectType = Type.String;
			jSONObject.String = val;
			return jSONObject;
		}

		public static JSONObject CreateBakedObject(string val)
		{
			JSONObject jSONObject = Create();
			jSONObject.ObjectType = Type.Baked;
			jSONObject.String = val;
			return jSONObject;
		}

		public static JSONObject Create(string val, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{
			JSONObject jSONObject = Create();
			jSONObject.Parse(val, maxDepth, storeExcessLevels, strict);
			return jSONObject;
		}

		public static JSONObject Create(AddJsonConents content)
		{
			JSONObject jSONObject = Create();
			content(jSONObject);
			return jSONObject;
		}

		public static JSONObject Create(Dictionary<string, string> dic)
		{
			JSONObject jSONObject = Create();
			jSONObject.ObjectType = Type.Object;
			jSONObject.Keys = new List<string>();
			jSONObject.List = new List<JSONObject>();
			foreach (KeyValuePair<string, string> item in dic)
			{
				jSONObject.Keys.Add(item.Key);
				jSONObject.List.Add(CreateStringObject(item.Value));
			}
			return jSONObject;
		}

		public static JSONObject Create(Dictionary<string, float> dic)
		{
			JSONObject jSONObject = Create();
			jSONObject.ObjectType = Type.Object;
			jSONObject.Keys = new List<string>();
			jSONObject.List = new List<JSONObject>();
			foreach (KeyValuePair<string, float> item in dic)
			{
				jSONObject.Keys.Add(item.Key);
				jSONObject.List.Add(new JSONObject(item.Value));
			}
			return jSONObject;
		}

		public void Absorb(JSONObject obj)
		{
			List.AddRange(obj.List);
			Keys.AddRange(obj.Keys);
			String = obj.String;
			F = obj.F;
			B = obj.B;
			ObjectType = obj.ObjectType;
		}

		public void Add(JSONObject obj)
		{
			if (!obj)
			{
				return;
			}
			if (ObjectType != Type.Array)
			{
				ObjectType = Type.Array;
				if (List == null)
				{
					List = new List<JSONObject>();
				}
			}
			List.Add(obj);
		}

		public void AddField(string name, bool val)
		{
			AddField(name, Create(val));
		}

		public void AddField(string name, float val)
		{
			AddField(name, Create(val));
		}

		public void AddField(string name, string val)
		{
			AddField(name, CreateStringObject(val));
		}

		public void AddField(string name, JSONObject obj)
		{
			if (!obj)
			{
				return;
			}
			if (ObjectType != Type.Object)
			{
				if (Keys == null)
				{
					Keys = new List<string>();
				}
				if (ObjectType == Type.Array)
				{
					for (int i = 0; i < List.Count; i++)
					{
						Keys.Add(i + string.Empty);
					}
				}
				else if (List == null)
				{
					List = new List<JSONObject>();
				}
				ObjectType = Type.Object;
			}
			Keys.Add(name);
			List.Add(obj);
		}

		public void RemoveField(string name)
		{
			if (Keys.IndexOf(name) > -1)
			{
				List.RemoveAt(Keys.IndexOf(name));
				Keys.Remove(name);
			}
		}

		public bool HasFields(params string[] names)
		{
			foreach (string item in names)
			{
				if (!Keys.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			return Print();
		}

		public string ToString(bool pretty)
		{
			return Print(pretty);
		}

		public Dictionary<string, string> ToDictionary()
		{
			if (ObjectType == Type.Object)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				for (int i = 0; i < List.Count; i++)
				{
					JSONObject jSONObject = List[i];
					switch (jSONObject.ObjectType)
					{
					case Type.String:
						dictionary.Add(Keys[i], jSONObject.String);
						break;
					case Type.Number:
						dictionary.Add(Keys[i], jSONObject.F.ToString(CultureInfo.InvariantCulture));
						break;
					case Type.Bool:
						dictionary.Add(Keys[i], jSONObject.B + string.Empty);
						break;
					default:
						Debug.LogWarning("Omitting object: " + Keys[i] + " in dictionary conversion");
						break;
					}
				}
				return dictionary;
			}
			return null;
		}

		private void Parse(string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{
			if (!string.IsNullOrEmpty(str))
			{
				str = str.Trim(s_whitespace);
				if (strict && str[0] != '[' && str[0] != '{')
				{
					ObjectType = Type.Null;
				}
				else if (str.Length > 0)
				{
					if (string.Compare(str, "true", StringComparison.OrdinalIgnoreCase) == 0)
					{
						ObjectType = Type.Bool;
						B = true;
						return;
					}
					if (string.Compare(str, "false", StringComparison.OrdinalIgnoreCase) == 0)
					{
						ObjectType = Type.Bool;
						B = false;
						return;
					}
					if (string.Compare(str, "null", StringComparison.OrdinalIgnoreCase) == 0)
					{
						ObjectType = Type.Null;
						return;
					}
					switch (str)
					{
					case "\"INFINITY\"":
						ObjectType = Type.Number;
						F = float.PositiveInfinity;
						return;
					case "\"NEGINFINITY\"":
						ObjectType = Type.Number;
						F = float.NegativeInfinity;
						return;
					case "\"NaN\"":
						ObjectType = Type.Number;
						F = float.NaN;
						return;
					}
					if (str[0] == '"')
					{
						ObjectType = Type.String;
						String = str.Substring(1, str.Length - 2);
						return;
					}
					int num = 1;
					int num2 = 0;
					switch (str[num2])
					{
					case '{':
						ObjectType = Type.Object;
						Keys = new List<string>();
						List = new List<JSONObject>();
						break;
					case '[':
						ObjectType = Type.Array;
						List = new List<JSONObject>();
						break;
					default:
						try
						{
							F = Convert.ToSingle(str, CultureInfo.InvariantCulture);
							ObjectType = Type.Number;
							return;
						}
						catch (FormatException)
						{
							ObjectType = Type.Null;
							return;
						}
					}
					string item = string.Empty;
					bool flag = false;
					bool flag2 = false;
					int num3 = 0;
					while (++num2 < str.Length)
					{
						if (Array.IndexOf(s_whitespace, str[num2]) > -1)
						{
							continue;
						}
						if (str[num2] == '\\')
						{
							num2 += 2;
						}
						if (str[num2] == '"')
						{
							if (flag)
							{
								if (!flag2 && num3 == 0 && ObjectType == Type.Object)
								{
									item = str.Substring(num + 1, num2 - num - 1);
								}
								flag = false;
							}
							else
							{
								if (num3 == 0 && ObjectType == Type.Object)
								{
									num = num2;
								}
								flag = true;
							}
						}
						if (flag)
						{
							continue;
						}
						if (ObjectType == Type.Object && num3 == 0 && str[num2] == ':')
						{
							num = num2 + 1;
							flag2 = true;
						}
						switch (str[num2])
						{
						case '[':
						case '{':
							num3++;
							break;
						case ']':
						case '}':
							num3--;
							break;
						}
						if ((str[num2] != ',' || num3 != 0) && num3 >= 0)
						{
							continue;
						}
						flag2 = false;
						string text = str.Substring(num, num2 - num).Trim(s_whitespace);
						if (text.Length > 0)
						{
							if (ObjectType == Type.Object)
							{
								Keys.Add(item);
							}
							if (maxDepth != -1)
							{
								List.Add(Create(text, (maxDepth >= -1) ? (maxDepth - 1) : (-2)));
							}
							else if (storeExcessLevels)
							{
								List.Add(CreateBakedObject(text));
							}
						}
						num = num2 + 1;
					}
				}
				else
				{
					ObjectType = Type.Null;
				}
			}
			else
			{
				ObjectType = Type.Null;
			}
		}

		private void SetField(string name, JSONObject obj)
		{
			if (HasField(name))
			{
				List.Remove(this[name]);
				Keys.Remove(name);
			}
			AddField(name, obj);
		}

		private JSONObject GetField(string name)
		{
			if (ObjectType == Type.Object)
			{
				for (int i = 0; i < Keys.Count; i++)
				{
					if (Keys[i] == name)
					{
						return List[i];
					}
				}
			}
			return null;
		}

		private bool HasField(string name)
		{
			return ObjectType == Type.Object && Keys.Any((string t) => t == name);
		}

		private string Print(bool pretty = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Stringify(0, stringBuilder, pretty);
			return stringBuilder.ToString();
		}

		private void Stringify(int depth, StringBuilder builder, bool pretty = false)
		{
			if (depth++ > 100)
			{
				return;
			}
			switch (ObjectType)
			{
			case Type.Baked:
				builder.Append(String);
				break;
			case Type.String:
				builder.AppendFormat("\"{0}\"", String);
				break;
			case Type.Number:
				if (float.IsInfinity(F))
				{
					builder.Append("\"INFINITY\"");
				}
				else if (float.IsNegativeInfinity(F))
				{
					builder.Append("\"NEGINFINITY\"");
				}
				else if (float.IsNaN(F))
				{
					builder.Append("\"NaN\"");
				}
				else
				{
					builder.Append(F.ToString(CultureInfo.InvariantCulture));
				}
				break;
			case Type.Object:
				builder.Append("{");
				if (List.Count > 0)
				{
					if (pretty)
					{
						builder.Append("\n");
					}
					for (int k = 0; k < List.Count; k++)
					{
						string arg = Keys[k];
						JSONObject jSONObject = List[k];
						if (!jSONObject)
						{
							continue;
						}
						if (pretty)
						{
							for (int l = 0; l < depth; l++)
							{
								builder.Append("\t");
							}
						}
						builder.AppendFormat("\"{0}\":", arg);
						jSONObject.Stringify(depth, builder, pretty);
						builder.Append(",");
						if (pretty)
						{
							builder.Append("\n");
						}
					}
					if (pretty)
					{
						builder.Length -= 2;
					}
					else
					{
						builder.Length--;
					}
				}
				if (pretty && List.Count > 0)
				{
					builder.Append("\n");
					for (int m = 0; m < depth - 1; m++)
					{
						builder.Append("\t");
					}
				}
				builder.Append("}");
				break;
			case Type.Array:
				builder.Append("[");
				if (List.Count > 0)
				{
					if (pretty)
					{
						builder.Append("\n");
					}
					foreach (JSONObject item in List)
					{
						if (!item)
						{
							continue;
						}
						if (pretty)
						{
							for (int i = 0; i < depth; i++)
							{
								builder.Append("\t");
							}
						}
						item.Stringify(depth, builder, pretty);
						builder.Append(",");
						if (pretty)
						{
							builder.Append("\n");
						}
					}
					if (pretty)
					{
						builder.Length -= 2;
					}
					else
					{
						builder.Length--;
					}
				}
				if (pretty && List.Count > 0)
				{
					builder.Append("\n");
					for (int j = 0; j < depth - 1; j++)
					{
						builder.Append("\t");
					}
				}
				builder.Append("]");
				break;
			case Type.Bool:
				builder.Append((!B) ? "false" : "true");
				break;
			case Type.Null:
				builder.Append("null");
				break;
			}
		}
	}
}
