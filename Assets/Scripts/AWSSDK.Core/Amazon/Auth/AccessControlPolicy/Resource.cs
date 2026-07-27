namespace Amazon.Auth.AccessControlPolicy
{
	public class Resource
	{
		private string resource;

		public string Id
		{
			get
			{
				return resource;
			}
		}

		public Resource(string resource)
		{
			this.resource = resource;
		}
	}
}
