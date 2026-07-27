using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using SimpleJSON;
using UnityEngine;

public class SaveData
{
	private static string s_filename = "save001.dat";

	private static string s_tempFilename = "tempsave.dat";

	private static string s_newfilename = "savegame.json";

	public static bool Save(DataStore data)
	{
		string text = Path.Combine(StoragePath.GetInternalStoragePath(), s_tempFilename);
		string text2 = Path.Combine(Application.persistentDataPath, s_tempFilename);
		string text3 = Path.Combine(StoragePath.GetInternalStoragePath(), s_newfilename);
		string text4 = Path.Combine(Application.persistentDataPath, s_newfilename);
		string text5 = null;
		string text6 = null;
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
			text5 = text;
			text6 = text3;
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to write save data at: " + text + " with exception: " + ex.Message);
		}
		if (fileStream == null)
		{
			AnalyticsManager.LogDebugEvent("InternalPathNotFound");
			try
			{
				fileStream = new FileStream(text2, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
				text5 = text2;
				text6 = text4;
			}
			catch (Exception ex2)
			{
				Debug.LogError("Failed to write save data at: " + text2 + " with exception: " + ex2.Message);
			}
		}
		if (fileStream == null)
		{
			AnalyticsManager.LogDebugEvent("PathNotFound");
			return false;
		}
		using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
		{
			try
			{
				data.BeforeSave();
				string value = JsonUtility.ToJson(data);
				streamWriter.Write(value);
				streamWriter.Flush();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return false;
			}
		}
		try
		{
			File.Copy(text5, text6, true);
		}
		catch (Exception exception2)
		{
			if (!File.Exists(text6))
			{
				Debug.LogException(exception2);
				return false;
			}
			try
			{
				File.Delete(text6);
				File.Copy(text5, text6, true);
			}
			catch (Exception exception3)
			{
				Debug.LogException(exception3);
				return false;
			}
		}
		try
		{
			File.Delete(text5);
		}
		catch (Exception exception4)
		{
			Debug.LogException(exception4);
		}
		return true;
	}

	public static DataStore Load()
	{
		DataStore dataStore = null;
		string text = Path.Combine(StoragePath.GetInternalStoragePath(), s_filename);
		string text2 = Path.Combine(Application.persistentDataPath, s_filename);
		string text3 = Path.Combine(StoragePath.GetInternalStoragePath(), s_newfilename);
		string text4 = Path.Combine(Application.persistentDataPath, s_newfilename);
		string path = null;
		SaveDataFormat format = SaveDataFormat.XML;
		if (File.Exists(text3))
		{
			path = text3;
			format = SaveDataFormat.JSON;
		}
		else if (File.Exists(text4))
		{
			path = text4;
			format = SaveDataFormat.JSON;
		}
		else if (File.Exists(text))
		{
			path = text;
			format = SaveDataFormat.XML;
		}
		else if (File.Exists(text2))
		{
			path = text2;
			format = SaveDataFormat.XML;
		}
		dataStore = LoadFromFile(path, format);
		if (dataStore != null)
		{
			dataStore.AfterLoad();
		}
		return dataStore;
	}

	private static DataStore LoadFromFile(string path, SaveDataFormat format)
	{
		DataStore data = null;
		if (!string.IsNullOrEmpty(path))
		{
			switch (format)
			{
			case SaveDataFormat.XML:
				try
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataStore));
					FileStream fileStream = new FileStream(path, FileMode.Open);
					StreamReader textReader = new StreamReader(fileStream, Encoding.UTF8);
					data = (DataStore)xmlSerializer.Deserialize(textReader);
					fileStream.Close();
				}
				catch (Exception exception2)
				{
					Debug.LogException(exception2);
				}
				break;
			case SaveDataFormat.JSON:
				try
				{
					using (FileStream stream = new FileStream(path, FileMode.Open))
					{
						using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
						{
							string text = streamReader.ReadToEnd();
							data = JsonUtility.FromJson<DataStore>(text);
							fixLostData(text, ref data);
						}
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				break;
			}
		}
		return data;
	}

	private static void fixLostData(string jsonStr, ref DataStore data)
	{
		try
		{
			Debug.Log("Found data that needs to be fixed!");
			JSONNode jSONNode = JSON.Parse(jsonStr);
			if (jSONNode["LevelsSolved"] != null)
			{
				Debug.Log("Found LevelsSolved...");
				int count = jSONNode["LevelsSolved"]["keys"].Count;
				List<string> list = new List<string>();
				for (int i = 0; i < count; i++)
				{
					list.Add(jSONNode["LevelsSolved"]["keys"][i]);
				}
				int count2 = jSONNode["LevelsSolved"]["values"].Count;
				List<int> list2 = new List<int>();
				for (int j = 0; j < count2; j++)
				{
					list2.Add(jSONNode["LevelsSolved"]["values"][j].AsInt);
				}
				if (list.Count == list2.Count)
				{
					for (int k = 0; k < list.Count; k++)
					{
						if (!data.LevelsSolved.ContainsKey(list[k]))
						{
							data.LevelsSolved.Add(list[k], list2[k]);
						}
					}
				}
			}
			if (jSONNode["LevelsLocked"] != null)
			{
				Debug.Log("Found LevelsLocked...");
				int count3 = jSONNode["LevelsLocked"]["keys"].Count;
				List<string> list3 = new List<string>();
				for (int l = 0; l < count3; l++)
				{
					list3.Add(jSONNode["LevelsLocked"]["keys"][l]);
				}
				int count4 = jSONNode["LevelsLocked"]["values"].Count;
				List<int> list4 = new List<int>();
				for (int m = 0; m < count4; m++)
				{
					list4.Add(jSONNode["LevelsLocked"]["values"][m].AsInt);
				}
				if (list3.Count == list4.Count)
				{
					for (int n = 0; n < list3.Count; n++)
					{
						if (!data.LevelsLocked.ContainsKey(list3[n]))
						{
							data.LevelsLocked.Add(list3[n], (list4[n] != 0) ? true : false);
						}
					}
				}
			}
			if (jSONNode["FreeItems"] != null)
			{
				Debug.Log("Found FreeItems...");
				int count5 = jSONNode["FreeItems"]["keys"].Count;
				List<string> list5 = new List<string>();
				for (int num = 0; num < count5; num++)
				{
					list5.Add(jSONNode["FreeItems"]["keys"][num]);
				}
				int count6 = jSONNode["FreeItems"]["values"].Count;
				List<int> list6 = new List<int>();
				for (int num2 = 0; num2 < count6; num2++)
				{
					list6.Add(jSONNode["FreeItems"]["values"][num2].AsInt);
				}
				if (list5.Count == list6.Count)
				{
					for (int num3 = 0; num3 < list5.Count; num3++)
					{
						if (!data.FreeItems.ContainsKey(list5[num3]))
						{
							data.FreeItems.Add(list5[num3], (list6[num3] != 0) ? true : false);
						}
					}
				}
			}
			if (jSONNode["LevelsMinShapeCount"] != null)
			{
				Debug.Log("Found LevelsMinShapeCount...");
				int count7 = jSONNode["LevelsMinShapeCount"]["keys"].Count;
				List<string> list7 = new List<string>();
				for (int num4 = 0; num4 < count7; num4++)
				{
					list7.Add(jSONNode["LevelsMinShapeCount"]["keys"][num4]);
				}
				int count8 = jSONNode["LevelsMinShapeCount"]["values"].Count;
				List<int> list8 = new List<int>();
				for (int num5 = 0; num5 < count8; num5++)
				{
					list8.Add(jSONNode["LevelsMinShapeCount"]["values"][num5].AsInt);
				}
				if (list7.Count == list8.Count)
				{
					for (int num6 = 0; num6 < list7.Count; num6++)
					{
						if (!data.LevelsMinShapeCount.ContainsKey(list7[num6]))
						{
							data.LevelsMinShapeCount.Add(list7[num6], list8[num6]);
						}
					}
				}
			}
			if (jSONNode["LevelsMinTime"] != null)
			{
				Debug.Log("Found LevelsMinTime...");
				int count9 = jSONNode["LevelsMinTime"]["keys"].Count;
				List<string> list9 = new List<string>();
				for (int num7 = 0; num7 < count9; num7++)
				{
					list9.Add(jSONNode["LevelsMinTime"]["keys"][num7]);
				}
				int count10 = jSONNode["LevelsMinTime"]["values"].Count;
				List<float> list10 = new List<float>();
				for (int num8 = 0; num8 < count10; num8++)
				{
					list10.Add(jSONNode["LevelsMinTime"]["values"][num8].AsFloat);
				}
				if (list9.Count == list10.Count)
				{
					for (int num9 = 0; num9 < list9.Count; num9++)
					{
						if (!data.LevelsMinTime.ContainsKey(list9[num9]))
						{
							data.LevelsMinTime.Add(list9[num9], list10[num9]);
						}
					}
				}
			}
			if (!(jSONNode["AchievementEarned"] != null))
			{
				return;
			}
			Debug.Log("Found AchievementEarned...");
			int count11 = jSONNode["AchievementEarned"]["keys"].Count;
			List<string> list11 = new List<string>();
			for (int num10 = 0; num10 < count11; num10++)
			{
				list11.Add(jSONNode["AchievementEarned"]["keys"][num10]);
			}
			int count12 = jSONNode["AchievementEarned"]["values"].Count;
			List<int> list12 = new List<int>();
			for (int num11 = 0; num11 < count12; num11++)
			{
				list12.Add(jSONNode["AchievementEarned"]["values"][num11].AsInt);
			}
			if (list11.Count != list12.Count)
			{
				return;
			}
			for (int num12 = 0; num12 < list11.Count; num12++)
			{
				if (!data.AchievementEarned.ContainsKey(list11[num12]))
				{
					data.AchievementEarned.Add(list11[num12], (list12[num12] != 0) ? true : false);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error fixing old data: " + ex.Message);
		}
	}

	private static void saveToPlayerPrefs(string dictionaryName, SerializableDictionary<string, int> dictionary)
	{
		int i;
		for (i = 0; PlayerPrefs.HasKey(dictionaryName + "_string_" + i); i++)
		{
			PlayerPrefs.DeleteKey(dictionaryName + "_string_" + i);
		}
		for (i = 0; PlayerPrefs.HasKey(dictionaryName + "_int_" + i); i++)
		{
			PlayerPrefs.DeleteKey(dictionaryName + "_int_" + i);
		}
		i = 0;
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			PlayerPrefs.SetString(dictionaryName + "_string_" + i, item.Key);
			PlayerPrefs.SetInt(dictionaryName + "_int_" + i, item.Value);
			i++;
		}
	}

	private static void saveToPlayerPrefs(string dictionaryName, SerializableDictionary<string, float> dictionary)
	{
		int i;
		for (i = 0; PlayerPrefs.HasKey(dictionaryName + "_string_" + i); i++)
		{
			PlayerPrefs.DeleteKey(dictionaryName + "_string_" + i);
		}
		for (i = 0; PlayerPrefs.HasKey(dictionaryName + "_float_" + i); i++)
		{
			PlayerPrefs.DeleteKey(dictionaryName + "_float_" + i);
		}
		i = 0;
		foreach (KeyValuePair<string, float> item in dictionary)
		{
			PlayerPrefs.SetString(dictionaryName + "_string_" + i, item.Key);
			PlayerPrefs.SetFloat(dictionaryName + "_float_" + i, item.Value);
			i++;
		}
	}

	private static void saveToPlayerPrefs(string dictionaryName, SerializableDictionary<string, bool> dictionary)
	{
		int i;
		for (i = 0; PlayerPrefs.HasKey(dictionaryName + "_string_" + i); i++)
		{
			PlayerPrefs.DeleteKey(dictionaryName + "_string_" + i);
		}
		for (i = 0; PlayerPrefs.HasKey(dictionaryName + "_bool_" + i); i++)
		{
			PlayerPrefs.DeleteKey(dictionaryName + "_bool_" + i);
		}
		i = 0;
		foreach (KeyValuePair<string, bool> item in dictionary)
		{
			PlayerPrefs.SetString(dictionaryName + "_string_" + i, item.Key);
			PlayerPrefs.SetInt(dictionaryName + "_bool_" + i, item.Value ? 1 : 0);
			i++;
		}
	}

	private static void loadFromPlayerPrefs(string dictionaryName, SerializableDictionary<string, int> dictionary)
	{
		for (int i = 0; PlayerPrefs.HasKey(dictionaryName + "_string_" + i); i++)
		{
			string key = PlayerPrefs.GetString(dictionaryName + "_string_" + i);
			int value = PlayerPrefs.GetInt(dictionaryName + "_int_" + i);
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}
	}

	private static void loadFromPlayerPrefs(string dictionaryName, SerializableDictionary<string, float> dictionary)
	{
		for (int i = 0; PlayerPrefs.HasKey(dictionaryName + "_string_" + i); i++)
		{
			string key = PlayerPrefs.GetString(dictionaryName + "_string_" + i);
			float value = PlayerPrefs.GetFloat(dictionaryName + "_float_" + i);
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}
	}

	private static void loadFromPlayerPrefs(string dictionaryName, SerializableDictionary<string, bool> dictionary)
	{
		for (int i = 0; PlayerPrefs.HasKey(dictionaryName + "_string_" + i); i++)
		{
			string key = PlayerPrefs.GetString(dictionaryName + "_string_" + i);
			int num = PlayerPrefs.GetInt(dictionaryName + "_bool_" + i);
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = num == 1;
			}
			else
			{
				dictionary.Add(key, num == 1);
			}
		}
	}
}
