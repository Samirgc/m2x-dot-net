using System;
using System.Runtime.Serialization;

namespace ATTM2X.Classes
{
	public class DeviceList
	{
		public DeviceDetails[] devices;
	}

	public class DeviceDetails
	{
		public string id;
		public string name;
		public string description;
		public string visibility;
		public string status;
		public string serial;
		public string[] tags;
		public string url;
		public LocationDetails location;
		public string created;
		public string updated;
	}

	public class LocationDetails : WaypointDetails
	{
		public string name;
		public WaypointDetails[] waypoints;
	}

	public class WaypointDetails
	{
		public string timestamp;
		public double? latitude;
		public double? longitude;
		public int? elevation;
	}

	[DataContract]
	public class DeviceParams
	{
		[DataMember]
		public string name;
		[DataMember(EmitDefaultValue = false)]
		public string description;
		[DataMember]
		public string visibility;
		[DataMember(EmitDefaultValue = false)]
		public string tags;
	}

	[DataContract]
	public class LocationParams
	{
		[DataMember]
		public string name;
		[DataMember]
		public double latitude;
		[DataMember]
		public double longitude;
		[DataMember(EmitDefaultValue = false)]
		public int? elevation;
		[DataMember(EmitDefaultValue = false)]
		public string timestamp;
	}

	public class RequestList
	{
		public RequestDetails[] requests;
	}

	public class RequestDetails
	{
		public string timestamp;
		public int status;
		public string method;
		public string path;
	}
}
