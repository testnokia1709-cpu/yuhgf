using System.Diagnostics;
using UnityEngine;

namespace Amazon.Runtime.Internal.Util
{
	public class UnityDebugTraceListener : TraceListener
	{
		public override bool IsThreadSafe
		{
			get
			{
				return true;
			}
		}

		public UnityDebugTraceListener()
		{
		}

		public UnityDebugTraceListener(string name)
			: base(name)
		{
		}

		public override void Write(string message)
		{
			UnityEngine.Debug.Log(message);
		}

		public override void WriteLine(string message)
		{
			UnityEngine.Debug.Log(message);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			LogMessage(string.Format(format, args), eventType);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			LogMessage(data.ToString(), eventType);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			LogMessage(message, eventType);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			foreach (object obj in data)
			{
				if (obj != null)
				{
					LogMessage(obj.ToString(), eventType);
				}
			}
		}

		public override void Fail(string message)
		{
			UnityEngine.Debug.LogError(message);
		}

		public override void Fail(string message, string detailMessage)
		{
			UnityEngine.Debug.LogError(message + " " + detailMessage);
		}

		public override void Write(object o)
		{
			UnityEngine.Debug.Log(o.ToString());
		}

		public override void WriteLine(object o)
		{
			UnityEngine.Debug.Log(o.ToString());
		}

		public override void WriteLine(object o, string category)
		{
			UnityEngine.Debug.Log(o.ToString());
		}

		private void LogMessage(string message, TraceEventType eventType)
		{
			if (eventType.Equals(TraceEventType.Critical) || eventType.Equals(TraceEventType.Error))
			{
				UnityEngine.Debug.LogError(message);
			}
			else if (eventType.Equals(TraceEventType.Warning))
			{
				UnityEngine.Debug.LogWarning(message);
			}
			else
			{
				UnityEngine.Debug.Log(message);
			}
		}
	}
}
