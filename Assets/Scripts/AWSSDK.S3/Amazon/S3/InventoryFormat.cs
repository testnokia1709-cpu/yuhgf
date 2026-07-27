using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class InventoryFormat : ConstantClass
	{
		public static readonly InventoryFormat CSV = new InventoryFormat("CSV");

		public static readonly InventoryFormat ORC = new InventoryFormat("ORC");

		public InventoryFormat(string value)
			: base(value)
		{
		}

		public static InventoryFormat FindValue(string value)
		{
			return ConstantClass.FindValue<InventoryFormat>(value);
		}

		public static implicit operator InventoryFormat(string value)
		{
			return ConstantClass.FindValue<InventoryFormat>(value);
		}
	}
}
