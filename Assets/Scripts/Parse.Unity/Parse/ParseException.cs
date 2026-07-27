using System;

namespace Parse
{
	public class ParseException : Exception
	{
		public enum ErrorCode
		{
			OtherCause = -1,
			InternalServerError = 1,
			ConnectionFailed = 100,
			ObjectNotFound = 101,
			InvalidQuery = 102,
			InvalidClassName = 103,
			MissingObjectId = 104,
			InvalidKeyName = 105,
			InvalidPointer = 106,
			InvalidJSON = 107,
			CommandUnavailable = 108,
			NotInitialized = 109,
			IncorrectType = 111,
			InvalidChannelName = 112,
			PushMisconfigured = 115,
			ObjectTooLarge = 116,
			OperationForbidden = 119,
			CacheMiss = 120,
			InvalidNestedKey = 121,
			InvalidFileName = 122,
			InvalidACL = 123,
			Timeout = 124,
			InvalidEmailAddress = 125,
			DuplicateValue = 137,
			InvalidRoleName = 139,
			ExceededQuota = 140,
			ScriptFailed = 141,
			ValidationFailed = 142,
			FileDeleteFailed = 153,
			RequestLimitExceeded = 155,
			InvalidEventName = 160,
			UsernameMissing = 200,
			PasswordMissing = 201,
			UsernameTaken = 202,
			EmailTaken = 203,
			EmailMissing = 204,
			EmailNotFound = 205,
			SessionMissing = 206,
			MustCreateUserThroughSignup = 207,
			AccountAlreadyLinked = 208,
			InvalidSessionToken = 209,
			LinkedIdMissing = 250,
			InvalidLinkedSession = 251,
			UnsupportedService = 252
		}

		public ErrorCode Code { get; private set; }

		internal ParseException(ErrorCode code, string message, Exception cause = null)
			: base(message, cause)
		{
			Code = code;
		}
	}
}
