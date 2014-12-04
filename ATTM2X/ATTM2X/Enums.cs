
namespace ATTM2X
{
	public enum M2XClientMethod
	{
		GET,
		POST,
		PUT,
		DELETE,
	}

	public static class M2XDistanceUnit
	{
		public const string Mi = "mi";
		public const string Miles = "miles";
		public const string Km = "km";
	}

	public static class M2XVisibility
	{
		public const string Public = "public";
		public const string Private = "private";
	}

	public static class M2XStreamType
	{
		public const string Numeric = "numeric";
		public const string Alphanumeric = "alphanumeric";
	}

	public static class M2XSamplingType
	{
		public const string Nth = "nth";
		public const string Min = "min";
		public const string Max = "max";
		public const string Count = "count";
		public const string Avg = "avg";
		public const string Sum = "sum";
	}

	public static class M2XTriggerStatus
	{
		public const string Enabled = "enabled";
		public const string Disabled = "disabled";
	}

	public static class M2XTriggerCondition
	{
		public const string Less = "<";
		public const string LessOrEqual = "<=";
		public const string Equal = "=";
		public const string GreaterOrEqual = ">=";
		public const string Greater = ">";
		public const string Between = "..";
	}

	public static class M2XRenderFormat
	{
		public const string Png = "png";
		public const string Svg = "svg";
	}
}
