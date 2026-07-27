namespace Amazon.Runtime.SharedInterfaces
{
	public interface ICoreAmazonSTS
	{
		AssumeRoleImmutableCredentials CredentialsFromAssumeRoleAuthentication(string roleArn, string roleSessionName, AssumeRoleAWSCredentialsOptions options);
	}
}
