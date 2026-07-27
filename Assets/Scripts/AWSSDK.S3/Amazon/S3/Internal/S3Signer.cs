using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Util;
using Amazon.Util;

namespace Amazon.S3.Internal
{
	public class S3Signer : AbstractAWSSigner
	{
		private readonly bool _useSigV4;

		private static readonly HashSet<string> SignableParameters = new HashSet<string>(new string[6] { "response-content-type", "response-content-language", "response-expires", "response-cache-control", "response-content-disposition", "response-content-encoding" }, StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> SubResourcesSigningExclusion = new HashSet<string>(new string[1] { "id" }, StringComparer.OrdinalIgnoreCase);

		public override ClientProtocol Protocol
		{
			get
			{
				return ClientProtocol.RestProtocol;
			}
		}

		public S3Signer()
		{
			_useSigV4 = AWSConfigsS3.UseSignatureVersion4;
		}

		public override void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			AWS4Signer aWS4Signer = SelectSigner(this, _useSigV4, request, clientConfig) as AWS4Signer;
			if (aWS4Signer != null)
			{
				AmazonS3Uri amazonS3Uri;
				RegionEndpoint value;
				if (AmazonS3Uri.TryParseAmazonS3Uri(request.Endpoint, out amazonS3Uri) && amazonS3Uri.Bucket != null && BucketRegionDetector.BucketRegionCache.TryGetValue(amazonS3Uri.Bucket, out value))
				{
					request.AlternateEndpoint = value;
				}
				AWS4SigningResult aWS4SigningResult = aWS4Signer.SignRequest(request, clientConfig, metrics, awsAccessKeyId, awsSecretAccessKey);
				request.Headers["Authorization"] = aWS4SigningResult.ForAuthorizationHeader;
				if (request.UseChunkEncoding)
				{
					request.AWS4SignerResult = aWS4SigningResult;
				}
			}
			else
			{
				SignRequest(request, metrics, awsAccessKeyId, awsSecretAccessKey);
			}
		}

		internal static void SignRequest(IRequest request, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			request.Headers["X-Amz-Date"] = AWSSDKUtils.FormattedCurrentTimestampRFC822;
			string text = BuildStringToSign(request);
			metrics.AddProperty(Metric.StringToSign, text);
			string text2 = CryptoUtilFactory.CryptoInstance.HMACSign(text, awsSecretAccessKey, SigningAlgorithm.HmacSHA1);
			string value = "AWS " + awsAccessKeyId + ":" + text2;
			request.Headers["Authorization"] = value;
		}

		private static string BuildStringToSign(IRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder("", 256);
			stringBuilder.Append(request.HttpMethod);
			stringBuilder.Append("\n");
			IDictionary<string, string> headers = request.Headers;
			IDictionary<string, string> parameters = request.Parameters;
			if (headers != null)
			{
				string text = null;
				if (headers.ContainsKey("Content-MD5") && !string.IsNullOrEmpty(text = headers["Content-MD5"]))
				{
					stringBuilder.Append(text);
				}
				stringBuilder.Append("\n");
				if (parameters.ContainsKey("ContentType"))
				{
					stringBuilder.Append(parameters["ContentType"]);
				}
				else if (headers.ContainsKey("Content-Type"))
				{
					stringBuilder.Append(headers["Content-Type"]);
				}
				stringBuilder.Append("\n");
			}
			else
			{
				stringBuilder.Append("\n\n");
			}
			if (parameters.ContainsKey("Expires"))
			{
				stringBuilder.Append(parameters["Expires"]);
				if (headers != null)
				{
					headers.Remove("X-Amz-Date");
				}
			}
			IDictionary<string, string> dictionary = new Dictionary<string, string>(headers);
			foreach (KeyValuePair<string, string> item in parameters)
			{
				if (!dictionary.ContainsKey(item.Key))
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			stringBuilder.Append("\n");
			stringBuilder.Append(BuildCanonicalizedHeaders(dictionary));
			string value = BuildCanonicalizedResource(request);
			if (!string.IsNullOrEmpty(value))
			{
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		private static string BuildCanonicalizedHeaders(IDictionary<string, string> headers)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			foreach (string item in headers.Keys.OrderBy((string x) => x, StringComparer.OrdinalIgnoreCase))
			{
				string text = item.ToLowerInvariant();
				if (text.StartsWith("x-amz-", StringComparison.Ordinal))
				{
					stringBuilder.Append(text + ":" + headers[item] + "\n");
				}
			}
			return stringBuilder.ToString();
		}

		private static string BuildCanonicalizedResource(IRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder(request.CanonicalResourcePrefix);
			stringBuilder.Append((!string.IsNullOrEmpty(request.ResourcePath)) ? AWSSDKUtils.UrlEncode(request.ResourcePath, true) : "/");
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (request.SubResources.Count > 0)
			{
				foreach (KeyValuePair<string, string> subResource in request.SubResources)
				{
					if (!SubResourcesSigningExclusion.Contains(subResource.Key))
					{
						dictionary.Add(subResource.Key, subResource.Value);
					}
				}
			}
			if (request.Parameters.Count > 0)
			{
				foreach (KeyValuePair<string, string> sortedParameters in request.ParameterCollection.GetSortedParametersList())
				{
					if (sortedParameters.Value != null && SignableParameters.Contains(sortedParameters.Key))
					{
						dictionary.Add(sortedParameters.Key, sortedParameters.Value);
					}
				}
			}
			string arg = "?";
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				list.Add(item);
			}
			list.Sort((KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair) => string.CompareOrdinal(firstPair.Key, nextPair.Key));
			foreach (KeyValuePair<string, string> item2 in list)
			{
				stringBuilder.AppendFormat("{0}{1}", arg, item2.Key);
				if (item2.Value != null)
				{
					stringBuilder.AppendFormat("={0}", item2.Value);
				}
				arg = "&";
			}
			return stringBuilder.ToString();
		}
	}
}
