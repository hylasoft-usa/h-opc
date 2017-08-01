h-opc [![Build status](https://ci.appveyor.com/api/projects/status/oajkgccisoe98gip?svg=true)](https://ci.appveyor.com/project/Hyla-Soft-Inc/h-opc) [![NuGet Status](http://img.shields.io/nuget/v/H.Opc.svg)](https://www.nuget.org/packages/H.Opc) [![Coverage Status](https://coveralls.io/repos/github/jmbeach/h-opc/badge.svg?branch=master)](https://coveralls.io/github/jmbeach/h-opc?branch=master)
==============

An Opc Library and a command line to perform OPC operations with ease and transparency among different protocols. Currently supports synchronous operation over UA and DA protocols.

## Table of Contents
* [Use](#use)
* [Documentation](#documentation)
  * [Exploring the nodes](#exploring-the-nodes)
  * [Read a node](#read-a-node)
  * [Writing to a node](#writing-to-a-node)
  * [Monitoring a tag](#monitoring-a-tag)
  * [Go Asynchronous!](#go-asynchronous)
* [Command line](#command-line)
* [Build + Contribute](#build--contribute)
  * [Unit Testing](#unit-testing)
    * [UA](#ua)
    * [DA](#da)
* [Disclaimer](#disclaimer)
* [Roadmap](#roadmap)


## Use

A [nuget package](https://www.nuget.org/packages/H.Opc/) is available for the library. To install `H.Opc`, run the following command in the Package Manager Console:

    PM> Install-Package H.Opc

*NOTE: Package was moved on NuGet.org from Hylasoft.Opc to H.Opc because of NuGet account issues*

To install the command line interface, head to the [`release section`](https://github.com/hylasoft-usa/h-opc/releases).

## Documentation

to use the UA Client simply...

````cs
using (var client = new UaClient(new Uri("opc.tcp://host-url")))
{
  client.Connect();
  // Use `client` here
}
````

or with options...

````cs
var options = new Opc.Ua.UserIdentity("<your-username>", "<your-password>");
using (var client = new UaClient(new Uri("opc.tcp://host-url")), options)
{
  client.Connect();
  // Use `client` here
}
````


and to use the DA Client instead:

````cs
using (var client = new DaClient(new Uri("opcda://host-url")))
{
  client.Connect();
  // Use `client` here
}
````

#### Exploring the nodes

You can get a reference to a node with...

````cs
var node = client.FindNode("path.to.my.node");
````

This will get you a reference to the node `node` in the folder `path.to.my`.

You can use the node reference to explore the hieriarchy of nodes with the properties `Parent` and `SubNodes`. For example...

````cs
Node parentNode = node.Parent;
IEnumerable<Node> children = client.ExploreFolder(node.Tag);
IENumerable<Node> grandChildren = children.SelectMany(m => client.ExploreFolder(m.Tag));
````

#### Read a node

Reading a variable? As simple as...

````cs
var myString = client.Read<string>("path.to.string");
var myInt = client.Read<int>("path.to.num");
````

The example above will read a string from the tags `string` and `num` in the folder `path.to`

#### Writing to a node

To write a value just...

````cs
client.Write("path.to.string", "My new value");
client.Write("path.to.num", 42);
````

#### Monitoring a tag

Dead-simple monitoring:

````cs
client.Monitor<string>("path.to.string", (newValue, unsubscribe) =>
{
  DoSomethingWithYourValue(newValue);
  if(ThatsEnough == true)
    unsubscribe();
});

````

The second parameter is an `Action<T, Action>` that has two parameter:

- `newValue` is the new value of the tag
- `unsubscribe` is a function that unsubscribes the current monitored item. It's very handy when you want to terminate your callback

it's **important** that you either enclose the client into a `using` statement or call `Dispose()` when you are finished, to unsubscribe all the monitored items and terminate the connection!

### Go Asynchronous!

Each method as an asynchornous counterpart that can be used with the async/await syntax. The asynchronous syntax is **recommended** over the synchronous one (maybe the synchronous one will be deprecated one day).

## Command line

You can also use the command line interface project to quickly test your an OPC. Build the `h-opc-cli` project or download it from the `release` page of this repository, then run:

````
h-opc-cli.exe [OpcType] [server-url]
````

Where `OpcType` is the type of opc to use (e.g: "UA", "DA"). Once the project is running, you can use the internal command to manipulate the variable. To have more information aboute the internal commands, type `help` or `?`

## Build + Contribute

The repository uses [cs-boilerplate](https://github.com/hylasoft-usa/cs-boilerplate). Read the readme of the cs-boilerplate repository to understand how to build, run tasks and commit your work to `master`.

### Unit Testing

+ The unit tests rely on locally running simulator OPC servers. The ones used in this project are [OPC Foundation's Sample Server](https://opcfoundation.org/developer-tools/developer-kits-unified-architecture/sample-applications)
and [Graybox Simulator](http://gray-box.net/download_graysim.php?lang=en)
  + You must download OPC Foundation's Sample Server from the OPC Foundation website (link above), but GrayBox can be downloaded using [Chocolatey](https://chocolatey.org/)
    + `choco install grayboxsimulator`
  + OPC Foundation's Sample Server requires you register with the website before you can download.
+ The tests use [NUnit](https://github.com/nunit/nunit). To run them in Visual Studio, install the [NUnit 3 Test Adapter](https://marketplace.visualstudio.com/items?itemName=NUnitDevelopers.NUnit3TestAdapter)

#### UA
+ Open OPC Foundation's Sample Client (under Start -> OPC Foundation -> UA x.xx -> Sample Applications -> Opc.Ua.SampleClient.exe)
  + This will start the server too
  + Running tests will only work with this program open

#### DA
+ With Graybox Simulator installed, tests should automatically work

## Disclaimer

The following binaries belong to the [OPC Foundation](https://opcfoundation.org/). You must become a registered user in order to use them:

- `OPC.Ua.Client.dll`
- `OPC.Ua.Core.dll`
- `OPC.Ua.Configuration.dll`
- `OpcComRcw.dll`
- `OpcNetApi.Com.dll`
- `OpcNetApi.dll`

You must agree to the terms and condition exposed on the OPC Foundation website. Hyla Soft is not responsible of their usage and cannot be held responsible.

## Roadmap

- [ ] Add promise-based asynchronous calls
