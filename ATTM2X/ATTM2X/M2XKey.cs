using System;
using System.Net;
using System.Threading.Tasks;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X Keys API
	/// https://m2x.att.com/developer/documentation/v2/keys
	/// </summary>
	public sealed class M2XKey : M2XClass
	{
		public const string UrlPath = "/keys";

		public readonly string KeyId;

		internal M2XKey(M2XClient client, string key)
			: base(client)
		{
			if (String.IsNullOrWhiteSpace(key))
				throw new ArgumentException(String.Format("Invalid key - {0}", key));

			this.KeyId = key;
		}

		internal override string BuildPath(string path)
		{
			return String.Concat(M2XKey.UrlPath, "/", WebUtility.UrlEncode(this.KeyId), path);
		}

		/// <summary>
		/// Regenerate the specified key.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#Regenerate-Key
		/// </summary>
		public Task<M2XResponse> Regenerate()
		{
			return MakeRequest("/regenerate", M2XClientMethod.POST);
		}
	}
}