AT&T's M2X .NET Client
========================

[AT&T’s M2X](https://m2x.att.com/) is a cloud-based fully managed data storage service for network connected machine-to-machine (M2M) devices. From trucks and turbines to vending machines and freight containers, M2X enables the devices that power your business to connect and share valuable data.
This library aims to provide a simple wrapper to interact with the [AT&T M2X API](https://m2x.att.com/developer/documentation/overview) for [.NET](http://www.microsoft.com/net). Refer to the [Glossary of Terms](https://m2x.att.com/developer/documentation/glossary) to understand the nomenclature used throughout this documentation.

Getting Started
==========================

1. Signup for an [M2X Account](https://m2x.att.com/signup).
2. Obtain your _Master Key_ from the Master Keys tab of your [Account Settings](https://m2x.att.com/account) screen.
2. Create your first [Device](https://m2x.att.com/devices) and copy its _Device ID_.
3. Review the [M2X API Documentation](https://m2x.att.com/developer/documentation/overview).

Installation and System Requirements
==========================

The M2X API .NET Client library is a regular MS VS 2012 Class Library portable for universal apps. The only dependency is .NET Framework version 4.5 which can be downloaded here:  http://www.microsoft.com/en-us/download/details.aspx?id=30653. 

Simply add it as an Existing Project into your VS solution or if you are using a different version of Visual Studio you can create a new class library project and include the content of the [ATTM2X/ATTM2X](https://github.com/attm2x/m2x-dot-net/tree/master/ATTM2X/ATTM2X) folder into it. 

Besides the API Client library, this solution also includes a tests project which contains multiple examples of library usage, which can be found here: [ATTM2X.Tests](https://github.com/attm2x/m2x-dot-net/tree/master/ATTM2X/ATTM2X.Tests) folder.

System requirements match those for .NET Framework 4.5.

 - Supported Operating System:

		Windows 7 Service Pack 1, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Vista Service Pack 2
		Windows Vista SP2 (x86 and x64)
		Windows 7 SP1 (x86 and x64)
		Windows Server 2008 R2 SP1 (x64)
		Windows Server 2008 SP2 (x86 and x64)

 - Hardware Requirements:

		1 GHz or faster processor
		512 MB of RAM
		850 MB of available hard disk space (x86)
		2 GB hard drive (x64)

Note: Windows 8 and Windows Server 2012 include the .NET Framework 4.5. Therefore, you don't have to install this software on those operating systems.

Library structure
==========================

Currently, the client supports API v2 and all M2X API documents can be found at [M2X API Documentation](https://m2x.att.com/developer/documentation/v2/overview).
All classes are located within ATTM2X namespace.

* [M2XClient](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XClient.cs): This is the library's main entry point. In order to communicate with the M2X API you need an instance of this class. The constructor signature includes two (2) parameters:

 apiKey - mandatory parameter. You can find it in your M2X [Account page](https://m2x.att.com/account#master-keys-tab)
Read more about M2X API keys in the [API Keys](https://m2x.att.com/developer/documentation/v2/overview#API-Keys) section of the [M2X API Documentation](https://m2x.att.com/developer/documentation/v2/overview).

 m2xApiEndPoint - optional parameter. You don't need to pass it unless you want to connect to a different API endpoint.

 Client class provides access to API calls returning lists of the following API objects: devices, distributions, keys, charts.

 - Get the list of all your keys:

			var client = new M2XClient("[API Key]");
			var response = client.Keys().Result;
			var keys = response.Json<KeyList>();
			Console.WriteLine("Number of keys = " + keys.keys.Count);

 There are also a number of methods allowing you to get an instance of individual API object by providing its id or name as a parameter:

 - Get an instance of a device:

			M2XDevice device = client.Device("[Device id]");
			response = device.Location().Result;
			var location = response.Json<LocationDetails>();

 All M2X* classes are thread safe.
 Refer to the documentation on each class for further usage instructions.

* [M2XResponse](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/M2XResponse.cs)

 All API responses are wrapped in M2XResponse object.

* [Enums](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/Enums.cs)

 This file contains all enumerable values that can be used in the API calls.

* [Classes](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X/Classes)

 This folder contains the most classes for parameters to be used and received within the API calls.

Tests project
==========================

 The [ATTM2X.Tests](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X.Tests/M2XClientTests.cs) included into ATTM2X solution has a lot of examples for the most of M2X API methods.

 In order to run these tests you should put Master Key of your M2X account into [configuration file](https://github.com/attm2x/m2x-dot-net/blob/master/ATTM2X/ATTM2X.Tests/App.config).

Versioning
==========================

This library aims to adhere to [Semantic Versioning 2.0.0](http://semver.org/). As a summary, given a version number MAJOR.MINOR.PATCH:

MAJOR will increment when backwards-incompatible changes are introduced to the client.
MINOR will increment when backwards-compatible functionality is added.
PATCH will increment with backwards-compatible bug fixes.
Additional labels for pre-release and build metadata are available as extensions to the MAJOR.MINOR.PATCH format.

Note: the client version does not necessarily reflect the version used in the AT&T M2X API.

License
==========================

This library is provided under the MIT license. See [LICENSE](https://github.com/attm2x/m2x-dot-net/blob/master/LICENSE) for applicable terms.
