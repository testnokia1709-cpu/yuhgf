using System;
using Amazon.Util;

namespace Amazon.Runtime.Internal.Transform
{
	public static class CustomMarshallTransformations
	{
		public static long ConvertDateTimeToEpochMilliseconds(DateTime dateTime)
		{
			TimeSpan timeSpan = new TimeSpan(dateTime.ToUniversalTime().Ticks - AWSSDKUtils.EPOCH_START.Ticks);
			return (long)timeSpan.TotalMilliseconds;
		}
	}
}
