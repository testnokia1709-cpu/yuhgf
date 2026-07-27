using System;
using UnityEngine;

namespace Amazon.Runtime.Internal.Util
{
	internal class UnityDebugLogger : InternalLogger
	{
		public override bool IsDebugEnabled
		{
			get
			{
				return true;
			}
		}

		public override bool IsErrorEnabled
		{
			get
			{
				return true;
			}
		}

		public override bool IsInfoEnabled
		{
			get
			{
				return true;
			}
		}

		public UnityDebugLogger(Type declaringType)
			: base(declaringType)
		{
		}

		public override void Flush()
		{
		}

		public override void Error(Exception exception, string messageFormat, params object[] args)
		{
			if (exception != null)
			{
				UnityEngine.Debug.LogException(exception);
			}
			if (!string.IsNullOrEmpty(messageFormat))
			{
				UnityEngine.Debug.LogError(string.Format(messageFormat, args));
			}
		}

		public override void Debug(Exception exception, string messageFormat, params object[] args)
		{
			if (exception != null)
			{
				UnityEngine.Debug.LogException(exception);
			}
			if (!string.IsNullOrEmpty(messageFormat))
			{
				UnityEngine.Debug.Log(string.Format(messageFormat, args));
			}
		}

		public override void DebugFormat(string messageFormat, params object[] args)
		{
			if (!string.IsNullOrEmpty(messageFormat))
			{
				UnityEngine.Debug.Log(string.Format(messageFormat, args));
			}
		}

		public override void InfoFormat(string message, params object[] arguments)
		{
			if (!string.IsNullOrEmpty(message))
			{
				UnityEngine.Debug.Log(string.Format(message, arguments));
			}
		}
	}
}
