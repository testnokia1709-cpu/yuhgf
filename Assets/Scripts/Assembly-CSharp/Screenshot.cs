using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Screenshot : MonoBehaviour
{
	public static Screenshot Instance;

	public List<Camera> IncludeCameras = new List<Camera>();

	public List<GameObject> EnableObjects = new List<GameObject>();

	public RawImage DestinationImage;

	public Vector2 ImageSize;

	[HideInInspector]
	public byte[] ByteData;

	public Texture2D TextureData;

	private int m_sourceWidth;

	private int m_sourceHeight;

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public static Texture2D Capture(int width, int height, bool allCameras)
	{
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
		RenderTexture renderTexture = new RenderTexture(width, height, 24);
		if (allCameras)
		{
			Camera[] allCameras2 = Camera.allCameras;
			foreach (Camera camera in allCameras2)
			{
				camera.targetTexture = renderTexture;
				camera.Render();
				camera.targetTexture = null;
			}
		}
		else
		{
			Camera.main.targetTexture = renderTexture;
			Camera.main.Render();
			Camera.main.targetTexture = null;
		}
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(renderTexture);
		return texture2D;
	}

	public void Capture(int srcWidth, int srcHeight, int destWidth, int destHeight, bool fast, List<Camera> cameras = null, Action onComplete = null)
	{
		DestinationImage = null;
		ImageSize = new Vector2(destWidth, destHeight);
		Capture(srcWidth, srcHeight, fast, cameras, onComplete);
	}

	public void Capture(int width, int height, bool fast, List<Camera> cameras = null, Action onComplete = null)
	{
		m_sourceWidth = width;
		m_sourceHeight = height;
		StartCoroutine(Screenshot_RenderToTexAsynch(fast, cameras, onComplete));
	}

	private IEnumerator Screenshot_RenderToTexAsynch(bool fast, List<Camera> cameras = null, Action onComplete = null)
	{
		if (m_sourceWidth == 0 || m_sourceHeight == 0)
		{
			yield return 0;
		}
		if (cameras == null)
		{
			cameras = IncludeCameras;
		}
		Vector2 sourceSize = new Vector2(m_sourceWidth, m_sourceHeight);
		RenderTexture rt = new RenderTexture((int)sourceSize.x, (int)sourceSize.y, 24);
		if (!fast)
		{
			yield return new WaitForEndOfFrame();
		}
		foreach (GameObject enableObject in EnableObjects)
		{
			enableObject.SetActive(true);
		}
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			if (cameras.Contains(camera))
			{
				camera.targetTexture = rt;
				camera.Render();
				camera.targetTexture = null;
			}
		}
		foreach (GameObject enableObject2 in EnableObjects)
		{
			enableObject2.SetActive(false);
		}
		if (!fast)
		{
			yield return new WaitForEndOfFrame();
		}
		Vector2 destImageSize = ((!(DestinationImage != null)) ? ImageSize : new Vector2(DestinationImage.rectTransform.rect.width, DestinationImage.rectTransform.rect.height));
		if (!fast)
		{
			yield return new WaitForEndOfFrame();
		}
		float halfWidth = sourceSize.x / 2f;
		float halfHeight = sourceSize.y / 2f;
		int destHalfWidth = Mathf.FloorToInt(destImageSize.x / 2f);
		int destHalfHeight = Mathf.FloorToInt(destImageSize.y / 2f);
		float widthRatio = sourceSize.x / destImageSize.x;
		float heightRatio = sourceSize.y / destImageSize.y;
		float newHalfHeight = (float)destHalfHeight * widthRatio;
		float newHalfWidth = (float)destHalfWidth * heightRatio;
		float newHeight = newHalfHeight * 2f;
		float newWidth = newHalfWidth * 2f;
		Rect captureRect = ((!(newHeight < sourceSize.y)) ? new Rect(halfWidth - newHalfWidth, 0f, newWidth, sourceSize.y) : new Rect(0f, halfHeight - newHalfHeight, sourceSize.x, newHeight));
		Texture2D texture = new Texture2D((int)captureRect.width, (int)captureRect.height, TextureFormat.RGB24, false);
		if (!fast)
		{
			yield return new WaitForEndOfFrame();
		}
		RenderTexture.active = rt;
		texture.ReadPixels(new Rect(captureRect.x, captureRect.y, captureRect.width, captureRect.height), 0, 0);
		if (!fast)
		{
			yield return new WaitForEndOfFrame();
		}
		texture.Apply();
		if (!fast)
		{
			yield return new WaitForEndOfFrame();
		}
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(rt);
		if (!fast)
		{
			yield return new WaitForEndOfFrame();
		}
		TextureData = texture;
		if (DestinationImage != null)
		{
			DestinationImage.texture = texture;
			DestinationImage.color = Color.white;
		}
		else
		{
			ByteData = texture.EncodeToPNG();
		}
		if (onComplete != null)
		{
			onComplete();
		}
		yield return 0;
	}

	public void Screenshot_Sync(int srcWidth, int srcHeight, List<Camera> cameras = null)
	{
		if (srcWidth == 0 || srcHeight == 0)
		{
			return;
		}
		if (cameras == null)
		{
			cameras = IncludeCameras;
		}
		RenderTexture renderTexture = new RenderTexture(srcWidth, srcHeight, 24);
		bool[] array = new bool[EnableObjects.Count];
		int num = 0;
		foreach (GameObject enableObject in EnableObjects)
		{
			array[num++] = enableObject.activeInHierarchy;
			enableObject.SetActive(true);
		}
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			if (cameras.Contains(camera))
			{
				camera.targetTexture = renderTexture;
				camera.Render();
				camera.targetTexture = null;
			}
		}
		num = 0;
		foreach (GameObject enableObject2 in EnableObjects)
		{
			enableObject2.SetActive(array[num++]);
		}
		Vector2 vector = ((!(DestinationImage != null)) ? ImageSize : new Vector2(DestinationImage.rectTransform.rect.width, DestinationImage.rectTransform.rect.height));
		float num2 = srcWidth / 2;
		float num3 = srcHeight / 2;
		int num4 = Mathf.FloorToInt(vector.x / 2f);
		int num5 = Mathf.FloorToInt(vector.y / 2f);
		float num6 = (float)srcWidth / vector.x;
		float num7 = (float)srcHeight / vector.y;
		float num8 = (float)num5 * num6;
		float num9 = (float)num4 * num7;
		float num10 = num8 * 2f;
		float width = num9 * 2f;
		Rect rect = ((!(num10 < (float)srcHeight)) ? new Rect(num2 - num9, 0f, width, srcHeight) : new Rect(0f, num3 - num8, srcWidth, num10));
		Texture2D texture2D = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(rect.x, rect.y, rect.width, rect.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(renderTexture);
		TextureData = texture2D;
		if (DestinationImage != null)
		{
			DestinationImage.texture = texture2D;
			DestinationImage.color = Color.white;
		}
		else
		{
			ByteData = texture2D.EncodeToPNG();
		}
	}

	public static bool SaveImage(string filepath, Texture2D texture)
	{
		if (texture == null)
		{
			return false;
		}
		byte[] buffer = texture.EncodeToPNG();
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to write image at: " + filepath + " with exception: " + ex.Message);
		}
		if (fileStream == null)
		{
			return false;
		}
		try
		{
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(buffer);
			binaryWriter.Close();
			fileStream.Close();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return false;
		}
		return true;
	}
}
