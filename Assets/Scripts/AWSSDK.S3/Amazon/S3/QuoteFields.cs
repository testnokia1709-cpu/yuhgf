using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class QuoteFields : ConstantClass
	{
		public static readonly QuoteFields Always = new QuoteFields("ALWAYS");

		public static readonly QuoteFields AsNeeded = new QuoteFields("ASNEEDED");

		private QuoteFields(string value)
			: base(value)
		{
		}

		public static QuoteFields FindValue(string value)
		{
			return ConstantClass.FindValue<QuoteFields>(value);
		}

		public static implicit operator QuoteFields(string value)
		{
			return FindValue(value);
		}
	}
}
