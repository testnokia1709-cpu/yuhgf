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
	public class RestoreObjectRequestMarshaller : IMarshaller<IRequest, RestoreObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static RestoreObjectRequestMarshaller _instance;

		public static RestoreObjectRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RestoreObjectRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((RestoreObjectRequest)input);
		}

		public IRequest Marshall(RestoreObjectRequest restoreObjectRequest)
		{
			IRequest request = new DefaultRequest(restoreObjectRequest, "AmazonS3");
			request.HttpMethod = "POST";
			if (restoreObjectRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(restoreObjectRequest.RequestPayer.ToString()));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(restoreObjectRequest.BucketName), S3Transforms.ToStringValue(restoreObjectRequest.Key));
			request.AddSubResource("restore");
			if (restoreObjectRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", S3Transforms.ToStringValue(restoreObjectRequest.VersionId));
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				restoreObjectRequest.Marshall("RestoreRequest", xmlWriter);
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
