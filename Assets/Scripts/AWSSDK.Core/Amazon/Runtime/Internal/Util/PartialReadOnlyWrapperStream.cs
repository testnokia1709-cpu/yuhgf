using System;
using System.IO;

namespace Amazon.Runtime.Internal.Util
{
	public class PartialReadOnlyWrapperStream : ReadOnlyWrapperStream
	{
		private long _currentPosition;

		private long _size;

		private long RemainingSize
		{
			get
			{
				return _size - _currentPosition;
			}
		}

		public override long Length
		{
			get
			{
				return _size;
			}
		}

		public override long Position
		{
			get
			{
				return _currentPosition;
			}
		}

		public PartialReadOnlyWrapperStream(Stream baseStream, long size)
			: base(baseStream)
		{
			_currentPosition = 0L;
			_size = size;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			long num = Math.Min(count, RemainingSize);
			if (num <= 0)
			{
				return 0;
			}
			num = Math.Min(num, 2147483647L);
			int num2 = base.Read(buffer, offset, (int)num);
			_currentPosition += num2;
			return num2;
		}
	}
}
