using System;
using System.IO;
using System.Net;
using System.Text;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X API response
	/// </summary>
	public sealed class M2XResponse
	{
		/// <summary>
		/// The URL of API call
		/// </summary>
		public readonly Uri RequestUri;
		/// <summary>
		/// The HTTP method of API call
		/// </summary>
		public readonly string RequestMethod;
		/// <summary>
		/// The headers of API call
		/// </summary>
		public readonly WebHeaderCollection RequestHeaders;
		/// <summary>
		/// The text of API call
		/// </summary>
		public readonly string RequestContent;
		/// <summary>
		/// The exception occured during API call
		/// </summary>
		public readonly WebException WebError;

		/// <summary>
		/// The status code of the response.
		/// </summary>
		public readonly HttpStatusCode Status;
		/// <summary>
		/// The headers included on the response.
		/// </summary>
		public readonly WebHeaderCollection Headers;
		/// <summary>
		/// The raw response body.
		/// </summary>
		public readonly string Raw;

		private dynamic json;
		/// <summary>
		/// The parsed response body.
		/// </summary>
		public dynamic Json
		{
			get
			{
				if (this.json != null)
					return this.json;
				if (String.IsNullOrWhiteSpace(this.Raw))
					return null;
				try
				{
					this.json = DynamicJsonConverter.Deserialize(this.Raw);
				}
				catch (ArgumentException)
				{
				}
				return this.json;
			}
		}

		/// <summary>
		/// Whether Status is a success (status code 2xx)
		/// </summary>
		public bool Success
		{
			get { return (int)this.Status >= 200 && (int)this.Status < 300; }
		}
		/// <summary>
		/// Whether Status is one of 4xx
		/// </summary>
		public bool ClientError
		{
			get { return (int)this.Status >= 400 && (int)this.Status < 500; }
		}
		/// <summary>
		/// Whether Status is one of 5xx
		/// </summary>
		public bool ServerError
		{
			get { return (int)this.Status >= 500 && (int)this.Status < 600; }
		}
		/// <summary>
		/// Whether ClientError or ServerError is true
		/// </summary>
		public bool Error
		{
			get { return this.ClientError || this.ServerError; }
		}

		internal M2XResponse(HttpWebRequest request, string requestContent, WebException error, HttpWebResponse response)
		{
			this.RequestUri = request.RequestUri;
			this.RequestMethod = request.Method;
			this.RequestHeaders = request.Headers;
			this.RequestContent = requestContent;
			this.WebError = error;

			if (response == null)
				return;

			this.Status = response.StatusCode;
			this.Headers = response.Headers;

			Stream stream = response.GetResponseStream();
			if (stream != null)
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8))
					this.Raw = reader.ReadToEnd();
			}
		}
	}
}
