namespace Parse
{
	public sealed class ParseRelation<T> : ParseRelationBase where T : ParseObject
	{
		public ParseQuery<T> Query
		{
			get
			{
				return GetQuery<T>();
			}
		}

		internal ParseRelation(ParseObject parent, string key)
			: base(parent, key)
		{
		}

		internal ParseRelation(ParseObject parent, string key, string targetClassName)
			: base(parent, key, targetClassName)
		{
		}

		public void Add(T obj)
		{
			Add((ParseObject)obj);
		}

		public void Remove(T obj)
		{
			Remove((ParseObject)obj);
		}
	}
}
