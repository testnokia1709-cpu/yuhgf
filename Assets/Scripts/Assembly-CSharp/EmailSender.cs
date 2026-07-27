using System;
using System.Text;
using UnityEngine;

public class EmailSender
{
	public static void Send(string toAddress, string subject, string body)
	{
		StringBuilder stringBuilder = new StringBuilder("mailto:");
		stringBuilder.Append((toAddress != null) ? toAddress : string.Empty);
		if (!string.IsNullOrEmpty(subject))
		{
			stringBuilder.AppendFormat("?subject={0}", subject);
		}
		stringBuilder.AppendFormat("&body=\n\n\n---\nDevice: {0} {1}\nGame Version: {2}\n\n", SystemInfo.deviceModel, SystemInfo.deviceName, TextLibrary.AppVersion);
		if (!string.IsNullOrEmpty(body))
		{
			stringBuilder.Append(body);
		}
		string url = Uri.EscapeUriString(stringBuilder.ToString());
		Application.OpenURL(url);
	}
}
