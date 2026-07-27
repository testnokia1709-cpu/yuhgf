using System;
using System.Collections.Generic;
using System.Linq;

namespace Parse.Internal
{
	internal class ParseIncrementOperation : IParseFieldOperation
	{
		private static readonly IDictionary<Tuple<Type, Type>, Func<object, object, object>> adders;

		private object amount;

		public object Amount
		{
			get
			{
				return amount;
			}
		}

		static ParseIncrementOperation()
		{
			adders = new Dictionary<Tuple<Type, Type>, Func<object, object, object>>
			{
				{
					new Tuple<Type, Type>(typeof(sbyte), typeof(sbyte)),
					(object left, object right) => (sbyte)left + (sbyte)right
				},
				{
					new Tuple<Type, Type>(typeof(sbyte), typeof(short)),
					(object left, object right) => (sbyte)left + (short)right
				},
				{
					new Tuple<Type, Type>(typeof(sbyte), typeof(int)),
					(object left, object right) => (sbyte)left + (int)right
				},
				{
					new Tuple<Type, Type>(typeof(sbyte), typeof(long)),
					(object left, object right) => (sbyte)left + (long)right
				},
				{
					new Tuple<Type, Type>(typeof(sbyte), typeof(float)),
					(object left, object right) => (float)(sbyte)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(sbyte), typeof(double)),
					(object left, object right) => (double)(sbyte)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(sbyte), typeof(decimal)),
					(object left, object right) => (decimal)(sbyte)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(byte)),
					(object left, object right) => (byte)left + (byte)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(short)),
					(object left, object right) => (byte)left + (short)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(ushort)),
					(object left, object right) => (byte)left + (ushort)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(int)),
					(object left, object right) => (byte)left + (int)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(uint)),
					(object left, object right) => (byte)left + (uint)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(long)),
					(object left, object right) => (byte)left + (long)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(ulong)),
					(object left, object right) => (byte)left + (ulong)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(float)),
					(object left, object right) => (float)(int)(byte)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(double)),
					(object left, object right) => (double)(int)(byte)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(byte), typeof(decimal)),
					(object left, object right) => (decimal)(byte)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(short), typeof(short)),
					(object left, object right) => (short)left + (short)right
				},
				{
					new Tuple<Type, Type>(typeof(short), typeof(int)),
					(object left, object right) => (short)left + (int)right
				},
				{
					new Tuple<Type, Type>(typeof(short), typeof(long)),
					(object left, object right) => (short)left + (long)right
				},
				{
					new Tuple<Type, Type>(typeof(short), typeof(float)),
					(object left, object right) => (float)(short)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(short), typeof(double)),
					(object left, object right) => (double)(short)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(short), typeof(decimal)),
					(object left, object right) => (decimal)(short)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(ushort), typeof(ushort)),
					(object left, object right) => (ushort)left + (ushort)right
				},
				{
					new Tuple<Type, Type>(typeof(ushort), typeof(int)),
					(object left, object right) => (ushort)left + (int)right
				},
				{
					new Tuple<Type, Type>(typeof(ushort), typeof(uint)),
					(object left, object right) => (ushort)left + (uint)right
				},
				{
					new Tuple<Type, Type>(typeof(ushort), typeof(long)),
					(object left, object right) => (ushort)left + (long)right
				},
				{
					new Tuple<Type, Type>(typeof(ushort), typeof(ulong)),
					(object left, object right) => (ushort)left + (ulong)right
				},
				{
					new Tuple<Type, Type>(typeof(ushort), typeof(float)),
					(object left, object right) => (float)(int)(ushort)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(ushort), typeof(double)),
					(object left, object right) => (double)(int)(ushort)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(ushort), typeof(decimal)),
					(object left, object right) => (decimal)(ushort)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(int), typeof(int)),
					(object left, object right) => (int)left + (int)right
				},
				{
					new Tuple<Type, Type>(typeof(int), typeof(long)),
					(object left, object right) => (int)left + (long)right
				},
				{
					new Tuple<Type, Type>(typeof(int), typeof(float)),
					(object left, object right) => (float)(int)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(int), typeof(double)),
					(object left, object right) => (double)(int)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(int), typeof(decimal)),
					(object left, object right) => (decimal)(int)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(uint), typeof(uint)),
					(object left, object right) => (uint)left + (uint)right
				},
				{
					new Tuple<Type, Type>(typeof(uint), typeof(long)),
					(object left, object right) => (uint)left + (long)right
				},
				{
					new Tuple<Type, Type>(typeof(uint), typeof(ulong)),
					(object left, object right) => (uint)left + (ulong)right
				},
				{
					new Tuple<Type, Type>(typeof(uint), typeof(float)),
					(object left, object right) => (float)(uint)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(uint), typeof(double)),
					(object left, object right) => (double)(uint)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(uint), typeof(decimal)),
					(object left, object right) => (decimal)(uint)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(long), typeof(long)),
					(object left, object right) => (long)left + (long)right
				},
				{
					new Tuple<Type, Type>(typeof(long), typeof(float)),
					(object left, object right) => (float)(long)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(long), typeof(double)),
					(object left, object right) => (double)(long)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(long), typeof(decimal)),
					(object left, object right) => (decimal)(long)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(char)),
					(object left, object right) => (char)left + (char)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(ushort)),
					(object left, object right) => (char)left + (ushort)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(int)),
					(object left, object right) => (char)left + (int)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(uint)),
					(object left, object right) => (char)left + (uint)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(long)),
					(object left, object right) => (char)left + (long)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(ulong)),
					(object left, object right) => (char)left + (ulong)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(float)),
					(object left, object right) => (float)(int)(char)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(double)),
					(object left, object right) => (double)(int)(char)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(char), typeof(decimal)),
					(object left, object right) => (decimal)(char)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(float), typeof(float)),
					(object left, object right) => (float)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(float), typeof(double)),
					(object left, object right) => (double)(float)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(ulong), typeof(ulong)),
					(object left, object right) => (ulong)left + (ulong)right
				},
				{
					new Tuple<Type, Type>(typeof(ulong), typeof(float)),
					(object left, object right) => (float)(ulong)left + (float)right
				},
				{
					new Tuple<Type, Type>(typeof(ulong), typeof(double)),
					(object left, object right) => (double)(ulong)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(ulong), typeof(decimal)),
					(object left, object right) => (decimal)(ulong)left + (decimal)right
				},
				{
					new Tuple<Type, Type>(typeof(double), typeof(double)),
					(object left, object right) => (double)left + (double)right
				},
				{
					new Tuple<Type, Type>(typeof(decimal), typeof(decimal)),
					(object left, object right) => (decimal)left + (decimal)right
				}
			};
			foreach (Tuple<Type, Type> item in adders.Keys.ToList())
			{
				if (!item.Item1.Equals(item.Item2))
				{
					Tuple<Type, Type> key = new Tuple<Type, Type>(item.Item2, item.Item1);
					Func<object, object, object> func = adders[item];
					adders[key] = (object left, object right) => func(right, left);
				}
			}
		}

		public ParseIncrementOperation(object amount)
		{
			this.amount = amount;
		}

		public object Encode()
		{
			return new Dictionary<string, object>
			{
				{ "__op", "Increment" },
				{ "amount", amount }
			};
		}

		private static object Add(object obj1, object obj2)
		{
			Func<object, object, object> value;
			if (adders.TryGetValue(new Tuple<Type, Type>(obj1.GetType(), obj2.GetType()), out value))
			{
				return value(obj1, obj2);
			}
			throw new InvalidCastException(string.Concat("Cannot add ", obj1.GetType(), " to ", obj2.GetType()));
		}

		public IParseFieldOperation MergeWithPrevious(IParseFieldOperation previous)
		{
			if (previous == null)
			{
				return this;
			}
			if (previous is ParseDeleteOperation)
			{
				return new ParseSetOperation(amount);
			}
			if (previous is ParseSetOperation)
			{
				object value = ((ParseSetOperation)previous).Value;
				if (value is string)
				{
					throw new InvalidOperationException("Cannot increment a non-number type.");
				}
				object obj = amount;
				return new ParseSetOperation(Add(value, obj));
			}
			if (previous is ParseIncrementOperation)
			{
				object obj2 = ((ParseIncrementOperation)previous).Amount;
				object obj3 = amount;
				return new ParseIncrementOperation(Add(obj2, obj3));
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, string key)
		{
			if (oldValue is string)
			{
				throw new InvalidOperationException("Cannot increment a non-number type.");
			}
			object obj = oldValue ?? ((object)0);
			object obj2 = amount;
			return Add(obj, obj2);
		}
	}
}
