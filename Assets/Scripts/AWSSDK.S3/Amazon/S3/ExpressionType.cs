using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class ExpressionType : ConstantClass
	{
		public static readonly ExpressionType SQL = new ExpressionType("SQL");

		private ExpressionType(string value)
			: base(value)
		{
		}

		public static ExpressionType FindValue(string value)
		{
			return ConstantClass.FindValue<ExpressionType>(value);
		}

		public static implicit operator ExpressionType(string value)
		{
			return FindValue(value);
		}
	}
}
