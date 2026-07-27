namespace Amazon.Util
{
	public class AWSPublicIpAddressRange
	{
		public enum AddressFormat
		{
			Ipv4 = 0,
			Ipv6 = 1
		}

		public string IpPrefix { get; internal set; }

		public AddressFormat IpAddressFormat { get; internal set; }

		public string Region { get; internal set; }

		public string Service { get; internal set; }
	}
}
