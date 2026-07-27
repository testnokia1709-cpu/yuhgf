using System;
using System.Collections.Generic;
using System.Threading;

namespace Amazon.Runtime
{
	public static class CorrectClockSkew
	{
		private static TimeSpan? manualClockCorrection;

		private static ReaderWriterLockSlim manualClockCorrectionLock = new ReaderWriterLockSlim();

		private static IDictionary<string, TimeSpan> clockCorrectionDictionary = new Dictionary<string, TimeSpan>();

		private static ReaderWriterLockSlim clockCorrectionDictionaryLock = new ReaderWriterLockSlim();

		internal static TimeSpan? GlobalClockCorrection
		{
			get
			{
				manualClockCorrectionLock.EnterReadLock();
				TimeSpan? result = manualClockCorrection;
				manualClockCorrectionLock.ExitReadLock();
				return result;
			}
			set
			{
				manualClockCorrectionLock.EnterWriteLock();
				manualClockCorrection = value;
				manualClockCorrectionLock.ExitWriteLock();
			}
		}

		public static TimeSpan GetClockCorrectionForEndpoint(string endpoint)
		{
			bool flag = false;
			clockCorrectionDictionaryLock.EnterReadLock();
			TimeSpan value;
			try
			{
				flag = clockCorrectionDictionary.TryGetValue(endpoint, out value);
			}
			finally
			{
				clockCorrectionDictionaryLock.ExitReadLock();
			}
			if (!flag)
			{
				return TimeSpan.Zero;
			}
			return value;
		}

		public static DateTime GetCorrectedUtcNowForEndpoint(string endpoint)
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			manualClockCorrectionLock.EnterReadLock();
			try
			{
				if (manualClockCorrection.HasValue)
				{
					timeSpan = manualClockCorrection.Value;
				}
			}
			finally
			{
				manualClockCorrectionLock.ExitReadLock();
			}
			if (AWSConfigs.CorrectForClockSkew && timeSpan == TimeSpan.Zero)
			{
				timeSpan = GetClockCorrectionForEndpoint(endpoint);
			}
			return AWSConfigs.utcNowSource() + timeSpan;
		}

		internal static void SetClockCorrectionForEndpoint(string endpoint, TimeSpan correction)
		{
			clockCorrectionDictionaryLock.EnterWriteLock();
			try
			{
				clockCorrectionDictionary[endpoint] = correction;
				AWSConfigs.ClockOffset = correction;
			}
			finally
			{
				clockCorrectionDictionaryLock.ExitWriteLock();
			}
		}
	}
}
