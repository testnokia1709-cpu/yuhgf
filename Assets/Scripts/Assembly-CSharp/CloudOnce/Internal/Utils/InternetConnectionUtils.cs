using System.IO;
using System.Linq;
using System.Net;

namespace CloudOnce.Internal.Utils
{
	public static class InternetConnectionUtils
	{
		public static InternetConnectionStatus GetConnectionStatus()
		{
			string htmlFromUrl = GetHtmlFromUrl("http://google.com");
			if (string.IsNullOrEmpty(htmlFromUrl))
			{
				return InternetConnectionStatus.Disconnected;
			}
			if (!htmlFromUrl.Contains("schema.org/WebPage"))
			{
				return InternetConnectionStatus.Unstable;
			}
			return InternetConnectionStatus.Connected;
		}

		private static string GetHtmlFromUrl(string url, int charCount = 80)
		{
			string text = string.Empty;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					if (httpWebResponse.StatusCode < (HttpStatusCode)299 && httpWebResponse.StatusCode >= HttpStatusCode.OK)
					{
						Stream responseStream = httpWebResponse.GetResponseStream();
						if (responseStream != null)
						{
							using (StreamReader streamReader = new StreamReader(responseStream))
							{
								char[] array = new char[charCount];
								streamReader.Read(array, 0, array.Length);
								text = array.Aggregate(text, (string current, char ch) => current + ch);
							}
						}
					}
				}
			}
			catch
			{
				return string.Empty;
			}
			return text;
		}
	}
}
