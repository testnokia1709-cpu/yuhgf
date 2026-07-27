using System;
using System.Threading;

namespace Amazon.Runtime.Internal
{
	internal class SimpleAsyncResult : IAsyncResult
	{
		public object AsyncState { get; private set; }

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return true;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return true;
			}
		}

		public SimpleAsyncResult(object state)
		{
			AsyncState = state;
		}
	}
}
