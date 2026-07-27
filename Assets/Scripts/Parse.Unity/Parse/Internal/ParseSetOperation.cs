namespace Parse.Internal
{
	internal class ParseSetOperation : IParseFieldOperation
	{
		public object Value { get; private set; }

		public ParseSetOperation(object value)
		{
			Value = value;
		}

		public object Encode()
		{
			return PointerOrLocalIdEncoder.Instance.Encode(Value);
		}

		public IParseFieldOperation MergeWithPrevious(IParseFieldOperation previous)
		{
			return this;
		}

		public object Apply(object oldValue, string key)
		{
			return Value;
		}
	}
}
