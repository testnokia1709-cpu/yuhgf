using System;
using System.Globalization;

namespace Amazon.Auth.AccessControlPolicy
{
	public static class ConditionFactory
	{
		public enum ArnComparisonType
		{
			ArnEquals = 0,
			ArnLike = 1,
			ArnNotEquals = 2,
			ArnNotLike = 3
		}

		public enum DateComparisonType
		{
			DateEquals = 0,
			DateGreaterThan = 1,
			DateGreaterThanEquals = 2,
			DateLessThan = 3,
			DateLessThanEquals = 4,
			DateNotEquals = 5
		}

		public enum IpAddressComparisonType
		{
			IpAddress = 0,
			NotIpAddress = 1
		}

		public enum NumericComparisonType
		{
			NumericEquals = 0,
			NumericGreaterThan = 1,
			NumericGreaterThanEquals = 2,
			NumericLessThan = 3,
			NumericLessThanEquals = 4,
			NumericNotEquals = 5
		}

		public enum StringComparisonType
		{
			StringEquals = 0,
			StringEqualsIgnoreCase = 1,
			StringLike = 2,
			StringNotEquals = 3,
			StringNotEqualsIgnoreCase = 4,
			StringNotLike = 5
		}

		public const string CURRENT_TIME_CONDITION_KEY = "aws:CurrentTime";

		public const string SECURE_TRANSPORT_CONDITION_KEY = "aws:SecureTransport";

		public const string SOURCE_IP_CONDITION_KEY = "aws:SourceIp";

		public const string USER_AGENT_CONDITION_KEY = "aws:UserAgent";

		public const string EPOCH_TIME_CONDITION_KEY = "aws:EpochTime";

		public const string REFERRER_CONDITION_KEY = "aws:Referer";

		public const string SOURCE_ARN_CONDITION_KEY = "aws:SourceArn";

		public const string S3_CANNED_ACL_CONDITION_KEY = "s3:x-amz-acl";

		public const string S3_LOCATION_CONSTRAINT_CONDITION_KEY = "s3:LocationConstraint";

		public const string S3_PREFIX_CONDITION_KEY = "s3:prefix";

		public const string S3_DELIMITER_CONDITION_KEY = "s3:delimiter";

		public const string S3_MAX_KEYS_CONDITION_KEY = "s3:max-keys";

		public const string S3_COPY_SOURCE_CONDITION_KEY = "s3:x-amz-copy-source";

		public const string S3_METADATA_DIRECTIVE_CONDITION_KEY = "s3:x-amz-metadata-directive";

		public const string S3_VERSION_ID_CONDITION_KEY = "s3:VersionId";

		public const string SNS_ENDPOINT_CONDITION_KEY = "sns:Endpoint";

		public const string SNS_PROTOCOL_CONDITION_KEY = "sns:Protocol";

		public static Condition NewCondition(ArnComparisonType type, string key, string value)
		{
			return new Condition(type.ToString(), key, value);
		}

		public static Condition NewCondition(string key, bool value)
		{
			return new Condition("Bool", key, value.ToString().ToLowerInvariant());
		}

		public static Condition NewCondition(DateComparisonType type, DateTime date)
		{
			return new Condition(type.ToString(), "aws:CurrentTime", date.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture));
		}

		public static Condition NewIpAddressCondition(string ipAddressRange)
		{
			return NewCondition(IpAddressComparisonType.IpAddress, ipAddressRange);
		}

		public static Condition NewCondition(IpAddressComparisonType type, string ipAddressRange)
		{
			return new Condition(type.ToString(), "aws:SourceIp", ipAddressRange);
		}

		public static Condition NewCondition(NumericComparisonType type, string key, string value)
		{
			return new Condition(type.ToString(), key, value);
		}

		public static Condition NewCondition(StringComparisonType type, string key, string value)
		{
			return new Condition(type.ToString(), key, value);
		}

		public static Condition NewSourceArnCondition(string arnPattern)
		{
			return NewCondition(ArnComparisonType.ArnLike, "aws:SourceArn", arnPattern);
		}

		public static Condition NewSecureTransportCondition()
		{
			return NewCondition("aws:SecureTransport", true);
		}

		public static Condition NewCannedACLCondition(string cannedAcl)
		{
			return NewCondition(StringComparisonType.StringEquals, "s3:x-amz-acl", cannedAcl);
		}

		public static Condition NewEndpointCondition(string endpointPattern)
		{
			return NewCondition(StringComparisonType.StringLike, "sns:Endpoint", endpointPattern);
		}

		public static Condition NewProtocolCondition(string protocol)
		{
			return NewCondition(StringComparisonType.StringEquals, "sns:Protocol", protocol);
		}
	}
}
