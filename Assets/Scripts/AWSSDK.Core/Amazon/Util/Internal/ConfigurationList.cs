using System.Collections.Generic;

namespace Amazon.Util.Internal
{
	public abstract class ConfigurationList<T> : List<T>
	{
		public IEnumerable<T> Items
		{
			get
			{
				return this;
			}
		}
	}
}
