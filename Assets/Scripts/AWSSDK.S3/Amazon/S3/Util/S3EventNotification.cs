using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Amazon.Runtime;
using ThirdParty.Json.LitJson;

namespace Amazon.S3.Util
{
	public class S3EventNotification
	{
		public class UserIdentityEntity
		{
			public string PrincipalId { get; set; }
		}

		public class S3BucketEntity
		{
			public string Name { get; set; }

			public UserIdentityEntity OwnerIdentity { get; set; }

			public string Arn { get; set; }
		}

		public class S3ObjectEntity
		{
			public string Key { get; set; }

			public long Size { get; set; }

			public string ETag { get; set; }

			public string VersionId { get; set; }
		}

		public class S3Entity
		{
			public string ConfigurationId { get; set; }

			public S3BucketEntity Bucket { get; set; }

			public S3ObjectEntity Object { get; set; }

			public string S3SchemaVersion { get; set; }
		}

		public class RequestParametersEntity
		{
			public string SourceIPAddress { get; set; }
		}

		public class ResponseElementsEntity
		{
			public string XAmzId2 { get; set; }

			public string XAmzRequestId { get; set; }
		}

		public class S3EventNotificationRecord
		{
			public string AwsRegion { get; set; }

			public EventType EventName { get; set; }

			public string EventSource { get; set; }

			public DateTime EventTime { get; set; }

			public string EventVersion { get; set; }

			public RequestParametersEntity RequestParameters { get; set; }

			public ResponseElementsEntity ResponseElements { get; set; }

			public S3Entity S3 { get; set; }

			public UserIdentityEntity UserIdentity { get; set; }
		}

		public List<S3EventNotificationRecord> Records { get; set; }

		public static S3EventNotification ParseJson(string json)
		{
			try
			{
				JsonData jsonData = JsonMapper.ToObject(json);
				S3EventNotification s3EventNotification = new S3EventNotification
				{
					Records = new List<S3EventNotificationRecord>()
				};
				if (jsonData["Records"] != null)
				{
					foreach (JsonData item in (IEnumerable)jsonData["Records"])
					{
						S3EventNotificationRecord s3EventNotificationRecord = new S3EventNotificationRecord();
						s3EventNotificationRecord.EventVersion = GetValueAsString(item, "eventVersion");
						s3EventNotificationRecord.EventSource = GetValueAsString(item, "eventSource");
						s3EventNotificationRecord.AwsRegion = GetValueAsString(item, "awsRegion");
						s3EventNotificationRecord.EventVersion = GetValueAsString(item, "eventVersion");
						if (item["eventTime"] != null)
						{
							s3EventNotificationRecord.EventTime = DateTime.Parse((string)item["eventTime"], CultureInfo.InvariantCulture);
						}
						if (item["eventName"] != null)
						{
							string text = (string)item["eventName"];
							if (!text.StartsWith("s3:", StringComparison.OrdinalIgnoreCase))
							{
								text = "s3:" + text;
							}
							s3EventNotificationRecord.EventName = EventType.FindValue(text);
						}
						if (item["userIdentity"] != null)
						{
							JsonData data = item["userIdentity"];
							s3EventNotificationRecord.UserIdentity = new UserIdentityEntity();
							s3EventNotificationRecord.UserIdentity.PrincipalId = GetValueAsString(data, "principalId");
						}
						if (item["requestParameters"] != null)
						{
							JsonData data2 = item["requestParameters"];
							s3EventNotificationRecord.RequestParameters = new RequestParametersEntity();
							s3EventNotificationRecord.RequestParameters.SourceIPAddress = GetValueAsString(data2, "sourceIPAddress");
						}
						if (item["responseElements"] != null)
						{
							JsonData data3 = item["responseElements"];
							s3EventNotificationRecord.ResponseElements = new ResponseElementsEntity();
							s3EventNotificationRecord.ResponseElements.XAmzRequestId = GetValueAsString(data3, "x-amz-request-id");
							s3EventNotificationRecord.ResponseElements.XAmzId2 = GetValueAsString(data3, "x-amz-id-2");
						}
						if (item["s3"] != null)
						{
							JsonData jsonData3 = item["s3"];
							s3EventNotificationRecord.S3 = new S3Entity();
							s3EventNotificationRecord.S3.S3SchemaVersion = GetValueAsString(jsonData3, "s3SchemaVersion");
							s3EventNotificationRecord.S3.ConfigurationId = GetValueAsString(jsonData3, "configurationId");
							if (jsonData3["bucket"] != null)
							{
								JsonData jsonData4 = jsonData3["bucket"];
								s3EventNotificationRecord.S3.Bucket = new S3BucketEntity();
								s3EventNotificationRecord.S3.Bucket.Name = GetValueAsString(jsonData4, "name");
								s3EventNotificationRecord.S3.Bucket.Arn = GetValueAsString(jsonData4, "arn");
								if (jsonData4["ownerIdentity"] != null)
								{
									JsonData data4 = jsonData4["ownerIdentity"];
									s3EventNotificationRecord.S3.Bucket.OwnerIdentity = new UserIdentityEntity();
									s3EventNotificationRecord.S3.Bucket.OwnerIdentity.PrincipalId = GetValueAsString(data4, "principalId");
								}
							}
							if (jsonData3["object"] != null)
							{
								JsonData data5 = jsonData3["object"];
								s3EventNotificationRecord.S3.Object = new S3ObjectEntity();
								s3EventNotificationRecord.S3.Object.Key = GetValueAsString(data5, "key");
								s3EventNotificationRecord.S3.Object.Size = GetValueAsLong(data5, "size");
								s3EventNotificationRecord.S3.Object.ETag = GetValueAsString(data5, "eTag");
								s3EventNotificationRecord.S3.Object.VersionId = GetValueAsString(data5, "versionId");
							}
						}
						s3EventNotification.Records.Add(s3EventNotificationRecord);
					}
				}
				return s3EventNotification;
			}
			catch (Exception ex)
			{
				throw new AmazonClientException("Failed to parse json string: " + ex.Message, ex);
			}
		}

		private static string GetValueAsString(JsonData data, string key)
		{
			if (data[key] != null)
			{
				return (string)data[key];
			}
			return null;
		}

		private static long GetValueAsLong(JsonData data, string key)
		{
			if (data[key] != null)
			{
				if (data[key].IsInt)
				{
					return (int)data[key];
				}
				return (long)data[key];
			}
			return 0L;
		}
	}
}
