using System;
using System.Runtime.Serialization;

namespace ATTM2X.Classes
{
	public class StreamList
	{
		public StreamDetails[] streams;
	}

	public class StreamDetails
	{
		public string name;
		public string type;
		public string value;
		public StreamUnit unit;
		public string url;
		public string created;
		public string updated;
	}

	public class StreamUnit
	{
		public string label;
		public string symbol;
	}

	[DataContract]
	public class StreamParams
	{
		[DataMember(EmitDefaultValue = false)]
		public string type;
		[DataMember]
		public StreamUnit unit;
	}

	[DataContract]
	public class StreamValues
	{
		[DataMember(EmitDefaultValue = false)]
		public string start;
		[DataMember(EmitDefaultValue = false)]
		public string end;
		[DataMember(EmitDefaultValue = false)]
		public int? limit;
		[DataMember]
		public StreamValue[] values;
	}

	[DataContract]
	public class StreamValue
	{
		[DataMember(EmitDefaultValue = false)]
		public string timestamp;
		[DataMember]
		public string value;
	}

	public class StreamStatsInfo
	{
		public string start;
		public string end;
		public StreamStats stats;
	}

	public class StreamStats
	{
		public int count;
		public double min;
		public double max;
		public double avg;
		public double stddev;
	}
}
