using System;
using System.Collections.Generic;
using System.IO;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using UnityEngine;
using UnityEngine.UI;

namespace AWSSDK.Examples
{
	public class S3Example : MonoBehaviour
	{
		public string IdentityPoolId = string.Empty;

		public string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;

		public string S3Region = RegionEndpoint.USEast1.SystemName;

		public string S3BucketName;

		public string SampleFileName;

		public Button GetBucketListButton;

		public Button PostBucketButton;

		public Button GetObjectsListButton;

		public Button DeleteObjectButton;

		public Button GetObjectButton;

		public Text ResultText;

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

		private void Start()
		{
			UnityInitializer.AttachToGameObject(base.gameObject);
			GetBucketListButton.onClick.AddListener(delegate
			{
				GetBucketList();
			});
			PostBucketButton.onClick.AddListener(delegate
			{
				PostObject();
			});
			GetObjectsListButton.onClick.AddListener(delegate
			{
				GetObjects();
			});
			DeleteObjectButton.onClick.AddListener(delegate
			{
				DeleteObject();
			});
			GetObjectButton.onClick.AddListener(delegate
			{
				GetObject();
			});
		}

		public void GetBucketList()
		{
			ResultText.text = "Fetching all the Buckets";
			Client.ListBucketsAsync(new ListBucketsRequest(), delegate(AmazonServiceResult<ListBucketsRequest, ListBucketsResponse> responseObject)
			{
				ResultText.text += "\n";
				if (responseObject.Exception == null)
				{
					ResultText.text += "Got Response \nPrinting now \n";
					responseObject.Response.Buckets.ForEach(delegate(S3Bucket s3b)
					{
						ResultText.text += string.Format("bucket = {0}, created date = {1} \n", s3b.BucketName, s3b.CreationDate);
					});
				}
				else
				{
					ResultText.text += "Got Exception \n";
				}
			});
		}

		private void GetObject()
		{
			ResultText.text = string.Format("fetching {0} from bucket {1}", SampleFileName, S3BucketName);
			Client.GetObjectAsync(S3BucketName, SampleFileName, delegate(AmazonServiceResult<GetObjectRequest, GetObjectResponse> responseObj)
			{
				string text = null;
				GetObjectResponse response = responseObj.Response;
				if (response.ResponseStream != null)
				{
					using (StreamReader streamReader = new StreamReader(response.ResponseStream))
					{
						text = streamReader.ReadToEnd();
					}
					ResultText.text += "\n";
					ResultText.text += text;
				}
			});
		}

		public void PostObject()
		{
			ResultText.text = "Retrieving the file";
			string fileHelper = GetFileHelper();
			FileStream inputStream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + fileHelper, FileMode.Open, FileAccess.Read, FileShare.Read);
			ResultText.text += "\nCreating request object";
			PostObjectRequest postObjectRequest = new PostObjectRequest();
			postObjectRequest.Bucket = S3BucketName;
			postObjectRequest.Key = fileHelper;
			postObjectRequest.InputStream = inputStream;
			postObjectRequest.CannedACL = S3CannedACL.Private;
			PostObjectRequest request = postObjectRequest;
			ResultText.text += "\nMaking HTTP post call";
			Client.PostObjectAsync(request, delegate(AmazonServiceResult<PostObjectRequest, PostObjectResponse> responseObj)
			{
				if (responseObj.Exception == null)
				{
					ResultText.text += string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket);
				}
				else
				{
					ResultText.text += "\nException while posting the result object";
					ResultText.text += string.Format("\n receieved error {0}", responseObj.Response.HttpStatusCode.ToString());
				}
			});
		}

		public void GetObjects()
		{
			ResultText.text = "Fetching all the Objects from " + S3BucketName;
			ListObjectsRequest listObjectsRequest = new ListObjectsRequest();
			listObjectsRequest.BucketName = S3BucketName;
			ListObjectsRequest request = listObjectsRequest;
			Client.ListObjectsAsync(request, delegate(AmazonServiceResult<ListObjectsRequest, ListObjectsResponse> responseObject)
			{
				ResultText.text += "\n";
				if (responseObject.Exception == null)
				{
					ResultText.text += "Got Response \nPrinting now \n";
					responseObject.Response.S3Objects.ForEach(delegate(S3Object o)
					{
						ResultText.text += string.Format("{0}\n", o.Key);
					});
				}
				else
				{
					ResultText.text += "Got Exception \n";
				}
			});
		}

		public void DeleteObject()
		{
			ResultText.text = string.Format("deleting {0} from bucket {1}", SampleFileName, S3BucketName);
			List<KeyVersion> list = new List<KeyVersion>();
			list.Add(new KeyVersion
			{
				Key = SampleFileName
			});
			DeleteObjectsRequest deleteObjectsRequest = new DeleteObjectsRequest();
			deleteObjectsRequest.BucketName = S3BucketName;
			deleteObjectsRequest.Objects = list;
			DeleteObjectsRequest request = deleteObjectsRequest;
			Client.DeleteObjectsAsync(request, delegate(AmazonServiceResult<DeleteObjectsRequest, DeleteObjectsResponse> responseObj)
			{
				ResultText.text += "\n";
				if (responseObj.Exception == null)
				{
					ResultText.text += "Got Response \n \n";
					ResultText.text += string.Format("deleted objects \n");
					responseObj.Response.DeletedObjects.ForEach(delegate(DeletedObject dObj)
					{
						ResultText.text += dObj.Key;
					});
				}
				else
				{
					ResultText.text += "Got Exception \n";
				}
			});
		}

		private string GetFileHelper()
		{
			string sampleFileName = SampleFileName;
			if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + sampleFileName))
			{
				StreamWriter streamWriter = File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + sampleFileName);
				streamWriter.WriteLine("This is a sample s3 file uploaded from unity s3 sample");
				streamWriter.Close();
			}
			return sampleFileName;
		}

		private string GetPostPolicy(string bucketName, string key, string contentType)
		{
			bucketName = bucketName.Trim();
			key = key.Trim();
			if (!string.IsNullOrEmpty(key) && key[0] == '/')
			{
				throw new ArgumentException("uploadFileName cannot start with / ");
			}
			contentType = contentType.Trim();
			if (string.IsNullOrEmpty(bucketName))
			{
				throw new ArgumentException("bucketName cannot be null or empty. It's required to build post policy");
			}
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("uploadFileName cannot be null or empty. It's required to build post policy");
			}
			if (string.IsNullOrEmpty(contentType))
			{
				throw new ArgumentException("contentType cannot be null or empty. It's required to build post policy");
			}
			string text = null;
			int num = key.LastIndexOf('/');
			if (num == -1)
			{
				return "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24.0).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" + bucketName + "\"},[\"starts-with\", \"$key\", \"\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", \"" + contentType + "\"]]}";
			}
			return "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24.0).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" + bucketName + "\"},[\"starts-with\", \"$key\", \"" + key.Substring(0, num) + "/\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", \"" + contentType + "\"]]}";
		}
	}
}
