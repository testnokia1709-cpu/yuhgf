using System;
using System.IO;
using System.Reflection;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using UnityEngine;

public class S3Config : MonoBehaviour
{
	public static S3Config Instance;

	public Action<bool> OnReceviedConfig;

	public string IdentityPoolId = string.Empty;

	public string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;

	public string S3Region = RegionEndpoint.USEast1.SystemName;

	public string S3BucketName;

	public string S3FileName;

	public bool Testing;

	public string S3TestFileName;

	private IAmazonS3 _s3Client;

	private AWSCredentials _credentials;

	private RegionEndpoint _CognitoIdentityRegion
	{
		get
		{
			return RegionEndpoint.GetBySystemName(CognitoIdentityRegion);
		}
	}

	private RegionEndpoint _S3Region
	{
		get
		{
			return RegionEndpoint.GetBySystemName(S3Region);
		}
	}

	private AWSCredentials Credentials
	{
		get
		{
			if (_credentials == null)
			{
				_credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
			}
			return _credentials;
		}
	}

	private IAmazonS3 Client
	{
		get
		{
			if (_s3Client == null)
			{
				_s3Client = new AmazonS3Client(Credentials, _S3Region);
			}
			return _s3Client;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		UnityInitializer.AttachToGameObject(base.gameObject);
		AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
		getObject(delegate(string result)
		{
			try
			{
				Debug.Log("S3Config: " + result);
				ConfigSettings configSettings = JsonUtility.FromJson<ConfigSettings>(result);
				bool flag = false;
				MemberInfo[] members = configSettings.GetType().GetMembers();
				foreach (MemberInfo memberInfo in members)
				{
					if (memberInfo.MemberType == MemberTypes.Field)
					{
						FieldInfo fieldInfo = (FieldInfo)memberInfo;
						object value = fieldInfo.GetValue(configSettings);
						if (result.Contains("\"" + fieldInfo.Name + "\""))
						{
							fieldInfo.SetValue(DataStore.Instance.ConfigSettings, value);
							flag = true;
						}
					}
				}
				if (flag)
				{
					DataStore.Save();
				}
				if (OnReceviedConfig != null)
				{
					OnReceviedConfig(flag);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Error parsing S3Config result: " + ex);
			}
		});
	}

	private void getObject(Action<string> callback)
	{
		string text = ((!Testing) ? S3FileName : S3TestFileName);
		Debug.Log(string.Format("Fetching {0} from bucket {1}", text, S3BucketName));
		Client.GetObjectAsync(S3BucketName, text, delegate(AmazonServiceResult<GetObjectRequest, GetObjectResponse> responseObj)
		{
			string obj = null;
			GetObjectResponse response = responseObj.Response;
			if (response.ResponseStream != null)
			{
				using (StreamReader streamReader = new StreamReader(response.ResponseStream))
				{
					obj = streamReader.ReadToEnd();
				}
				callback(obj);
			}
		});
	}
}
