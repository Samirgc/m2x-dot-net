using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace ATTM2X
{
	public enum M2XClientMethod
	{
		GET,
		POST,
		PUT,
		DELETE,
	}

	public class M2XAPIException : Exception
	{
		private readonly HttpStatusCode code;
		private readonly string url;
		private readonly object validationErrors;

		internal M2XAPIException(string url, HttpStatusCode code, string message, object errors)
			: base("M2X API error code " + (int)code + ": " + message)
		{
			this.url = url;
			this.code = code;
			this.validationErrors = errors;
		}

		public string Url
		{
			get { return this.url; }
		}

		public HttpStatusCode ErrorCode
		{
			get { return this.code; }
		}

		public object ValidationErrors
		{
			get { return this.validationErrors;  }
		}
	}

	public abstract class M2XClientBase
	{
		private const string EndPoint = "http://api-m2x.att.com/v1";
		private const string UserAgent = "m2x-dot-net-client";
		private readonly string apiKey;

		protected M2XClientBase(string apiKey)
		{
			if (String.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentException("Invalid API key");

			this.apiKey = apiKey;
		}

		public string APIKey
		{
			get { return this.apiKey; }
		}

		protected object MakeRequest(string url, M2XClientMethod method = M2XClientMethod.GET, object data = null, object queryParams = null)
		{
			string queryString = queryParams == null ? null : ObjectToQueryString(queryParams);
			string fullUrl = BuildUrl(url) + (String.IsNullOrEmpty(queryString) ? "" : ("?" + queryString));
			var request = (HttpWebRequest)WebRequest.Create(fullUrl);
			request.Method = method.ToString();
			request.ContentType = "application/json";
			request.Headers["X-M2X-KEY"] = apiKey;
			request.UserAgent = UserAgent;

			if (data != null && (method == M2XClientMethod.POST || method == M2XClientMethod.PUT))
			{
				using (var streamWriter = new StreamWriter(request.GetRequestStream()))
				{
					streamWriter.Write(SerializeData(data)); 
				}
			}

			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
						return null;

					dynamic jsonObject = GetJsonObjFromResponse(fullUrl, response);

					if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
					{
						throw new M2XAPIException(fullUrl, response.StatusCode, jsonObject == null ? null : jsonObject.message, jsonObject == null ? null : jsonObject.errors);
					}

					return jsonObject;
				}
			}catch(WebException ex)
			{
				if (ex.Response == null)
					throw new M2XAPIException(fullUrl, (HttpStatusCode)0, "WebException - status:" + ex.Status + "; Message: " + ex.Message, null);

				using (var response = (HttpWebResponse)ex.Response)
				{
					dynamic jsonObject = GetJsonObjFromResponse(fullUrl, response);
					throw new M2XAPIException(fullUrl, response.StatusCode, jsonObject == null ? null : jsonObject.message, jsonObject == null ? null : jsonObject.errors);
				}
			}
		}

		private string ObjectToQueryString(object queryParams)
		{
			var type = queryParams.GetType();
			var props = type.GetProperties();
			var pairs = props
				.Where(x => x.GetValue(queryParams, null) != null)
				.Select(x => x.Name + "=" + HttpUtility.UrlEncode(x.GetValue(queryParams, null).ToString()))
				.ToArray();
			return String.Join("&", pairs);
		}

		private string SerializeData(object data)
		{
			var serializer = new JavaScriptSerializer();
			return serializer.Serialize(data);			
		}

		private dynamic GetJsonObjFromResponse(string url, WebResponse webResponse)
		{
			if (webResponse == null)
				return null;

			using (var stream = webResponse.GetResponseStream())
			{
				var reader = new StreamReader(stream, Encoding.UTF8);
				string responseData = reader.ReadToEnd();
				
				try
				{
					return String.IsNullOrWhiteSpace(responseData) ? null : DynamicJsonConverter.Deserialize(responseData);
				}
				catch(ArgumentException ex)
				{
					throw new M2XAPIException(url, (HttpStatusCode)0, "Invalid JSON response - " + responseData, null);
				}
			}
		}

		protected virtual string BuildUrl(string urlPath)
		{
			return EndPoint + urlPath;
		}
	}
}