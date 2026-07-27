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
	public class DeleteObjectsRequestMarshaller : IMarshaller<IRequest, DeleteObjectsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteObjectsRequestMarshaller _instance;

		public static DeleteObjectsRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteObjectsRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteObjectsRequest)input);
		}

		public IRequest Marshall(DeleteObjectsRequest deleteObjectsRequest)
		{
			IRequest request = new DefaultRequest(deleteObjectsRequest, "AmazonS3");
			request.HttpMethod = "POST";
			if (deleteObjectsRequest.IsSetMfaCodes())
			{
				request.Headers.Add("x-amz-mfa", deleteObjectsRequest.MfaCodes.FormattedMfaCodes);
			}
			if (deleteObjectsRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(deleteObjectsRequest.RequestPayer.ToString()));
			}
			request.ResourcePath = "/" + S3Transforms.ToStringValue(deleteObjectsRequest.BucketName);
			request.AddSubResource("delete");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("Delete", "");
				List<KeyVersion> objects = deleteObjectsRequest.Objects;
				if (objects != null && objects.Count > 0)
				{
					foreach (KeyVersion item in objects)
					{
						xmlWriter.WriteStartElement("Object", "");
						if (item.IsSetKey())
						{
							xmlWriter.WriteElementString("Key", "", S3Transforms.ToXmlStringValue(item.Key));
						}
						if (item.IsSetVersionId())
						{
							xmlWriter.WriteElementString("VersionId", "", S3Transforms.ToXmlStringValue(item.VersionId));
						}
						xmlWriter.WriteEndElement();
					}
				}
				if (deleteObjectsRequest.IsSetQuiet())
				{
					xmlWriter.WriteElementString("Quiet", "", deleteObjectsRequest.Quiet.ToString().ToLowerInvariant());
				}
				xmlWriter.WriteEndElement();
			}
			try
			{
				string text = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(text);
				request.Headers["Content-Type"] = "application/xml";
				string value = AmazonS3Util.GenerateChecksumForContent(text, true);
				request.Headers["Content-MD5"] = value;
				return request;
			}
			catch (EncoderFallbackException innerException)
			{
				throw new AmazonServiceException("Unable to marshall request to XML", innerException);
			}
		}
	}
}
