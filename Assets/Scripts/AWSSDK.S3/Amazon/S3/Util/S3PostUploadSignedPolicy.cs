using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Auth;
using Amazon.Util;
using ThirdParty.Json.LitJson;

namespace Amazon.S3.Util
{
	[XmlRoot(IsNullable = false)]
	public class S3PostUploadSignedPolicy
	{
		private static string KEY_POLICY = "policy";

		private static string KEY_SIGNATURE = "signature";

		private static string KEY_ACCESSKEY = "access_key";

		public string Policy { get; set; }

		public string Signature { get; set; }

		public string AccessKeyId { get; set; }

		public string SecurityToken { get; set; }

		public string SignatureVersion { get; set; }

		public string Algorithm { get; set; }

		public string Date { get; set; }

		public string Credential { get; set; }

		public static S3PostUploadSignedPolicy GetSignedPolicy(string policy, AWSCredentials credentials)
		{
			ImmutableCredentials credentials2 = credentials.GetCredentials();
			string text = Convert.ToBase64String(credentials2.UseToken ? addTokenToPolicy(policy, credentials2.Token) : Encoding.UTF8.GetBytes(policy.Trim()));
			string signature = CryptoUtilFactory.CryptoInstance.HMACSign(Encoding.UTF8.GetBytes(text), credentials2.SecretKey, SigningAlgorithm.HmacSHA1);
			return new S3PostUploadSignedPolicy
			{
				Policy = text,
				Signature = signature,
				AccessKeyId = credentials2.AccessKey,
				SecurityToken = credentials2.Token,
				SignatureVersion = "2"
			};
		}

		public static S3PostUploadSignedPolicy GetSignedPolicyV4(string policy, AWSCredentials credentials, RegionEndpoint region)
		{
			DateTime correctedUtcNow = AWSSDKUtils.CorrectedUtcNow;
			ImmutableCredentials credentials2 = credentials.GetCredentials();
			string text = "AWS4-HMAC-SHA256";
			string text2 = AWS4Signer.FormatDateTime(correctedUtcNow, "yyyyMMdd");
			string text3 = AWS4Signer.FormatDateTime(correctedUtcNow, "yyyyMMddTHHmmssZ");
			string text4 = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/", credentials2.AccessKey, text2, region.SystemName, "s3", "aws4_request");
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					S3Constants.PostFormDataXAmzCredential,
					text4
				},
				{
					S3Constants.PostFormDataXAmzAlgorithm,
					text
				},
				{
					S3Constants.PostFormDataXAmzDate,
					text3
				}
			};
			if (credentials2.UseToken)
			{
				dictionary[S3Constants.PostFormDataSecurityToken] = credentials2.Token;
			}
			string text5 = Convert.ToBase64String(addConditionsToPolicy(policy, dictionary));
			byte[] key = AWS4Signer.ComposeSigningKey(credentials2.SecretKey, region.SystemName, text2, "s3");
			string signature = AWSSDKUtils.ToHex(AWS4Signer.ComputeKeyedHash(SigningAlgorithm.HmacSHA256, key, text5), true);
			return new S3PostUploadSignedPolicy
			{
				Policy = text5,
				Signature = signature,
				AccessKeyId = credentials2.AccessKey,
				SecurityToken = credentials2.Token,
				SignatureVersion = "4",
				Algorithm = text,
				Date = text3,
				Credential = text4
			};
		}

		private static byte[] addConditionsToPolicy(string policy, Dictionary<string, string> newConditions)
		{
			JsonData jsonData = JsonMapper.ToObject(new JsonReader(policy));
			JsonData jsonData2 = jsonData["conditions"];
			if (jsonData2 != null && jsonData2.IsArray)
			{
				foreach (KeyValuePair<string, string> newCondition in newConditions)
				{
					bool flag = false;
					for (int i = 0; i < jsonData2.Count; i++)
					{
						JsonData jsonData3 = jsonData2[i];
						if (jsonData3.IsObject && jsonData3[newCondition.Key] != null)
						{
							jsonData3[newCondition.Key] = newCondition.Value;
							flag = true;
						}
					}
					if (!flag)
					{
						JsonData jsonData4 = new JsonData();
						jsonData4.SetJsonType(JsonType.Object);
						jsonData4[newCondition.Key] = newCondition.Value;
						jsonData2.Add(jsonData4);
					}
				}
			}
			return Encoding.UTF8.GetBytes(JsonMapper.ToJson(jsonData).Trim());
		}

		private static byte[] addTokenToPolicy(string policy, string token)
		{
			JsonData jsonData = JsonMapper.ToObject(new JsonReader(policy));
			bool flag = false;
			JsonData jsonData2 = jsonData["conditions"];
			if (jsonData2 != null && jsonData2.IsArray)
			{
				for (int i = 0; i < jsonData2.Count; i++)
				{
					JsonData jsonData3 = jsonData2[i];
					if (jsonData3.IsObject && jsonData3[S3Constants.PostFormDataSecurityToken] != null)
					{
						jsonData3[S3Constants.PostFormDataSecurityToken] = token;
						flag = true;
					}
				}
				if (!flag)
				{
					JsonData jsonData4 = new JsonData();
					jsonData4.SetJsonType(JsonType.Object);
					jsonData4[S3Constants.PostFormDataSecurityToken] = token;
					jsonData2.Add(jsonData4);
				}
			}
			return Encoding.UTF8.GetBytes(JsonMapper.ToJson(jsonData).Trim());
		}

		public string GetReadablePolicy()
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(Policy));
		}

		public string ToJson()
		{
			JsonData jsonData = new JsonData();
			jsonData[KEY_POLICY] = Policy;
			jsonData[KEY_SIGNATURE] = Signature;
			jsonData[KEY_ACCESSKEY] = AccessKeyId;
			return JsonMapper.ToJson(jsonData);
		}

		public string ToXml()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			XmlSerializer xmlSerializer = new XmlSerializer(GetType());
			using (StringWriter textWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			return stringBuilder.ToString();
		}

		public static S3PostUploadSignedPolicy GetSignedPolicyFromJson(string policyJson)
		{
			JsonData jsonData;
			try
			{
				jsonData = JsonMapper.ToObject(policyJson);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Invalid JSON document", innerException);
			}
			if (jsonData[KEY_POLICY] == null || !jsonData[KEY_POLICY].IsString)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "JSON document requires '{0}' field"), KEY_POLICY);
			}
			if (jsonData[KEY_SIGNATURE] == null || !jsonData[KEY_SIGNATURE].IsString)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "JSON document requires '{0}' field"), KEY_SIGNATURE);
			}
			if (jsonData[KEY_ACCESSKEY] == null || !jsonData[KEY_ACCESSKEY].IsString)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "JSON document requires '{0}' field"), KEY_ACCESSKEY);
			}
			return new S3PostUploadSignedPolicy
			{
				Policy = jsonData[KEY_POLICY].ToString(),
				Signature = jsonData[KEY_SIGNATURE].ToString(),
				AccessKeyId = jsonData[KEY_ACCESSKEY].ToString()
			};
		}

		public static S3PostUploadSignedPolicy GetSignedPolicyFromXml(string policyXml)
		{
			StringReader textReader = new StringReader(policyXml);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(S3PostUploadSignedPolicy));
			S3PostUploadSignedPolicy s3PostUploadSignedPolicy;
			try
			{
				s3PostUploadSignedPolicy = xmlSerializer.Deserialize(textReader) as S3PostUploadSignedPolicy;
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Could not parse XML", innerException);
			}
			if (string.IsNullOrEmpty(s3PostUploadSignedPolicy.AccessKeyId))
			{
				throw new ArgumentException("XML Document requries 'AccessKeyId' field");
			}
			if (string.IsNullOrEmpty(s3PostUploadSignedPolicy.Policy))
			{
				throw new ArgumentException("XML Document requries 'Policy' field");
			}
			if (string.IsNullOrEmpty(s3PostUploadSignedPolicy.Signature))
			{
				throw new ArgumentException("XML Document requries 'Signature' field");
			}
			return s3PostUploadSignedPolicy;
		}
	}
}
