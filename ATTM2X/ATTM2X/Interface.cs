using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM2X
{
	public sealed class M2XPostedValue
	{
		public DateTime? At { get; set; }
		public string Value { get; set; }
	}

	public sealed class M2XStreamPostedValues
	{
		public string StreamName { get; set; }
		public IEnumerable<M2XPostedValue> PostedValues { get; set; }
	}

	public enum M2XVisibility
	{
		Public,
		Private
	}

	public enum M2XFeedType
	{
		Blueprint, 
		Batch,
		DataSource
	}

	public enum M2XFeedLocationDistanceUnitType
	{
		Mi,
		Miles,
		Km
	}
}
