using System.Xml;
using Amazon.Runtime;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public class RestoreObjectRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string key;

		private int? days;

		private string versionId;

		private RequestPayer requestPayer;

		private GlacierJobTier tier;

		private GlacierJobTier retrievalTier;

		private RestoreRequestType type;

		private string description;

		private SelectParameters selectParameters;

		private OutputLocation outputLocation;

		public string BucketName
		{
			get
			{
				return bucketName;
			}
			set
			{
				bucketName = value;
			}
		}

		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public int Days
		{
			get
			{
				return days ?? 0;
			}
			set
			{
				days = value;
			}
		}

		public string VersionId
		{
			get
			{
				return versionId;
			}
			set
			{
				versionId = value;
			}
		}

		public RequestPayer RequestPayer
		{
			get
			{
				return requestPayer;
			}
			set
			{
				requestPayer = value;
			}
		}

		public GlacierJobTier Tier
		{
			get
			{
				return tier;
			}
			set
			{
				tier = value;
			}
		}

		public GlacierJobTier RetrievalTier
		{
			get
			{
				return retrievalTier;
			}
			set
			{
				retrievalTier = value;
			}
		}

		public RestoreRequestType RestoreRequestType
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public SelectParameters SelectParameters
		{
			get
			{
				return selectParameters;
			}
			set
			{
				selectParameters = value;
			}
		}

		public OutputLocation OutputLocation
		{
			get
			{
				return outputLocation;
			}
			set
			{
				outputLocation = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetDays()
		{
			return days.HasValue;
		}

		internal bool IsSetVersionId()
		{
			return versionId != null;
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		internal bool IsSetTier()
		{
			return tier != null;
		}

		internal bool IsSetRetrievalTier()
		{
			return retrievalTier != null;
		}

		internal bool IsSetType()
		{
			return type != null;
		}

		internal bool IsSetDescription()
		{
			return description != null;
		}

		internal bool IsSetSelectParameters()
		{
			return selectParameters != null;
		}

		internal bool IsSetOutputLocation()
		{
			return outputLocation != null;
		}

		internal void Marshall(string propertyName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(propertyName);
			if (IsSetRetrievalTier())
			{
				xmlWriter.WriteElementString("Tier", S3Transforms.ToXmlStringValue(RetrievalTier));
			}
			if (IsSetTier())
			{
				xmlWriter.WriteStartElement("GlacierJobParameters");
				xmlWriter.WriteElementString("Tier", S3Transforms.ToXmlStringValue(Tier));
				xmlWriter.WriteEndElement();
			}
			if (IsSetDays())
			{
				xmlWriter.WriteElementString("Days", S3Transforms.ToXmlStringValue(Days));
			}
			if (IsSetType())
			{
				xmlWriter.WriteElementString("Type", S3Transforms.ToXmlStringValue(RestoreRequestType.Value));
			}
			if (IsSetDescription())
			{
				xmlWriter.WriteElementString("Description", S3Transforms.ToXmlStringValue(Description));
			}
			if (IsSetSelectParameters())
			{
				SelectParameters.Marshall("SelectParameters", xmlWriter);
			}
			if (IsSetOutputLocation())
			{
				OutputLocation.Marshall("OutputLocation", xmlWriter);
			}
			xmlWriter.WriteEndElement();
		}
	}
}
