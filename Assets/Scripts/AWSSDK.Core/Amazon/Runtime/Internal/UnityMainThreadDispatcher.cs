using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.Util.Internal.PlatformServices;
using UnityEngine;

namespace Amazon.Runtime.Internal
{
	public class UnityMainThreadDispatcher : MonoBehaviour
	{
		private Amazon.Runtime.Internal.Util.Logger _logger;

		private float _nextUpdateTime;

		private float _updateInterval = 0.1f;

		private NetworkStatus _currentNetworkStatus;

		public void Awake()
		{
			_logger = Amazon.Runtime.Internal.Util.Logger.GetLogger(GetType());
			_nextUpdateTime = Time.unscaledTime;
			_nextUpdateTime += _updateInterval;
		}

		private void Update()
		{
			if (Time.unscaledTime >= _nextUpdateTime)
			{
				ProcessRequests();
				_nextUpdateTime += _updateInterval;
			}
		}

		private void ProcessRequests()
		{
			IUnityHttpRequest unityHttpRequest = UnityRequestQueue.Instance.DequeueRequest();
			if (unityHttpRequest != null)
			{
				StartCoroutine(InvokeRequest(unityHttpRequest));
			}
			RuntimeAsyncResult runtimeAsyncResult = UnityRequestQueue.Instance.DequeueCallback();
			if (runtimeAsyncResult != null && runtimeAsyncResult.Action != null)
			{
				try
				{
					runtimeAsyncResult.Action(runtimeAsyncResult.Request, runtimeAsyncResult.Response, runtimeAsyncResult.Exception, runtimeAsyncResult.AsyncOptions);
				}
				catch (Exception exception)
				{
					_logger.Error(exception, "An unhandled exception was thrown from the callback method {0}.", runtimeAsyncResult.Request.ToString());
				}
			}
			Action action = UnityRequestQueue.Instance.DequeueMainThreadOperation();
			if (action != null)
			{
				try
				{
					action();
				}
				catch (Exception exception2)
				{
					_logger.Error(exception2, "An unhandled exception was thrown from the callback method");
				}
			}
			Amazon.Util.Internal.PlatformServices.NetworkReachability networkReachability = ServiceFactory.Instance.GetService<INetworkReachability>() as Amazon.Util.Internal.PlatformServices.NetworkReachability;
			if (_currentNetworkStatus != networkReachability.NetworkStatus)
			{
				_currentNetworkStatus = networkReachability.NetworkStatus;
				networkReachability.OnNetworkReachabilityChanged(_currentNetworkStatus);
			}
		}

		private IEnumerator InvokeRequest(IUnityHttpRequest request)
		{
			if ((ServiceFactory.Instance.GetService<INetworkReachability>() as Amazon.Util.Internal.PlatformServices.NetworkReachability).NetworkStatus != NetworkStatus.NotReachable)
			{
				if (request is UnityWwwRequest)
				{
					WWW wwwRequest = new WWW((request as UnityWwwRequest).RequestUri.AbsoluteUri, request.RequestContent, request.Headers);
					if (wwwRequest == null)
					{
						yield return null;
					}
					bool uploadCompleted = false;
					while (!wwwRequest.isDone)
					{
						float uploadProgress = wwwRequest.uploadProgress;
						if (!uploadCompleted)
						{
							request.OnUploadProgressChanged(uploadProgress);
							if (uploadProgress == 1f)
							{
								uploadCompleted = true;
							}
						}
						yield return null;
					}
					request.WwwRequest = wwwRequest;
					request.Response = new UnityWebResponseData(wwwRequest);
				}
				else
				{
					UnityRequest unityRequest = request as UnityRequest;
					if (unityRequest == null)
					{
						yield return null;
					}
					UnityWebRequestWrapper unityWebRequest = new UnityWebRequestWrapper(unityRequest.RequestUri.AbsoluteUri, unityRequest.Method)
					{
						DownloadHandler = new DownloadHandlerBufferWrapper()
					};
					if (request.RequestContent != null && request.RequestContent.Length != 0)
					{
						unityWebRequest.UploadHandler = new UploadHandlerRawWrapper(request.RequestContent);
					}
					bool uploadCompleted2 = false;
					foreach (KeyValuePair<string, string> header in request.Headers)
					{
						unityWebRequest.SetRequestHeader(header.Key, header.Value);
					}
					AsyncOperation operation = unityWebRequest.Send();
					while (!operation.isDone)
					{
						float progress = operation.progress;
						if (!uploadCompleted2)
						{
							request.OnUploadProgressChanged(progress);
							if (progress == 1f)
							{
								uploadCompleted2 = true;
							}
						}
						yield return null;
					}
					request.WwwRequest = unityWebRequest;
					request.Response = new UnityWebResponseData(unityWebRequest);
				}
			}
			else
			{
				request.Exception = new WebException("Network Unavailable", WebExceptionStatus.ConnectFailure);
			}
			if (request.IsSync)
			{
				if (request.Response != null && !request.Response.IsSuccessStatusCode)
				{
					request.Exception = new HttpErrorResponseException(request.Response);
				}
				request.WaitHandle.Set();
				yield break;
			}
			if (request.Response != null && !request.Response.IsSuccessStatusCode)
			{
				request.Exception = new HttpErrorResponseException(request.Response);
			}
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					request.Callback(request.AsyncResult);
				}
				catch (Exception exception)
				{
					_logger.Error(exception, "An exception was thrown from the callback method executed fromUnityMainThreadDispatcher.InvokeRequest method.");
				}
			});
		}
	}
}
