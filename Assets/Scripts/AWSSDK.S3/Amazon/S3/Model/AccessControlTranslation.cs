namespace Amazon.S3.Model
{
	public class AccessControlTranslation
	{
		private OwnerOverride owner;

		public OwnerOverride Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}

		internal bool IsSetOwner()
		{
			return owner != null;
		}
	}
}
