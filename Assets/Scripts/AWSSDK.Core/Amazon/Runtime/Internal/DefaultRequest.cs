using System;
using System.Collections.Generic;
using System.IO;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;

namespace Amazon.Runtime.Internal
{
	public class DefaultRequest : IRequest
	{
		private readonly ParameterCollection parametersCollection;

		private readonly IDictionary<string, string> parametersFacade;

		private readonly IDictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private readonly IDictionary<string, string> subResources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private Uri endpoint;

		private string resourcePath;

		private string serviceName;

		private readonly AmazonWebServiceRequest originalRequest;

		private byte[] content;

		private Stream contentStream;

		private string contentStreamHash;

		private string httpMethod = "POST";

		private bool useQueryString;

		private string requestName;

		private string canonicalResource;

		private RegionEndpoint alternateRegion;

		private long originalStreamLength;

		public string RequestName
		{
			get
			{
				return requestName;
			}
		}

		public string HttpMethod
		{
			get
			{
				return httpMethod;
			}
			set
			{
				httpMethod = value;
			}
		}

		public bool UseQueryString
		{
			get
			{
				if (HttpMethod == "GET")
				{
					return true;
				}
				return useQueryString;
			}
			set
			{
				useQueryString = value;
			}
		}

		public AmazonWebServiceRequest OriginalRequest
		{
			get
			{
				return originalRequest;
			}
		}

		public IDictionary<string, string> Headers
		{
			get
			{
				return headers;
			}
		}

		public IDictionary<string, string> Parameters
		{
			get
			{
				return parametersFacade;
			}
		}

		public ParameterCollection ParameterCollection
		{
			get
			{
				return parametersCollection;
			}
		}

		public IDictionary<string, string> SubResources
		{
			get
			{
				return subResources;
			}
		}

		public Uri Endpoint
		{
			get
			{
				return endpoint;
			}
			set
			{
				endpoint = value;
			}
		}

		public string ResourcePath
		{
			get
			{
				return resourcePath;
			}
			set
			{
				resourcePath = value;
			}
		}

		public string CanonicalResource
		{
			get
			{
				return canonicalResource;
			}
			set
			{
				canonicalResource = value;
			}
		}

		public byte[] Content
		{
			get
			{
				return content;
			}
			set
			{
				content = value;
			}
		}

		public bool SetContentFromParameters { get; set; }

		public Stream ContentStream
		{
			get
			{
				return contentStream;
			}
			set
			{
				contentStream = value;
				OriginalStreamPosition = -1L;
				if (contentStream != null)
				{
					Stream nonWrapperBaseStream = WrapperStream.GetNonWrapperBaseStream(contentStream);
					if (nonWrapperBaseStream.CanSeek)
					{
						OriginalStreamPosition = nonWrapperBaseStream.Position;
					}
				}
			}
		}

		public long OriginalStreamPosition
		{
			get
			{
				return originalStreamLength;
			}
			set
			{
				originalStreamLength = value;
			}
		}

		public string ServiceName
		{
			get
			{
				return serviceName;
			}
		}

		public RegionEndpoint AlternateEndpoint
		{
			get
			{
				return alternateRegion;
			}
			set
			{
				alternateRegion = value;
			}
		}

		public bool Suppress404Exceptions { get; set; }

		public AWS4SigningResult AWS4SignerResult { get; set; }

		public bool UseChunkEncoding { get; set; }

		public string CanonicalResourcePrefix { get; set; }

		public bool UseSigV4 { get; set; }

		public string AuthenticationRegion { get; set; }

		public DefaultRequest(AmazonWebServiceRequest request, string serviceName)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (string.IsNullOrEmpty(serviceName))
			{
				throw new ArgumentNullException("serviceName");
			}
			this.serviceName = serviceName;
			originalRequest = request;
			requestName = originalRequest.GetType().Name;
			UseSigV4 = ((IAmazonWebServiceRequest)originalRequest).UseSigV4;
			parametersCollection = new ParameterCollection();
			parametersFacade = new ParametersDictionaryFacade(parametersCollection);
		}

		public void AddSubResource(string subResource)
		{
			AddSubResource(subResource, null);
		}

		public void AddSubResource(string subResource, string value)
		{
			SubResources.Add(subResource, value);
		}

		public string ComputeContentStreamHash()
		{
			if (contentStream == null)
			{
				return null;
			}
			if (contentStreamHash == null)
			{
				Stream stream = WrapperStream.SearchWrappedStream(contentStream, (Stream s) => s.CanSeek);
				if (stream != null)
				{
					long position = stream.Position;
					byte[] data = CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(stream);
					contentStreamHash = AWSSDKUtils.ToHex(data, true);
					stream.Seek(position, SeekOrigin.Begin);
				}
			}
			return contentStreamHash;
		}

		public bool IsRequestStreamRewindable()
		{
			Stream stream = ContentStream;
			if (stream != null)
			{
				stream = WrapperStream.GetNonWrapperBaseStream(stream);
				return stream.CanSeek;
			}
			return true;
		}

		public bool MayContainRequestBody()
		{
			if (!(HttpMethod == "POST") && !(HttpMethod == "PUT"))
			{
				return HttpMethod == "PATCH";
			}
			return true;
		}

		public bool HasRequestBody()
		{
			bool num = HttpMethod == "POST" || HttpMethod == "PUT" || HttpMethod == "PATCH";
			bool flag = this.HasRequestData();
			return num && flag;
		}
	}
}
