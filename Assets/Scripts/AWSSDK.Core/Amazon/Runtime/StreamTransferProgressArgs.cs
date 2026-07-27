using System;

namespace Amazon.Runtime
{
	public class StreamTransferProgressArgs : EventArgs
	{
		private long _incrementTransferred;

		private long _total;

		private long _transferred;

		public int PercentDone
		{
			get
			{
				return (int)(_transferred * 100 / _total);
			}
		}

		public long IncrementTransferred
		{
			get
			{
				return _incrementTransferred;
			}
		}

		public long TransferredBytes
		{
			get
			{
				return _transferred;
			}
		}

		public long TotalBytes
		{
			get
			{
				return _total;
			}
		}

		public StreamTransferProgressArgs(long incrementTransferred, long transferred, long total)
		{
			_incrementTransferred = incrementTransferred;
			_transferred = transferred;
			_total = total;
		}

		public override string ToString()
		{
			return "Transfer Statistics. Percentage completed: " + PercentDone + ", Bytes transferred: " + _transferred + ", Total bytes to transfer: " + _total;
		}
	}
}
