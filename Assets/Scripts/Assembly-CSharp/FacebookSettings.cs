using System;
using UnityEngine;

[Serializable]
public struct FacebookSettings
{
	public static string s_imagePrefix = "fbprofile_";

	public string Name;

	public string FirstName;

	public string Token;

	public string UserId;

	public string Gender;

	public string Email;

	[NonSerialized]
	private byte[] m_profileImageData;

	public void SetProfileImage(Texture2D texture)
	{
		if (texture == null)
		{
			DataFile.DeleteImage(s_imagePrefix + "self");
			m_profileImageData = null;
		}
		else
		{
			DataFile.SaveImage(s_imagePrefix + "self", texture);
			m_profileImageData = texture.EncodeToPNG();
		}
	}

	public Texture2D GetProfileImage()
	{
		Texture2D texture2D = null;
		if (m_profileImageData == null)
		{
			texture2D = DataFile.LoadImage(s_imagePrefix + "self");
			if (texture2D != null)
			{
				m_profileImageData = texture2D.EncodeToPNG();
			}
		}
		else
		{
			texture2D = new Texture2D(128, 128);
			texture2D.LoadImage(m_profileImageData);
		}
		return texture2D;
	}
}
