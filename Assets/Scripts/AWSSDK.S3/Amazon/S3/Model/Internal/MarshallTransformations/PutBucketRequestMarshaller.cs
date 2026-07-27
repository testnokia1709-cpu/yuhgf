using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutBucketRequestMarshaller : IMarshaller<IRequest, PutBucketRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketRequestMarshaller _instance;

		public static PutBucketRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketRequest)input);
		}

		public IRequest Marshall(PutBucketRequest putBucketRequest)
		{
			IRequest request = new DefaultRequest(putBucketRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			if (putBucketRequest.IsSetCannedACL())
			{
				request.Headers.Add("x-amz-acl", putBucketRequest.CannedACL.Value);
			}
			else if (putBucketRequest.Grants != null && putBucketRequest.Grants.Count > 0)
			{
				ConvertPutWithACLRequest(putBucketRequest, request);
			}
			string resourcePath = "/" + S3Transforms.ToStringValue(putBucketRequest.BucketName);
			request.ResourcePath = resourcePath;
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				string text = null;
				S3Region bucketRegion = putBucketRequest.BucketRegion;
				if (bucketRegion != null && !string.IsNullOrEmpty(bucketRegion.Value))
				{
					text = bucketRegion.Value;
				}
				else if (!string.IsNullOrEmpty(putBucketRequest.BucketRegionName))
				{
					if (putBucketRequest.BucketRegionName == "eu-west-1")
					{
						text = "EU";
					}
					else if (putBucketRequest.BucketRegionName != "us-east-1")
					{
						text = putBucketRequest.BucketRegionName;
					}
				}
				if (text != null)
				{
					xmlWriter.WriteStartElement("CreateBucketConfiguration", "");
					xmlWriter.WriteElementString("LocationConstraint", "", text);
					xmlWriter.WriteEndElement();
				}
			}
			try
			{
				string text2 = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(text2);
				request.Headers["Content-Type"] = "application/xml";
				string value = AmazonS3Util.GenerateChecksumForContent(text2, true);
				request.Headers["Content-MD5"] = value;
				return request;
			}
			catch (EncoderFallbackException innerException)
			{
				throw new AmazonServiceException("Unable to marshall request to XML", innerException);
			}
		}

		protected internal static void ConvertPutWithACLRequest(PutWithACLRequest request, IRequest irequest)
		{
			Dictionary<S3Permission, string> dictionary = new Dictionary<S3Permission, string>();
			foreach (S3Grant grant in request.Grants)
			{
				string text = null;
				if (grant.Grantee.CanonicalUser != null && !string.IsNullOrEmpty(grant.Grantee.CanonicalUser))
				{
					text = string.Format(CultureInfo.InvariantCulture, "id=\"{0}\"", grant.Grantee.CanonicalUser);
				}
				else if (grant.Grantee.IsSetEmailAddress())
				{
					text = string.Format(CultureInfo.InvariantCulture, "emailAddress=\"{0}\"", grant.Grantee.EmailAddress);
				}
				else
				{
					if (!grant.Grantee.IsSetURI())
					{
						continue;
					}
					text = string.Format(CultureInfo.InvariantCulture, "uri=\"{0}\"", grant.Grantee.URI);
				}
				string value = null;
				if (dictionary.TryGetValue(grant.Permission, out value))
				{
					dictionary[grant.Permission] = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", value, text);
				}
				else
				{
					dictionary.Add(grant.Permission, text);
				}
			}
			foreach (S3Permission key in dictionary.Keys)
			{
				if (key == S3Permission.READ)
				{
					irequest.Headers["x-amz-grant-read"] = dictionary[key];
				}
				if (key == S3Permission.WRITE)
				{
					irequest.Headers["x-amz-grant-write"] = dictionary[key];
				}
				if (key == S3Permission.READ_ACP)
				{
					irequest.Headers["x-amz-grant-read-acp"] = dictionary[key];
				}
				if (key == S3Permission.WRITE_ACP)
				{
					irequest.Headers["x-amz-grant-write-acp"] = dictionary[key];
				}
				if (key == S3Permission.RESTORE_OBJECT)
				{
					irequest.Headers["x-amz-grant-restore-object"] = dictionary[key];
				}
				if (key == S3Permission.FULL_CONTROL)
				{
					irequest.Headers["x-amz-grant-full-control"] = dictionary[key];
				}
			}
		}
	}
}
