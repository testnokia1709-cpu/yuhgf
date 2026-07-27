using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Amazon.Runtime.Internal.Util;
using UnityEngine;

namespace Amazon.Runtime.Internal.Transform
{
	public sealed class UnityWebResponseData : IWebResponseData, IHttpResponseBody, IDisposable
	{
		private Dictionary<string, string> _headers;

		private string[] _headerNames;

		private HashSet<string> _headerNamesSet;

		private Stream _responseStream;

		private byte[] _responseBody;

		private Amazon.Runtime.Internal.Util.ILogger _logger;

		public long ContentLength { get; private set; }

		public string ContentType { get; private set; }

		public HttpStatusCode StatusCode { get; private set; }

		public bool IsSuccessStatusCode { get; private set; }

		public IHttpResponseBody ResponseBody
		{
			get
			{
				return this;
			}
		}

		public bool IsResponseBodyPresent
		{
			get
			{
				if (_responseBody != null)
				{
					return _responseBody.Length != 0;
				}
				return false;
			}
		}

		public UnityWebResponseData(UnityWebRequestWrapper unityWebRequest)
		{
			CopyHeaderValues(unityWebRequest.ResponseHeaders);
			if (!unityWebRequest.IsError)
			{
				_responseBody = unityWebRequest.DownloadHandler.Data;
				if (_responseBody == null)
				{
					_responseBody = new byte[0];
				}
				_responseStream = new MemoryStream(_responseBody);
				ContentLength = _responseBody.LongLength;
				string value = null;
				_headers.TryGetValue("Content-Type", out value);
				ContentType = value;
				if (unityWebRequest.StatusCode.HasValue)
				{
					StatusCode = unityWebRequest.StatusCode.Value;
				}
				IsSuccessStatusCode = StatusCode >= HttpStatusCode.OK && StatusCode <= (HttpStatusCode)299;
				return;
			}
			IsSuccessStatusCode = false;
			_responseBody = Encoding.UTF8.GetBytes(unityWebRequest.Error);
			_responseStream = new MemoryStream(_responseBody);
			if (unityWebRequest.DownloadedBytes != 0)
			{
				ContentLength = (long)unityWebRequest.DownloadedBytes;
			}
			else
			{
				string value2 = null;
				if (_headers.TryGetValue("Content-Length", out value2))
				{
					long result;
					if (long.TryParse(value2, out result))
					{
						ContentLength = result;
					}
					else
					{
						ContentLength = 0L;
					}
				}
				else
				{
					ContentLength = _responseBody.Length;
				}
			}
			if (unityWebRequest.StatusCode.HasValue)
			{
				StatusCode = unityWebRequest.StatusCode.Value;
				return;
			}
			string value3 = null;
			if (_headers.TryGetValue("Status", out value3))
			{
				StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), value3);
			}
			else
			{
				StatusCode = (HttpStatusCode)0;
			}
		}

		public UnityWebResponseData(WWW wwwRequest)
		{
			_logger = Amazon.Runtime.Internal.Util.Logger.GetLogger(GetType());
			CopyHeaderValues(wwwRequest.responseHeaders);
			try
			{
				_responseBody = wwwRequest.bytes;
			}
			catch (Exception)
			{
				_responseBody = null;
			}
			if ((_responseBody != null && _responseBody.Length != 0) || (_responseBody.Length == 0 && wwwRequest.error == null))
			{
				_responseStream = new MemoryStream(_responseBody);
			}
			ContentLength = wwwRequest.bytesDownloaded;
			string value = null;
			_headers.TryGetValue("Content-Type", out value);
			ContentType = value;
			try
			{
				int result;
				if (string.IsNullOrEmpty(wwwRequest.error))
				{
					string value2 = string.Empty;
					if (_headers.TryGetValue("Status", out value2))
					{
						StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), value2.Substring(9, 3).Trim());
					}
					else
					{
						StatusCode = (HttpStatusCode)0;
					}
				}
				else if (int.TryParse(wwwRequest.error.Substring(0, 3), out result))
				{
					StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), wwwRequest.error.Substring(3).Replace(" ", "").Replace(":", "")
						.Trim(), true);
				}
				else
				{
					StatusCode = (HttpStatusCode)0;
				}
			}
			catch
			{
				StatusCode = (HttpStatusCode)0;
			}
			IsSuccessStatusCode = wwwRequest.error == null;
		}

		public bool IsHeaderPresent(string headerName)
		{
			return _headerNamesSet.Contains(headerName);
		}

		public string[] GetHeaderNames()
		{
			return _headerNames;
		}

		public string GetHeaderValue(string name)
		{
			string value;
			if (_headers.TryGetValue(name, out value))
			{
				return value;
			}
			return string.Empty;
		}

		private void CopyHeaderValues(Dictionary<string, string> headers)
		{
			if (headers == null)
			{
				headers = new Dictionary<string, string>();
			}
			_headers = new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
			_headerNames = headers.Keys.ToArray();
			_headerNamesSet = new HashSet<string>(_headerNames, StringComparer.OrdinalIgnoreCase);
		}

		public Stream OpenResponse()
		{
			return _responseStream;
		}

		public void Dispose()
		{
			if (_responseStream != null)
			{
				_responseStream.Dispose();
				_responseStream = null;
			}
		}
	}
}
