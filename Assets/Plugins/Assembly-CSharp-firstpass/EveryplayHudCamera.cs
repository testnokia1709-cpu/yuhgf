using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class EveryplayHudCamera : MonoBehaviour
{
	private const int EPSR = 1162892114;

	private bool subscribed;

	private bool readyForRecording;

	private IntPtr renderEventPtr = IntPtr.Zero;

	private bool isMetalDevice;

	private bool isAndroidDevice;

	private void Awake()
	{
		Subscribe(true);
		readyForRecording = Everyplay.IsReadyForRecording();
		if (readyForRecording)
		{
			renderEventPtr = EveryplayGetUnityRenderEventPtr();
		}
		isMetalDevice = SystemInfo.graphicsDeviceVersion.Contains("Metal") && !SystemInfo.graphicsDeviceVersion.Contains("OpenGL");
		isAndroidDevice = Application.platform == RuntimePlatform.Android;
	}

	private void OnDestroy()
	{
		Subscribe(false);
	}

	private void OnEnable()
	{
		Subscribe(true);
	}

	private void OnDisable()
	{
		Subscribe(false);
	}

	private void Subscribe(bool subscribe)
	{
		if (!subscribed && subscribe)
		{
			Everyplay.ReadyForRecording += ReadyForRecording;
		}
		else if (subscribed && !subscribe)
		{
			Everyplay.ReadyForRecording -= ReadyForRecording;
		}
		subscribed = subscribe;
	}

	private void ReadyForRecording(bool ready)
	{
		if (ready && renderEventPtr == IntPtr.Zero)
		{
			renderEventPtr = EveryplayGetUnityRenderEventPtr();
		}
		readyForRecording = ready;
	}

	private void OnPreRender()
	{
		if (readyForRecording && renderEventPtr != IntPtr.Zero)
		{
			if (isMetalDevice || isAndroidDevice)
			{
				GL.IssuePluginEvent(renderEventPtr, 1162892114);
			}
			else
			{
				Everyplay.SnapshotRenderbuffer();
			}
		}
	}

	[DllImport("everyplay")]
	private static extern IntPtr EveryplayGetUnityRenderEventPtr();
}
