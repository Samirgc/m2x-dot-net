using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ATTM2X
{
	internal sealed class DynamicJsonConverter : JavaScriptConverter
	{
		public static dynamic Deserialize(string json)
		{
			var serializer = new JavaScriptSerializer { MaxJsonLength = json.Length };
			serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
			return serializer.Deserialize(json, typeof(object));
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) }));
			}
		}

		private sealed class DynamicJsonObject : DynamicObject
		{
			private readonly IDictionary<string, object> dictionary;

			public DynamicJsonObject(IDictionary<string, object> dict)
			{
				if (dict == null)
					throw new ArgumentNullException("dict");
				this.dictionary = dict;
			}

			public override string ToString()
			{
				var sb = new StringBuilder("{");
				ToString(sb);
				return sb.ToString();
			}

			private void ToString(StringBuilder sb)
			{
				var firstInDictionary = true;
				foreach (var pair in dictionary)
				{
					if (!firstInDictionary)
						sb.Append(",");
					firstInDictionary = false;
					var value = pair.Value;
					var name = pair.Key;
					if (value is string)
					{
						sb.AppendFormat("{0}:\"{1}\"", name, value);
					}
					else if (value is IDictionary<string, object>)
					{
						new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
					}
					else if (value is ArrayList)
					{
						sb.Append(name + ":[");
						var firstInArray = true;
						foreach (var arrayValue in (ArrayList)value)
						{
							if (!firstInArray)
								sb.Append(",");
							firstInArray = false;
							if (arrayValue is IDictionary<string, object>)
								new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
							else if (arrayValue is string)
								sb.AppendFormat("\"{0}\"", arrayValue);
							else
								sb.AppendFormat("{0}", arrayValue);

						}
						sb.Append("]");
					}
					else
					{
						sb.AppendFormat("{0}:{1}", name, value);
					}
				}
				sb.Append("}");
			}

			public override bool TryGetMember(GetMemberBinder binder, out object result)
			{
				if (!this.dictionary.TryGetValue(binder.Name, out result))
				{
					result = null;
					return true;
				}

				var dictionary = result as IDictionary<string, object>;
				if (dictionary != null)
				{
					result = new DynamicJsonObject(dictionary);
					return true;
				}

				var arrayList = result as ArrayList;
				if (arrayList != null)
				{
					if (arrayList.Count > 0 && arrayList[0] is IDictionary<string, object>)
						result = new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
					else
						result = new List<object>(arrayList.Cast<object>());
				}

				return true;
			}
		}
	}
}