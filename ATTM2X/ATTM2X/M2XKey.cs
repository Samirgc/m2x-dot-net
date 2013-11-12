using System;
using System.Web;

namespace ATTM2X
{
	//API Keys:
	//http://api-m2x.att.com/v1/keys
	//https://m2x.att.com/developer/documentation/keys
	public sealed class M2XKey : M2XClientBase
	{
		private readonly string keyId;

		internal M2XKey(string apiKey, string keyId)
			: base(apiKey)
		{
			if (String.IsNullOrWhiteSpace(keyId))
				throw new ArgumentException("Invalid keyId - " + keyId);

			this.keyId = keyId;
		}

		public string KeyId
		{
			get { return this.keyId; }
		}

		public dynamic Details()
		{
			return MakeRequest(String.Empty);
		}

		public void Update(object data)
		{
			MakeRequest(String.Empty, M2XClientMethod.PUT, data);
		}

		public dynamic Regenerate()
		{
			return MakeRequest("/regenerate", M2XClientMethod.POST);
		}

		public void Delete()
		{
			MakeRequest(String.Empty, M2XClientMethod.DELETE);
		}

		protected override string BuildUrl(string urlPath)
		{
			return base.BuildUrl("/keys/" + HttpUtility.UrlPathEncode(keyId) + urlPath);
		}
	}
}