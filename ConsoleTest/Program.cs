using ATTM2X;
using System;
using System.Net;
using System.Threading;

namespace ConsoleTest
{
	internal class Program
	{
		private static DateTime now;
		private static string testId;
		private static M2XClient m2x;

		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: ConsoleTest [APIKey]");
				Console.ReadKey();
				return;
			}

			Console.WriteLine("Press Any Key to start...");
			Console.ReadKey();

			now = DateTime.UtcNow;
			testId = Guid.NewGuid().ToString("N");
			Console.WriteLine("TestId: {0}", testId);

			m2x = new M2XClient(args[0]);

			Console.WriteLine();
			TestDeviceAPI();

			Console.WriteLine();
			TestDistributionAPI();

			Console.WriteLine();
			TestKeysAPI();

			Console.WriteLine();
			TestChartsAPI();

			Console.WriteLine("Press Any Key to exit...");
			Console.ReadKey();
		}

		static void TestDeviceAPI()
		{
			// device

			m2x.DeviceCatalog(new { page = 1, limit = 10 });
			Console.WriteLine("DeviceCatalog: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.devices.Count);

			m2x.DeviceGroups();
			Console.WriteLine("DeviceGroups: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.groups.Count);

			m2x.CreateDevice(new { name = "TestDevice-" + testId, visibility = M2XVisibility.Private });
			Console.WriteLine("CreateDevice: {0}", m2x.LastResponse.Status);
			string deviceId = m2x.LastResponse.Json.id;
			Console.WriteLine("id: {0}", deviceId);

			m2x.Devices(new { visibility = M2XVisibility.Private });
			Console.WriteLine("Devices: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.devices.Count);

			var device = m2x.Device(deviceId);
			device.Details();
			Console.WriteLine("device.Details: {0}", m2x.LastResponse.Status);
			Console.WriteLine("status: {0}", m2x.LastResponse.Json.status);
			Console.WriteLine("description: {0}", m2x.LastResponse.Json.description);

			device.Update(new { name = "TestDevice-" + testId, visibility = M2XVisibility.Private, description = "test" });
			Console.WriteLine("device.Update: {0}", m2x.LastResponse.Status);
			device.Details();
			Console.WriteLine("description: {0}", m2x.LastResponse.Json.description);

			device.UpdateLocation(new { name = "Test Location", latitude = 12, longitude = -34 });
			Console.WriteLine("device.UpdateLocation: {0}", m2x.LastResponse.Status);

			device.Location();
			Console.WriteLine("device.Location: {0}", m2x.LastResponse.Status);
			if (m2x.LastResponse.Status != HttpStatusCode.NoContent)
				Console.WriteLine("name: {0}", m2x.LastResponse.Json.name);

			// stream

			string streamName = "testdevicestream";
			var stream = device.Stream(streamName);
			stream.CreateOrUpdate(new { type = M2XStreamType.Numeric, unit = new { label = "points", symbol = "pt" } });
			Console.WriteLine("stream.CreateOrUpdate: {0}", m2x.LastResponse.Status);

			device.Streams();
			Console.WriteLine("device.Streams: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.streams.Count);

			stream.UpdateValue(new { value = 10 });
			Console.WriteLine("stream.UpdateValue: {0}", m2x.LastResponse.Status);

			stream.Details();
			Console.WriteLine("stream.Details: {0}", m2x.LastResponse.Status);
			Console.WriteLine("name: {0}", m2x.LastResponse.Json.name);
			Console.WriteLine("value: {0}", m2x.LastResponse.Json.value);

			stream.Values(new { start = M2XClient.DateTimeToString(now.AddHours(-1)) });
			Console.WriteLine("stream.Values: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.values.Count);

			stream.UpdateValue(new { value = 20 });
			Console.WriteLine("stream.UpdateValue: {0}", m2x.LastResponse.Status);
			stream.Sampling(new { type = M2XSamplingType.Sum, interval = 55 });
			Console.WriteLine("stream.Sampling: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.values.Count);

			stream.Stats();
			Console.WriteLine("stream.Stats: {0}", m2x.LastResponse.Status);
			Console.WriteLine("avg: {0}", m2x.LastResponse.Json.stats.avg);

			stream.PostValues(new[]
			{
				new { timestamp = M2XClient.DateTimeToString(now.AddMinutes(-2)), value = 1 },
				new { timestamp = M2XClient.DateTimeToString(now.AddMinutes(-1)), value = 2 },
			});
			Console.WriteLine("stream.PostValues: {0}", m2x.LastResponse.Status);

			stream.DeleteValues(new
			{
				from = M2XClient.DateTimeToString(now.AddMinutes(-2)),
				end = M2XClient.DateTimeToString(now.AddMinutes(-1))
			});
			Console.WriteLine("stream.DeleteValues: {0}", m2x.LastResponse.Status);

			device.PostUpdates(new
			{
				values = new
				{
					testdevicestream = new[] {
					new { timestamp = M2XClient.DateTimeToString(now.AddMinutes(-2)), value = 1 },
					new { timestamp = M2XClient.DateTimeToString(now.AddMinutes(-1)), value = 2 },
				}
				}
			});
			Console.WriteLine("device.PostUpdates: {0}", m2x.LastResponse.Status);

			// trigger

			device.CreateTrigger(new { stream = streamName, name = "test trigger", condition = M2XTriggerCondition.Equal, value = 0, callback_url = M2XClient.ApiEndPoint });
			Console.WriteLine("device.CreateTrigger: {0}", m2x.LastResponse.Status);
			string triggerId = m2x.LastResponse.Json.id;
			Console.WriteLine("id: {0}", triggerId);

			device.Triggers();
			Console.WriteLine("device.Triggers: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.triggers.Count);

			var trigger = device.Trigger(triggerId);
			trigger.Details();
			Console.WriteLine("trigger.Details: {0}", m2x.LastResponse.Status);
			Console.WriteLine("status: {0}", m2x.LastResponse.Json.status);
			Console.WriteLine("name: {0}", m2x.LastResponse.Json.name);

			string triggerName = "test trigger " + testId;
			trigger.Update(new { stream = streamName, name = triggerName, condition = M2XTriggerCondition.Equal, value = 0, callback_url = M2XClient.ApiEndPoint });
			Console.WriteLine("trigger.Update: {0}", m2x.LastResponse.Status);

			trigger.Test(new
			{
				device_id = deviceId,
				stream = streamName,
				trigger_name = "test",
				trigger_description = "test",
				condition = M2XTriggerCondition.Equal,
				threshold = 3.5,
				value = 0,
				timestamp = M2XClient.DateTimeToString(now),
			});
			Console.WriteLine("trigger.Test: {0}", m2x.LastResponse.Status);

			device.Log();
			Console.WriteLine("device.Log: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.requests.Count);

			trigger.Delete();
			Console.WriteLine("trigger.Delete: {0}", m2x.LastResponse.Status);

			stream.Delete();
			Console.WriteLine("stream.Delete: {0}", m2x.LastResponse.Status);

			device.Delete();
			Console.WriteLine("device.Delete: {0}", m2x.LastResponse.Status);
		}

		private static void TestDistributionAPI()
		{
			// distribution

			m2x.CreateDistribution(new { name = "TestDistribution-" + testId, visibility = M2XVisibility.Public });
			Console.WriteLine("CreateDistribution: {0}", m2x.LastResponse.Status);
			string distributionId = m2x.LastResponse.Json.id;
			Console.WriteLine("id: {0}", distributionId);

			m2x.Distributions();
			Console.WriteLine("Distributions: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.distributions.Count);

			var distribution = m2x.Distribution(distributionId);
			distribution.Details();
			Console.WriteLine("distribution.Details: {0}", m2x.LastResponse.Status);
			Console.WriteLine("name: {0}", m2x.LastResponse.Json.name);
			Console.WriteLine("devices: {0}", m2x.LastResponse.Json.devices.Count);

			distribution.Update(new { name = "TestDistribution-" + testId, visibility = M2XVisibility.Private, description = "test" });
			Console.WriteLine("distribution.Update: {0}", m2x.LastResponse.Status);

			// device

			distribution.AddDevice(new { serial = testId });
			Console.WriteLine("distribution.AddDevice: {0}", m2x.LastResponse.Status);
			string deviceId = m2x.LastResponse.Json.id;
			Console.WriteLine("id: {0}", deviceId);

			distribution.Devices();
			Console.WriteLine("distribution.Devices: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.devices.Count);

			// stream

			string streamName = "testdevicestream";
			var stream = distribution.Stream(streamName);
			stream.CreateOrUpdate(new { type = M2XStreamType.Numeric, unit = new { label = "points", symbol = "pt" } });
			Console.WriteLine("stream.CreateOrUpdate: {0}", m2x.LastResponse.Status);

			distribution.Streams();
			Console.WriteLine("distribution.Streams: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.streams.Count);

			stream.Details();
			Console.WriteLine("stream.Details: {0}", m2x.LastResponse.Status);
			Console.WriteLine("name: {0}", m2x.LastResponse.Json.name);

			// trigger

			distribution.CreateTrigger(new { stream = streamName, name = "test trigger", condition = M2XTriggerCondition.Equal, value = 0, callback_url = M2XClient.ApiEndPoint });
			Console.WriteLine("distribution.CreateTrigger: {0}", m2x.LastResponse.Status);
			string triggerId = m2x.LastResponse.Json.id;
			Console.WriteLine("id: {0}", triggerId);

			distribution.Triggers();
			Console.WriteLine("distribution.Triggers: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.triggers.Count);

			var trigger = distribution.Trigger(triggerId);
			trigger.Details();
			Console.WriteLine("trigger.Details: {0}", m2x.LastResponse.Status);
			Console.WriteLine("status: {0}", m2x.LastResponse.Json.status);
			Console.WriteLine("name: {0}", m2x.LastResponse.Json.name);

			string triggerName = "test trigger " + testId;
			trigger.Update(new { stream = streamName, name = triggerName, condition = M2XTriggerCondition.Less, value = 0, callback_url = M2XClient.ApiEndPoint });
			Console.WriteLine("trigger.Update: {0}", m2x.LastResponse.Status);

			trigger.Test(new
			{
				device_id = deviceId,
				stream = streamName,
				trigger_name = "test",
				trigger_description = "test",
				condition = M2XTriggerCondition.Equal,
				threshold = 3.5,
				value = -1,
				timestamp = M2XClient.DateTimeToString(now),
			});
			Console.WriteLine("trigger.Test: {0}", m2x.LastResponse.Status);

			trigger.Delete();
			Console.WriteLine("trigger.Delete: {0}", m2x.LastResponse.Status);

			stream.Delete();
			Console.WriteLine("stream.Delete: {0}", m2x.LastResponse.Status);

			var device = m2x.Device(deviceId);
			device.Delete();
			Console.WriteLine("device.Delete: {0}", m2x.LastResponse.Status);

			distribution.Delete();
			Console.WriteLine("distribution.Delete: {0}", m2x.LastResponse.Status);
		}

		private static void TestKeysAPI()
		{
			// keys

			m2x.Keys();
			Console.WriteLine("Keys: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.keys.Count);

			m2x.CreateKey(new { name = "testkey" + testId, permissions = new[] { "POST" } });
			Console.WriteLine("CreateKey: {0}", m2x.LastResponse.Status);
			string keystr = m2x.LastResponse.Json.key;
			Console.WriteLine("key: {0}", keystr);

			Thread.Sleep(1000);

			var key = m2x.Key(keystr);
			key.Details();
			Console.WriteLine("key.Details: {0}", m2x.LastResponse.Status);
			Console.WriteLine("master: {0}", m2x.LastResponse.Json.master);

			key.Update(new { name = "testkey" + testId, permissions = new[] { "GET" } });
			Console.WriteLine("key.Update: {0}", m2x.LastResponse.Status);

			key.Regenerate();
			Console.WriteLine("key.Regenerate: {0}", m2x.LastResponse.Status);
			keystr = m2x.LastResponse.Json.key;
			Console.WriteLine("key: {0}", keystr);

			Thread.Sleep(1000);

			key = m2x.Key(keystr);
			key.Delete();
			Console.WriteLine("key.Delete: {0}", m2x.LastResponse.Status);
		}

		private static void TestChartsAPI()
		{
			// device & stream

			m2x.CreateDevice(new { name = "TestDevice-" + testId, visibility = M2XVisibility.Public });
			Console.WriteLine("CreateDevice: {0}", m2x.LastResponse.Status);
			string deviceId = m2x.LastResponse.Json.id;
			Console.WriteLine("id: {0}", deviceId);
			var device = m2x.Device(deviceId);

			var stream = device.Stream("testdevicestream");
			stream.CreateOrUpdate(new { type = M2XStreamType.Numeric, unit = new { label = "points", symbol = "pt" } });
			Console.WriteLine("stream.CreateOrUpdate: {0}", m2x.LastResponse.Status);

			Thread.Sleep(1000);

			stream.PostValues(new[]
			{
				new { timestamp = M2XClient.DateTimeToString(now.AddMinutes(-3)), value = 3 },
				new { timestamp = M2XClient.DateTimeToString(now.AddMinutes(-2)), value = 1 },
				new { timestamp = M2XClient.DateTimeToString(now.AddMinutes(-1)), value = 2 },
				new { timestamp = M2XClient.DateTimeToString(now), value = 3 },
			});
			Console.WriteLine("stream.PostValues: {0}", m2x.LastResponse.Status);

			// charts

			m2x.CreateChart(new { name = "testchart" + testId, series = new []
			{
				new { device = device.DeviceId, stream = stream.StreamName }
			}});
			Console.WriteLine("CreateChart: {0}", m2x.LastResponse.Status);
			string id = m2x.LastResponse.Json.id;
			Console.WriteLine("id: {0}", id);

			Thread.Sleep(1000);

			var chart = m2x.Chart(id);
			chart.Details();
			Console.WriteLine("chart.Details: {0}", m2x.LastResponse.Status);
			Console.WriteLine("name: {0}", m2x.LastResponse.Json.name);

			chart.Update(new { name = "testchart_" + testId, series = new []
			{
				new { device = device.DeviceId, stream = stream.StreamName }
			}});
			Console.WriteLine("chart.Update: {0}", m2x.LastResponse.Status);

			m2x.Charts();
			Console.WriteLine("Charts: {0}", m2x.LastResponse.Status);
			Console.WriteLine("count: {0}", m2x.LastResponse.Json.charts.Count);

			string url = chart.RenderUrl(M2XRenderFormat.Png, new { width = 100, height = 50 });
			Console.WriteLine("chart.RenderUrl: {0}", url);

			chart.Delete();
			Console.WriteLine("chart.Delete: {0}", m2x.LastResponse.Status);

			stream.Delete();
			Console.WriteLine("stream.Delete: {0}", m2x.LastResponse.Status);

			device.Delete();
			Console.WriteLine("device.Delete: {0}", m2x.LastResponse.Status);
		}
	}
}
