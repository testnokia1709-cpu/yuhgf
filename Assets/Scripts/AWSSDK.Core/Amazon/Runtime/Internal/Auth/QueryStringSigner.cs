using System;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;

namespace Amazon.Runtime.Internal.Auth
{
	public class QueryStringSigner : AbstractAWSSigner
	{
		private const string SignatureVersion2 = "2";

		public override ClientProtocol Protocol
		{
			get
			{
				return ClientProtocol.QueryStringProtocol;
			}
		}

		public override void Sign(IRequest request, IClientConfig clientConfig, RequestMetrics metrics, string awsAccessKeyId, string awsSecretAccessKey)
		{
			if (string.IsNullOrEmpty(awsAccessKeyId))
			{
				throw new ArgumentOutOfRangeException("awsAccessKeyId", "The AWS Access Key ID cannot be NULL or a Zero length string");
			}
			request.Parameters["AWSAccessKeyId"] = awsAccessKeyId;
			request.Parameters["SignatureVersion"] = "2";
			request.Parameters["SignatureMethod"] = clientConfig.SignatureMethod.ToString();
			request.Parameters["Timestamp"] = AWSSDKUtils.GetFormattedTimestampISO8601(clientConfig);
			request.Parameters.Remove("Signature");
			string text = AWSSDKUtils.CalculateStringToSignV2(request.ParameterCollection, request.Endpoint.AbsoluteUri);
			metrics.AddProperty(Metric.StringToSign, text);
			string value = AbstractAWSSigner.ComputeHash(text, awsSecretAccessKey, clientConfig.SignatureMethod);
			request.Parameters["Signature"] = value;
		}
	}
}
