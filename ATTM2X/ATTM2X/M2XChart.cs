using System;
using System.Web;

namespace ATTM2X
{
	/// <summary>
	/// Wrapper for AT&T M2X Charts API
	/// https://m2x.att.com/developer/documentation/v2/charts
	/// </summary>
	public sealed class M2XChart : M2XClass
	{
		public const string UrlPath = "/charts";

		public readonly string ChartId;

		internal M2XChart(M2XClient client, string chartId)
			: base(client)
		{
			if (String.IsNullOrWhiteSpace(chartId))
				throw new ArgumentException(String.Format("Invalid chartId - {0}", chartId));

			this.ChartId = chartId;
		}

		internal override string BuildPath(string path)
		{
			return String.Concat(M2XChart.UrlPath, "/", HttpUtility.UrlPathEncode(this.ChartId), path);
		}

		/// <summary>
		/// Get details of a specific chart.
		///
		/// This method is public and therefore it does not require
		/// the user to authenticate himself using an API key.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#View-Chart-Details
		/// </summary>
		public M2XResponse Details()
		{
			return MakeRequest();
		}

		/// <summary>
		/// Update an existing chart.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Update-Chart
		/// </summary>
		public M2XResponse Update(object parms)
		{
			return MakeRequest(null, M2XClientMethod.PUT, parms);
		}

		/// <summary>
		/// Delete an existing chart.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Delete-Chart
		/// </summary>
		public M2XResponse Delete()
		{
			return MakeRequest(null, M2XClientMethod.DELETE);
		}

		/// <summary>
		/// Generally used in the src attribute of an <img> html tag.
		///
		/// This method is public and therefore it does not require
		/// the user to authenticate himself using an API key.
		///
		/// https://m2x.att.com/developer/documentation/v2/charts#Render-Chart
		/// </summary>
		public string RenderUrl(string format, object parms = null)
		{
			return this.Client.BuildUrl(BuildPath("." + format), parms);
		}
	}
}
