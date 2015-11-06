using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X API
	/// https://m2x.att.com/developer/documentation/v2/overview
	/// </summary>
	public sealed class M2XClient : IDisposable
	{
		public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
		public const string ApiEndPointSecure = "https://api-m2x.att.com/v2";
		public const string ApiEndPoint = "http://api-m2x.att.com/v2";

		private static readonly string UserAgent;

		public readonly string APIKey;
		public readonly string EndPoint;

		private HttpClient client = new HttpClient();

		private CancellationToken cancellationToken = CancellationToken.None;
		/// <summary>
		/// Gets or sets the cancellation token used in all requests
		/// </summary>
		public CancellationToken CancellationToken
		{
			get { return this.cancellationToken; }
			set { this.cancellationToken = value; }
		}

		private volatile M2XResponse lastResponse;
		/// <summary>
		/// The last API call response
		/// </summary>
		public M2XResponse LastResponse { get { return this.lastResponse; } }

		static M2XClient()
		{
			string version = typeof(M2XClient).GetTypeInfo().Assembly.GetName().Version.ToString();
			string langVersion = "4.5";//Environment.Version.ToString();
			string osVersion = "unknown";//Environment.OSVersion.ToString();
			UserAgent = String.Format("M2X-.NET/{0} .NETFramework/{1} ({2})", version, langVersion, osVersion);
		}

		public M2XClient(string apiKey, string m2xApiEndPoint = ApiEndPoint)
		{
			if (String.IsNullOrWhiteSpace(m2xApiEndPoint))
				throw new ArgumentException("Invalid API end point url");

			this.APIKey = apiKey;
			this.EndPoint = m2xApiEndPoint;

			client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
			if (!String.IsNullOrEmpty(this.APIKey))
				client.DefaultRequestHeaders.Add("X-M2X-KEY", this.APIKey);
		}

		public void Dispose()
		{
			if (this.client != null)
			{
				this.client.Dispose();
				this.client = null;
			}
		}

		// Device API

		/// <summary>
		/// List the catalog of public Devices.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Public-Devices-Catalog
		/// </summary>
		public Task<M2XResponse> DeviceCatalog(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath + "/catalog", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Search the catalog of public Devices.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Search-Public-Devices-Catalog
		/// </summary>
		public Task<M2XResponse> DeviceCatalogSearch(object parms = null, object bodyParms = null)
		{
			return MakeRequest(M2XDevice.UrlPath + "/catalog/search", bodyParms == null ? M2XClientMethod.GET : M2XClientMethod.POST, parms, bodyParms);
		}

		/// <summary>
		/// Retrieve the list of devices accessible by the authenticated API key.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Devices
		/// </summary>
		public Task<M2XResponse> Devices(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Retrieve the list of devices accessible by the authenticated API key that meet the search criteria.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Search-Devices
		/// </summary>
		public Task<M2XResponse> SearchDevices(object parms = null, object bodyParms = null)
		{
			return MakeRequest(M2XDevice.UrlPath + "/search", bodyParms == null ? M2XClientMethod.GET : M2XClientMethod.POST, parms, bodyParms);
		}

		/// <summary>
		/// List Device Tags
		/// Retrieve the list of device tags for the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/device#List-Device-Tags
		/// </summary>
		public Task<M2XResponse> DeviceTags(object parms = null)
		{
			return MakeRequest(M2XDevice.UrlPath + "/tags", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new device
		///
		/// https://m2x.att.com/developer/documentation/v2/device#Create-Device
		/// </summary>
		public Task<M2XResponse> CreateDevice(object parms)
		{
			return MakeRequest(M2XDevice.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing Device.
		/// </summary>
		public M2XDevice Device(string deviceId, string serial = null)
		{
			return new M2XDevice(this, deviceId, serial);
		}

		// Distribution API

		/// <summary>
		/// Retrieve list of device distributions accessible by the authenticated API key.
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#List-Distributions
		/// </summary>
		public Task<M2XResponse> Distributions(object parms = null)
		{
			return MakeRequest(M2XDistribution.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new device distribution
		///
		/// https://m2x.att.com/developer/documentation/v2/distribution#Create-Distribution
		/// </summary>
		public Task<M2XResponse> CreateDistribution(object parms)
		{
			return MakeRequest(M2XDistribution.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing device distribution.
		/// </summary>
		public M2XDistribution Distribution(string distributionId)
		{
			return new M2XDistribution(this, distributionId);
		}

		// Keys API

		/// <summary>
		/// Retrieve list of keys associated with the specified account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#List-Keys
		/// </summary>
		public Task<M2XResponse> Keys(object parms = null)
		{
			return MakeRequest(M2XKey.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new key associated with the specified account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#Create-Key
		/// </summary>
		public Task<M2XResponse> CreateKey(object parms)
		{
			return MakeRequest(M2XKey.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing key associated with the specified account.
		/// </summary>
		public M2XKey Key(string key)
		{
			return new M2XKey(this, key);
		}

		// Collections API

		/// <summary>
		/// Retrieve a list of collections accessible by the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/collections#List-collections
		/// </summary>
		public Task<M2XResponse> Collections(object parms = null)
		{
			return MakeRequest(M2XCollection.UrlPath, M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Create a new collection.
		/// 
		/// https://m2x.att.com/developer/documentation/v2/collections#Create-Collection
		/// </summary>
		public Task<M2XResponse> CreateCollection(object parms)
		{
			return MakeRequest(M2XCollection.UrlPath, M2XClientMethod.POST, parms);
		}

		/// <summary>
		/// Get a wrapper to access an existing Collection.
		/// </summary>
		public M2XCollection Collection(string collectionId)
		{
			return new M2XCollection(this, collectionId);
		}

		// Jobs API

		/// <summary>
		/// Retrieve the list of the most recent jobs that belong to the authenticated user.
		///
		/// https://m2x.att.com/developer/documentation/v2/jobs#List-Jobs
		/// </summary>
		public Task<M2XResponse> Jobs(object parms = null)
		{
			return MakeRequest("/jobs", M2XClientMethod.GET, parms);
		}

		/// <summary>
		/// Retrieve the job details.
		///
		/// https://m2x.att.com/developer/documentation/v2/jobs#View-Job-Details
		/// https://m2x.att.com/developer/documentation/v2/jobs#View-Job-Results
		/// </summary>
		public Task<M2XResponse> JobDetails(string jobId)
		{
			return MakeRequest("/jobs/" + WebUtility.UrlEncode(jobId), M2XClientMethod.GET);
		}

		// Time API

		/// <summary>
		/// Returns M2X servers' time.
		///
		/// https://m2x.att.com/developer/documentation/v2/time
		/// </summary>
		public Task<M2XResponse> Time(string format = null)
		{
			string path = "/time";
			if (!String.IsNullOrEmpty(format))
				path += "/" + WebUtility.UrlEncode(format);
			return MakeRequest(path);
		}

		// Common

		/// <summary>
		/// Formats a DateTime value to an ISO8601 timestamp.
		/// </summary>
		public static string DateTimeToString(DateTime dateTime)
		{
			return dateTime.ToString(DateTimeFormat);
		}

		/// <summary>
		/// Builds url to AT&T M2X API with optional query parameters
		/// </summary>
		/// <param name="path">AT&T M2X API url path</param>
		/// <param name="parms">parameters to build url query</param>
		public string BuildUrl(string path, object parms = null)
		{
			string fullUrl = this.EndPoint + path;
			if (parms != null)
			{
				string queryString = ObjectToQueryString(parms);
				if (!String.IsNullOrEmpty(queryString))
					fullUrl += "?" + queryString;
			}
			return fullUrl;
		}

		/// <summary>
		/// Performs async M2X API request
		/// </summary>
		/// <param name="path">API path</param>
		/// <param name="method">HTTP method</param>
		/// <param name="parms">Query string (for GET and DELETE) or body (for POST and PUT) parameters</param>
		/// <param name="addBodyParms">Additional body parameters, if specified, the parms will be treated as query parameters.
		/// The passed object will be serialized into a JSON string. In case of a string passed it will be treated as a valid JSON string.</param>
		/// <returns>The request and response data from M2X server</returns>
		public async Task<M2XResponse> MakeRequest(
			string path, M2XClientMethod method = M2XClientMethod.GET, object parms = null, object addBodyParms = null)
		{
			M2XResponse result = CreateResponse(path, method, parms, addBodyParms);
			CancellationToken ct = this.cancellationToken;
			try
			{
				HttpResponseMessage responseMessage;
				switch (method)
				{
					case M2XClientMethod.POST:
						responseMessage = ct == CancellationToken.None
							? await this.client.PostAsync(result.RequestUri, result.GetContent())
							: await this.client.PostAsync(result.RequestUri, result.GetContent(), ct);
						break;
					case M2XClientMethod.PUT:
						responseMessage = ct == CancellationToken.None
							? await this.client.PutAsync(result.RequestUri, result.GetContent())
							: await this.client.PutAsync(result.RequestUri, result.GetContent(), ct);
						break;
					case M2XClientMethod.DELETE:
						responseMessage = ct == CancellationToken.None
							? await this.client.DeleteAsync(result.RequestUri)
							: await this.client.DeleteAsync(result.RequestUri, ct);
						break;
					default:
						responseMessage = ct == CancellationToken.None
							? await this.client.GetAsync(result.RequestUri)
							: await this.client.GetAsync(result.RequestUri, ct);
						break;
				}
				if (ct != CancellationToken.None)
					ct.ThrowIfCancellationRequested();
				await result.SetResponse(responseMessage);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex)
			{
				result.WebError = ex;
			}
			this.lastResponse = result;
			return result;
		}

		private M2XResponse CreateResponse(string path, M2XClientMethod method, object parms, object addBodyParms)
		{
			bool isGetOrDelete = method == M2XClientMethod.GET || method == M2XClientMethod.DELETE;
			string url = BuildUrl(path, isGetOrDelete || addBodyParms != null ? parms : null);
			string content = isGetOrDelete ? null : SerializeData(addBodyParms ?? parms);
			return new M2XResponse(new Uri(url), method, content);
		}

		public static string ObjectToQueryString(object queryParams)
		{
			StringBuilder sb = new StringBuilder();
			IEnumerable<FieldInfo> fields = queryParams.GetType().GetFields();
			foreach (var prop in fields)
			{
				if (prop.IsStatic || !prop.IsPublic || prop.FieldType.IsArray)
					continue;
				object value = prop.GetValue(queryParams);
				AppendQuery(sb, prop.Name, value);
			}
			IEnumerable<PropertyInfo> props = queryParams.GetType().GetProperties();
			foreach (var prop in props)
			{
				if (!prop.CanRead || prop.PropertyType.IsArray)
					continue;
				object value = prop.GetValue(queryParams, null);
				AppendQuery(sb, prop.Name, value);
			}
			return sb.ToString();
		}
		private static void AppendQuery(StringBuilder sb, string name, object value)
		{
			if (value == null)
				return;
			if (sb.Length > 0)
				sb.Append('&');
			sb.Append(name).Append('=').Append(WebUtility.UrlEncode(value.ToString()));
		}

		public static string SerializeData(object data)
		{
			if (data == null)
				return null;
			string result = data as string;
			if (result != null)
				return result;

			var serializer = new DataContractJsonSerializer(data.GetType());
			using (var stream = new MemoryStream())
			{
				serializer.WriteObject(stream, data);
				stream.Position = 0;
				return new StreamReader(stream).ReadToEnd();
			}
		}
	}
}
