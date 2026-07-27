using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class RestoreRequestType : ConstantClass
	{
		public static readonly RestoreRequestType SELECT = new RestoreRequestType("SELECT");

		private RestoreRequestType(string value)
			: base(value)
		{
		}

		public static RestoreRequestType FindValue(string value)
		{
			return ConstantClass.FindValue<RestoreRequestType>(value);
		}

		public static implicit operator RestoreRequestType(string value)
		{
			return FindValue(value);
		}
	}
}
