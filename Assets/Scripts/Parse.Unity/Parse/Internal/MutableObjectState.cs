using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Parse.Internal
{
	internal class MutableObjectState : IObjectState, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		private IDictionary<string, object> serverData = new Dictionary<string, object>();

		public bool IsNew { get; internal set; }

		public string ClassName { get; internal set; }

		public string ObjectId { get; internal set; }

		public DateTime? UpdatedAt { get; internal set; }

		public DateTime? CreatedAt { get; internal set; }

		internal IDictionary<string, object> ServerData
		{
			get
			{
				return serverData;
			}
			set
			{
				serverData = value;
			}
		}

		public object this[string key]
		{
			get
			{
				return ServerData[key];
			}
		}

		public bool ContainsKey(string key)
		{
			return ServerData.ContainsKey(key);
		}

		public void Apply(IDictionary<string, IParseFieldOperation> operationSet)
		{
			foreach (KeyValuePair<string, IParseFieldOperation> item in operationSet)
			{
				object value;
				ServerData.TryGetValue(item.Key, out value);
				object obj = item.Value.Apply(value, item.Key);
				if (obj != ParseDeleteOperation.DeleteToken)
				{
					ServerData[item.Key] = obj;
				}
				else
				{
					ServerData.Remove(item.Key);
				}
			}
		}

		public void Apply(IObjectState other)
		{
			IsNew = other.IsNew;
			if (other.ObjectId != null)
			{
				ObjectId = other.ObjectId;
			}
			if (other.UpdatedAt.HasValue)
			{
				UpdatedAt = other.UpdatedAt;
			}
			if (other.CreatedAt.HasValue)
			{
				CreatedAt = other.CreatedAt;
			}
			foreach (KeyValuePair<string, object> item in other)
			{
				ServerData[item.Key] = item.Value;
			}
		}

		public IObjectState MutatedClone(Action<MutableObjectState> func)
		{
			MutableObjectState mutableObjectState = MutableClone();
			func(mutableObjectState);
			return mutableObjectState;
		}

		protected virtual MutableObjectState MutableClone()
		{
			return new MutableObjectState
			{
				IsNew = IsNew,
				ClassName = ClassName,
				ObjectId = ObjectId,
				CreatedAt = CreatedAt,
				UpdatedAt = UpdatedAt,
				ServerData = this.ToDictionary((KeyValuePair<string, object> t) => t.Key, (KeyValuePair<string, object> t) => t.Value)
			};
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return ServerData.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
		}
	}
}
