using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class FileHeaderInfo : ConstantClass
	{
		public static readonly FileHeaderInfo Use = new FileHeaderInfo("USE");

		public static readonly FileHeaderInfo Ignore = new FileHeaderInfo("IGNORE");

		public static readonly FileHeaderInfo None = new FileHeaderInfo("NONE");

		private FileHeaderInfo(string value)
			: base(value)
		{
		}

		public static FileHeaderInfo FindValue(string value)
		{
			return ConstantClass.FindValue<FileHeaderInfo>(value);
		}

		public static implicit operator FileHeaderInfo(string value)
		{
			return FindValue(value);
		}
	}
}
