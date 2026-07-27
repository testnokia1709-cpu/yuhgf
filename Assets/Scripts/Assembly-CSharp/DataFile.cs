using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataFile
{
	public static Texture2D LoadImage(string key)
	{
		string path = "image_" + key + ".dat";
		string text = Path.Combine(StoragePath.GetInternalStoragePath(), path);
		string text2 = Path.Combine(Application.persistentDataPath, path);
		string text3 = null;
		if (File.Exists(text))
		{
			text3 = text;
		}
		else if (File.Exists(text2))
		{
			text3 = text2;
		}
		if (string.IsNullOrEmpty(text3))
		{
			return null;
		}
		byte[] array = null;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		try
		{
			FileStream fileStream = new FileStream(text3, FileMode.Open, FileAccess.Read, FileShare.Read);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			num = binaryReader.ReadInt32();
			num2 = binaryReader.ReadInt32();
			num3 = binaryReader.ReadInt32();
			array = binaryReader.ReadBytes(num3);
			binaryReader.Close();
			fileStream.Close();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
		Texture2D texture2D = new Texture2D(num, num2);
		texture2D.LoadImage(array);
		return texture2D;
	}

	public static bool SaveImage(string key, Texture2D texture)
	{
		int width = texture.width;
		int height = texture.height;
		byte[] array = texture.EncodeToPNG();
		string path = "image_" + key + ".dat";
		string text = Path.Combine(Application.persistentDataPath, path);
		string text2 = Path.Combine(StoragePath.GetInternalStoragePath(), path);
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(text2, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to write image at: " + text2 + " with exception: " + ex.Message);
		}
		if (fileStream == null)
		{
			try
			{
				fileStream = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
			}
			catch (Exception ex2)
			{
				Debug.LogError("Failed to write image at: " + text + " with exception: " + ex2.Message);
			}
		}
		if (fileStream == null)
		{
			return false;
		}
		try
		{
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(width);
			binaryWriter.Write(height);
			binaryWriter.Write(array.Length);
			binaryWriter.Write(array);
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

	public static bool DeleteImage(string key)
	{
		string path = "image_" + key + ".dat";
		string text = Path.Combine(StoragePath.GetInternalStoragePath(), path);
		string text2 = Path.Combine(Application.persistentDataPath, path);
		string text3 = null;
		if (File.Exists(text))
		{
			text3 = text;
		}
		else if (File.Exists(text2))
		{
			text3 = text2;
		}
		if (!string.IsNullOrEmpty(text3))
		{
			try
			{
				File.Delete(text3);
				return true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		return false;
	}

	public static bool ExistsImage(string key)
	{
		string path = "image_" + key + ".dat";
		string text = Path.Combine(StoragePath.GetInternalStoragePath(), path);
		string text2 = Path.Combine(Application.persistentDataPath, path);
		string value = null;
		if (File.Exists(text))
		{
			value = text;
		}
		else if (File.Exists(text2))
		{
			value = text2;
		}
		return !string.IsNullOrEmpty(value);
	}

	public static bool ClearAllImages(HashSet<string> skipFilter = null)
	{
		return ClearAllImages(TimeSpan.FromSeconds(0.0), skipFilter);
	}

	public static bool ClearAllImages(TimeSpan olderThan, HashSet<string> skipFilter = null)
	{
		bool result = true;
		string searchPattern = "image_*.dat";
		string internalStoragePath = StoragePath.GetInternalStoragePath();
		DirectoryInfo directoryInfo = new DirectoryInfo(internalStoragePath);
		FileInfo[] files = directoryInfo.GetFiles(searchPattern);
		FileInfo[] array = files;
		foreach (FileInfo fileInfo in array)
		{
			if (skipFilter != null)
			{
				string item = Path.GetFileNameWithoutExtension(fileInfo.FullName).Replace("image_", string.Empty);
				if (skipFilter.Contains(item))
				{
					continue;
				}
			}
			if (!(DateTime.Now - fileInfo.CreationTime < olderThan))
			{
				try
				{
					fileInfo.Delete();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					result = false;
				}
			}
		}
		internalStoragePath = Application.persistentDataPath;
		directoryInfo = new DirectoryInfo(internalStoragePath);
		files = directoryInfo.GetFiles(searchPattern);
		FileInfo[] array2 = files;
		foreach (FileInfo fileInfo2 in array2)
		{
			if (skipFilter != null)
			{
				string item2 = Path.GetFileNameWithoutExtension(fileInfo2.FullName).Replace("image_", string.Empty);
				if (skipFilter.Contains(item2))
				{
					continue;
				}
			}
			if (!(DateTime.Now - fileInfo2.CreationTime < olderThan))
			{
				try
				{
					fileInfo2.Delete();
				}
				catch (Exception exception2)
				{
					Debug.LogException(exception2);
					result = false;
				}
			}
		}
		return result;
	}

	public static bool SaveText(string filename, string data)
	{
		string text = Path.Combine(StoragePath.GetInternalStoragePath(), filename);
		string text2 = Path.Combine(Application.persistentDataPath, filename);
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.Write);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to write file at: " + text + " with exception: " + ex.Message);
		}
		if (fileStream == null)
		{
			try
			{
				fileStream = new FileStream(text2, FileMode.Create, FileAccess.Write, FileShare.Write);
			}
			catch (Exception ex2)
			{
				Debug.LogError("Failed to write file at: " + text2 + " with exception: " + ex2.Message);
			}
		}
		if (fileStream == null)
		{
			return false;
		}
		try
		{
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(data);
			streamWriter.Close();
			fileStream.Close();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return false;
		}
		return true;
	}

	public static string LoadText(string filename)
	{
		string text = Path.Combine(StoragePath.GetInternalStoragePath(), filename);
		string text2 = Path.Combine(Application.persistentDataPath, filename);
		string text3 = null;
		string empty = string.Empty;
		if (File.Exists(text))
		{
			text3 = text;
		}
		else if (File.Exists(text2))
		{
			text3 = text2;
		}
		if (string.IsNullOrEmpty(text3))
		{
			return null;
		}
		try
		{
			FileStream fileStream = new FileStream(text3, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader streamReader = new StreamReader(fileStream);
			empty = streamReader.ReadToEnd();
			streamReader.Close();
			fileStream.Close();
			return empty;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	public static bool LoadFromFile<T>(string path, ref T data)
	{
		string storagePath = GetStoragePath(path);
		if (File.Exists(storagePath))
		{
			try
			{
				using (FileStream stream = new FileStream(storagePath, FileMode.Open))
				{
					using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
					{
						string json = streamReader.ReadToEnd();
						data = JsonUtility.FromJson<T>(json);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return false;
			}
			return true;
		}
		return false;
	}

	public static void SaveToFile<T>(string path, T data)
	{
		string storagePath = GetStoragePath(path);
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(storagePath, FileMode.Create, FileAccess.Write, FileShare.Write);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		if (fileStream == null)
		{
			return;
		}
		using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
		{
			try
			{
				string value = JsonUtility.ToJson(data);
				streamWriter.Write(value);
				streamWriter.Flush();
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}
	}

	public static bool Delete(string path)
	{
		string storagePath = GetStoragePath(path);
		if (File.Exists(storagePath))
		{
			try
			{
				File.Delete(storagePath);
				return true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		return false;
	}

	public static string GetStoragePath(string filepath)
	{
		return Path.Combine(StoragePath.GetInternalStoragePath(), filepath);
	}
}
