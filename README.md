h-opc [![Build status](https://ci.appveyor.com/api/projects/status/oajkgccisoe98gip/branch/master?svg=true)](https://ci.appveyor.com/project/hylasoft/h-opc/branch/master)
==============

An Opc Library and a command line to perform OPC operations with ease and transparency among different protocols. Currently supports synchronous operation over UA and DA protocols.

## Use

A [nuget package](https://www.nuget.org/packages/Hylasoft.Opc/) is available for the library. To install `Hylasoft.Opc`, run the following command in the Package Manager Console:

    PM> Install-Package Hylasoft.Opc
    
To install the command line interface, head to the [`release section`](https://github.com/hylasoft-usa/h-opc/releases).

## Usage

to use the UA Client simply...

````cs
using (var client = new UaClient(new Uri("opc.tcp://host-url")))
{
  // Use `client` here
}
````

and to use the DA Client instead:

````cs
using (var client = new DaClient(new Uri("opcda://host-url")))
{
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
IEnumerable<Node> children = node.SubNodes;
IENumerable<Node> grandChildren = children.SelectMany(m => m.SubNodes);
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

## Command line

You can also use the command line interface project to quickly test your an OPC. Build the `h-opc-cli` project or download it from the `release` page of this repository, then run:

````
h-opc-cli.exe [OpcType] [server-url]
````

Where `OpcType` is the type of opc to use (e.g: "UA", "DA"). Once the project is running, you can use the internal command to manipulate the variable. To have more information aboute the internal commands, type `help` or `?`

## Build + Contribute

The repository uses [cs-boilerplate](https://github.com/hylasoft-usa/cs-boilerplate). Read the readme of the cs-boilerplate repository to understand how to build, run tasks and commit your work to `master`.

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
