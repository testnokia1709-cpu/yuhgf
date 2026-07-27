using System.Collections.Generic;

namespace Parse.Internal
{
	internal class ParseDeleteOperation : IParseFieldOperation
	{
		internal static readonly object DeleteToken = new object();

		private static ParseDeleteOperation _Instance = new ParseDeleteOperation();

		public static ParseDeleteOperation Instance
		{
			get
			{
				return _Instance;
			}
		}

		private ParseDeleteOperation()
		{
		}

		public object Encode()
		{
			return new Dictionary<string, object> { { "__op", "Delete" } };
		}

		public IParseFieldOperation MergeWithPrevious(IParseFieldOperation previous)
		{
			return this;
		}

		public object Apply(object oldValue, string key)
		{
			return DeleteToken;
		}
	}
}
