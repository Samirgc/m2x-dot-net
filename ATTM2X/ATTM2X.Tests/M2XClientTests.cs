using ATTM2X;
using ATTM2X.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading;

namespace ATTM2X.Tests
{
	[TestClass]
	public class M2XClientTests : TestBase
	{
		[TestMethod]
		public void ErrorTest()
		{
			response = m2x.CreateDevice(new DeviceParams()).Result;
			Assert.AreEqual(422, (int)response.Status, response.Raw);
			var errorInfo = response.Json<ErrorInfo>();
			Assert.IsNotNull(errorInfo.message);
		}

		[TestMethod]
		public void DeviceAPITest()
		{
			// device

			response = m2x.DeviceTags().Result;
			Assert.IsNotNull(m2x.LastResponse);
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			Assert.IsTrue(response.Success);
			Assert.IsFalse(response.Error);
			Assert.IsNotNull(response.Raw);

			response = m2x.DeviceCatalog(new { page = 1, limit = 10 }).Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var deviceList = response.Json<DeviceList>();
			Assert.IsNotNull(deviceList.devices);
			Assert.IsTrue(deviceList.devices.Length > 0);

			response = m2x.CreateDevice(new DeviceParams
				{
					name = "TestDevice-" + this.TestId,
					visibility = M2XVisibility.Private,
				}).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			var deviceDetails = response.Json<DeviceDetails>();
			Assert.IsNotNull(deviceDetails.id);
			this.device = m2x.Device(deviceDetails.id);
			Assert.AreEqual(M2XVisibility.Private, deviceDetails.visibility);

			Thread.Sleep(1000);

			response = device.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			deviceDetails = response.Json<DeviceDetails>();
			Assert.AreEqual(M2XStatus.Enabled, deviceDetails.status);

			response = m2x.Devices(new { visibility = M2XVisibility.Private }).Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			deviceList = response.Json<DeviceList>();
			Assert.IsNotNull(deviceList.devices);
			Assert.IsTrue(deviceList.devices.Length > 0);

			response = device.Update(new DeviceParams
				{
					name = "TestDevice-" + this.TestId,
					visibility = M2XVisibility.Private,
					description = "test",
				}).Result;
			Assert.AreEqual(HttpStatusCode.NoContent, response.Status, response.Raw);
			response = device.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			deviceDetails = response.Json<DeviceDetails>();
			Assert.AreEqual("test", deviceDetails.description);

			response = device.UpdateLocation(new LocationParams
				{
					name = "Test Location",
					latitude = 12,
					longitude = -34,
				}).Result;
			Assert.AreEqual(HttpStatusCode.Accepted, response.Status, response.Raw);

			Thread.Sleep(1000);

			response = device.Location().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var location = response.Json<LocationDetails>();
			Assert.AreEqual("Test Location", location.name);

			response = device.Log().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var requestList = response.Json<RequestList>();
			Assert.IsNotNull(requestList.requests);
			Assert.IsTrue(requestList.requests.Length > 0);

			// stream

			this.stream = device.Stream("testdevicestream");
			response = stream.Update(new StreamParams
				{
					type = M2XStreamType.Numeric,
					unit = new StreamUnit { label = "points", symbol = "pt" },
				}).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);

			Thread.Sleep(1000);

			response = device.Streams().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var streamList = response.Json<StreamList>();
			Assert.IsNotNull(streamList.streams);
			Assert.AreEqual(1, streamList.streams.Length);
			Assert.AreEqual(stream.StreamName, streamList.streams[0].name);

			response = stream.UpdateValue(new StreamValue { value = "10" }).Result;
			Assert.AreEqual(HttpStatusCode.Accepted, response.Status, response.Raw);

			Thread.Sleep(1000);

			response = stream.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var streamDetails = response.Json<StreamDetails>();
			Assert.AreEqual("10.0", streamDetails.value);

			response = stream.Values(new { start = M2XClient.DateTimeToString(this.UtcNow.AddHours(-1)) }).Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var streamValues = response.Json<StreamValues>();
			Assert.IsNotNull(streamValues.values);
			Assert.AreEqual(1, streamValues.values.Length);

			response = stream.UpdateValue(new StreamValue { value = "20" }).Result;
			Assert.AreEqual(HttpStatusCode.Accepted, response.Status, response.Raw);

			Thread.Sleep(1000);

			response = stream.Sampling(new { type = M2XSamplingType.Sum, interval = 100 }).Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			streamValues = response.Json<StreamValues>();
			Assert.IsNotNull(streamValues.values);
			Assert.AreEqual(1, streamValues.values.Length);
			Assert.AreEqual("30.0", streamValues.values[0].value);

			response = stream.Stats().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var streamStats = response.Json<StreamStatsInfo>();
			Assert.IsNotNull(streamStats.stats);
			Assert.AreEqual(2, streamStats.stats.count);
			Assert.AreEqual(15.0, streamStats.stats.avg);

			string from = M2XClient.DateTimeToString(this.UtcNow.AddMinutes(-2));
			string end = M2XClient.DateTimeToString(this.UtcNow.AddMinutes(-1));
			response = stream.PostValues(new StreamValues
			{
				values = new StreamValue[]
				{
					new StreamValue { timestamp = from, value = "1", },
					new StreamValue { timestamp = end, value = "2", },
				}
			}).Result;
			Assert.AreEqual(HttpStatusCode.Accepted, response.Status, response.Raw);

			response = stream.DeleteValues(new { from, end }).Result;
			Assert.AreEqual(HttpStatusCode.NoContent, response.Status, response.Raw);

			// trigger

			var triggerParams = new TriggerParams
			{
				stream = stream.StreamName,
				name = "test trigger",
				condition = M2XTriggerCondition.Equal,
				value = "0",
				callback_url = M2XClient.ApiEndPoint,
			};
			response = device.CreateTrigger(triggerParams).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			var triggerDetails = response.Json<TriggerDetails>();
			string triggerId = triggerDetails.id;
			Assert.IsNotNull(triggerId);
			this.trigger = device.Trigger(triggerId);

			Thread.Sleep(1000);

			response = trigger.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			triggerDetails = response.Json<TriggerDetails>();
			Assert.AreEqual(M2XStatus.Enabled, triggerDetails.status);
			Assert.AreEqual(triggerParams.name, triggerDetails.name);

			response = device.Triggers().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var triggerList = response.Json<TriggerList>();
			Assert.IsNotNull(triggerList.triggers);
			Assert.AreEqual(1, triggerList.triggers.Length);
			Assert.AreEqual(stream.StreamName, triggerList.triggers[0].stream);

			triggerParams.name += this.TestId;
			response = trigger.Update(triggerParams).Result;
			Assert.AreEqual(HttpStatusCode.NoContent, response.Status, response.Raw);

			trigger.Test(new TriggerTestParams
			{
				device_id = device.DeviceId,
				stream = stream.StreamName,
				trigger_name = "test",
				trigger_description = "test",
				condition = M2XTriggerCondition.Equal,
				threshold = 3.5,
				value = "0",
				timestamp = M2XClient.DateTimeToString(this.UtcNow),
			});
			Assert.AreEqual(HttpStatusCode.NoContent, response.Status, response.Raw);
		}

		[Ignore]
		[TestMethod]
		public void DistributionApiTest()
		{
			// distribution

			response = m2x.CreateDistribution(new DistributionParams
				{
					name = "TestDistribution-" + this.TestId,
					visibility = M2XVisibility.Public,
				}).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			var distributionDetails = response.Json<DistributionDetails>();
			string distributionId = distributionDetails.id;
			Assert.IsNotNull(distributionId);
			this.distribution = m2x.Distribution(distributionId);
			Assert.AreEqual(M2XVisibility.Public, distributionDetails.visibility);

			Thread.Sleep(1000);

			response = distribution.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			distributionDetails = response.Json<DistributionDetails>();
			Assert.AreEqual(M2XStatus.Enabled, distributionDetails.status);

			response = m2x.Distributions().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var distributionList = response.Json<DistributionList>();
			Assert.IsNotNull(distributionList.distributions);
			Assert.IsTrue(distributionList.distributions.Length > 0);

			response = distribution.Update(new DistributionParams
			{
				name = distributionDetails.name,
				visibility = M2XVisibility.Private,
				description = "test",
			}).Result;
			Assert.AreEqual(HttpStatusCode.NoContent, response.Status, response.Raw);

			// device

			response = distribution.AddDevice(new DistributionDeviceParams { serial = this.TestId }).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			var deviceDetails = response.Json<DeviceDetails>();
			Assert.IsNotNull(deviceDetails.id);
			this.device = m2x.Device(deviceDetails.id);
			Assert.AreEqual(this.TestId, deviceDetails.serial);

			Thread.Sleep(1000);

			response = distribution.Devices().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var deviceList = response.Json<DeviceList>();
			Assert.IsNotNull(deviceList.devices);
			Assert.AreEqual(1, deviceList.devices.Length);

			// stream

			this.stream = distribution.Stream("testdevicestream");
			response = stream.Update(new StreamParams
			{
				unit = new StreamUnit { label = "points", symbol = "pt" },
			}).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);

			Thread.Sleep(1000);

			response = stream.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var streamDetails = response.Json<StreamDetails>();
			Assert.AreEqual(M2XStreamType.Numeric, streamDetails.type);

			response = distribution.Streams().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var streamList = response.Json<StreamList>();
			Assert.IsNotNull(streamList.streams);
			Assert.AreEqual(1, streamList.streams.Length);

			// trigger

			var triggerParams = new TriggerParams
			{
				stream = stream.StreamName,
				name = "test trigger",
				condition = M2XTriggerCondition.Equal,
				value = "0",
				callback_url = M2XClient.ApiEndPoint,
			};
			response = distribution.CreateTrigger(triggerParams).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			var triggerDetails = response.Json<TriggerDetails>();
			string triggerId = triggerDetails.id;
			Assert.IsNotNull(triggerId);
			this.trigger = distribution.Trigger(triggerId);

			Thread.Sleep(1000);

			response = trigger.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			triggerDetails = response.Json<TriggerDetails>();
			Assert.AreEqual(M2XStatus.Enabled, triggerDetails.status);
			Assert.AreEqual(triggerParams.name, triggerDetails.name);

			response = distribution.Triggers().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var triggerList = response.Json<TriggerList>();
			Assert.IsNotNull(triggerList.triggers);
			Assert.AreEqual(1, triggerList.triggers.Length);
			Assert.AreEqual(stream.StreamName, triggerList.triggers[0].stream);
		}

		[TestMethod]
		public void KeyApiTest()
		{
			// keys

			response = m2x.Keys().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var keyList = response.Json<KeyList>();
			Assert.IsNotNull(keyList.keys);
			Assert.IsTrue(keyList.keys.Length > 0);

			response = m2x.CreateKey(new KeyParams
				{
					name = "testkey" + this.TestId,
					permissions = new[] { "POST" },
				}).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			var keyDetails = response.Json<KeyDetails>();
			Assert.IsNotNull(keyDetails.key);
			this.key = m2x.Key(keyDetails.key);

			Thread.Sleep(1000);

			response = key.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			keyDetails = response.Json<KeyDetails>();
			Assert.IsTrue(keyDetails.master);
			Assert.IsFalse(keyDetails.expired);

			response = key.Update(new KeyParams
				{
					name = keyDetails.name,
					permissions = new[] { "GET" },
					device_access = M2XVisibility.Public,
				}).Result;
			Assert.AreEqual(HttpStatusCode.NoContent, response.Status, response.Raw);

			response = key.Regenerate().Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			keyDetails = response.Json<KeyDetails>();
			Assert.IsNotNull(keyDetails.key);
			var newKey = m2x.Key(keyDetails.key);

			Thread.Sleep(1000);

			Delete(newKey);
			key = null;
		}

		[TestMethod]
		public void ChartApiTest()
		{
			// device & stream

			response = m2x.CreateDevice(new DeviceParams
			{
				name = "TestDevice-" + this.TestId,
				visibility = M2XVisibility.Private,
			}).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			var deviceDetails = response.Json<DeviceDetails>();
			Assert.IsNotNull(deviceDetails.id);
			this.device = m2x.Device(deviceDetails.id);

			Thread.Sleep(1000);

			this.stream = device.Stream("testdevicestream");
			response = stream.Update(new StreamParams
			{
				type = M2XStreamType.Numeric,
				unit = new StreamUnit { label = "points", symbol = "pt" },
			}).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);

			Thread.Sleep(1000);

			response = stream.PostValues(new StreamValues
			{
				values = new StreamValue[]
				{
					new StreamValue { timestamp = M2XClient.DateTimeToString(this.UtcNow.AddMinutes(-3)), value = "30" },
					new StreamValue { timestamp = M2XClient.DateTimeToString(this.UtcNow.AddMinutes(-2)), value = "10" },
					new StreamValue { timestamp = M2XClient.DateTimeToString(this.UtcNow.AddMinutes(-1)), value = "20" },
					new StreamValue { timestamp = M2XClient.DateTimeToString(this.UtcNow), value = "30" },
				}
			}).Result;
			Assert.AreEqual(HttpStatusCode.Accepted, response.Status, response.Raw);

			// charts

			var chartParams = new ChartParams
			{
				name = "testchart" + this.TestId,
				series = new ChartSeries[]
				{
					new ChartSeries { device = device.DeviceId, stream = stream.StreamName }
				}
			};
			response = m2x.CreateChart(chartParams).Result;
			Assert.AreEqual(HttpStatusCode.Created, response.Status, response.Raw);
			var chartDetails = response.Json<ChartDetails>();
			Assert.IsNotNull(chartDetails.id);
			this.chart = m2x.Chart(chartDetails.id);
			Assert.AreEqual(chartParams.name, chartDetails.name);
			Assert.IsNotNull(chartDetails.series);
			Assert.AreEqual(1, chartDetails.series.Length);
			Assert.AreEqual(device.DeviceId, chartDetails.series[0].device);
			Assert.AreEqual(stream.StreamName, chartDetails.series[0].stream);

			Thread.Sleep(1000);

			response = chart.Details().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			chartDetails = response.Json<ChartDetails>();
			Assert.AreEqual(chartParams.name, chartDetails.name);
			Assert.IsNotNull(chartDetails.series);

			chartParams.name += "_";
			response = chart.Update(chartParams).Result;
			Assert.AreEqual(HttpStatusCode.NoContent, response.Status, response.Raw);

			response = m2x.Charts().Result;
			Assert.AreEqual(HttpStatusCode.OK, response.Status, response.Raw);
			var chartList = response.Json<ChartList>();
			Assert.IsNotNull(chartList.charts);
			Assert.AreEqual(1, chartList.charts.Length);

			string url = chart.RenderUrl(M2XRenderFormat.Png, new { width = 100, height = 50 });
			Assert.IsNotNull(url);
		}
	}
}
