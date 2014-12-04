using System;
using System.Web;

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
			return String.Concat(M2XKey.UrlPath, "/", HttpUtility.UrlPathEncode(this.KeyId), path);
		}

		/// <summary>
		/// Get details of a specific key associated with a developer account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#View-Key-Details
		/// </summary>
		public M2XResponse Details()
		{
			return MakeRequest();
		}

		/// <summary>
		/// Update name, stream, permissions, expiration date, origin or device access
		/// of an existing key associated with the specified account.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#Update-Key
		/// </summary>
		public M2XResponse Update(object parms)
		{
			return MakeRequest(null, M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Regenerate the specified key.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#Regenerate-Key
		/// </summary>
		public M2XResponse Regenerate()
		{
			return MakeRequest("/regenerate", M2XClientMethod.POST);
		}

		/// <summary>
		/// Delete an existing key.
		///
		/// https://m2x.att.com/developer/documentation/v2/keys#Delete-Key
		/// </summary>
		public M2XResponse Delete()
		{
			return MakeRequest(null, M2XClientMethod.DELETE);
		}
	}
}