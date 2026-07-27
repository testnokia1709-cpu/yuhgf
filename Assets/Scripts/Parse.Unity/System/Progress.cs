using Parse.Internal;

namespace System
{
	public class Progress<T> : IProgress<T> where T : EventArgs
	{
		private SynchronizedEventHandler<T> progressChanged = new SynchronizedEventHandler<T>();

		public event EventHandler<T> ProgressChanged
		{
			add
			{
				progressChanged.Add(value);
			}
			remove
			{
				progressChanged.Remove(value);
			}
		}

		public Progress()
		{
			ProgressChanged += delegate(object sender, T args)
			{
				OnReport(args);
			};
		}

		public Progress(Action<T> handler)
			: this()
		{
			ProgressChanged += delegate(object sender, T args)
			{
				handler(args);
			};
			ProgressChanged += delegate(object sender, T args)
			{
				OnReport(args);
			};
		}

		void IProgress<T>.Report(T value)
		{
			progressChanged.Invoke(this, value);
		}

		protected virtual void OnReport(T value)
		{
		}
	}
}
