using System;
using System.Collections.Generic;
using System.Linq;
using Hylasoft.Opc.Common;

namespace Hylasoft.Opc.Cli
{
  internal class Repl
  {
    private readonly IClient<Node> _client;
    private Node _currentNode;
    private bool _keepAlive = true;

    public Repl(IClient<Node> client)
    {
      _client = client;
      _currentNode = client.RootNode;
    }

    /// <summary>
    /// Starts the REPL routing
    /// </summary>
    public void Start()
    {
      while (_keepAlive)
      {
        try
        {
          Console.WriteLine();
          Console.Write(_currentNode.Tag + ": ");
          var line = Console.ReadLine();
          var command = CreateCommand(line);
          RunCommand(command);
        }
        catch (BadCommandException)
        {
          Console.WriteLine("Invalid command or arguments");
          Console.WriteLine();
          RunCommand(new Command(SupportedCommands.Help));
        }
        catch (Exception e)
        {
          Console.WriteLine("An error occurred running the last command:");
          Console.WriteLine(e.Message);
        }
      }
      // ReSharper disable once FunctionNeverReturns
    }

    private void RunCommand(Command command)
    {
      switch (command.Cmd)
      {
        case SupportedCommands.Help:
          ShowHelp();
          break;
        case SupportedCommands.Read:
          Read(command.Args);
          break;
        case SupportedCommands.Write:
          Write(command.Args);
          break;
        case SupportedCommands.Ls:
          ShowSubnodes();
          break;
        case SupportedCommands.Root:
          _currentNode = _client.RootNode;
          break;
        case SupportedCommands.Up:
          _currentNode = _currentNode.Parent ?? _client.RootNode;
          break;
        case SupportedCommands.Monitor:
          Monitor(command.Args);
          break;
        case SupportedCommands.Cd:
          Cd(command.Args);
          break;
        case SupportedCommands.Exit:
          _client.Dispose();
          _keepAlive = false;
          break;
        default:
          throw new BadCommandException();
      }
    }

    #region commands
    private void Cd(IList<string> args)
    {
      if (!args.Any())
        throw new BadCommandException();
      _currentNode = _client.FindNode(GenerateRelativeTag(args[0]));
    }

    private void Write(IList<string> args)
    {
      if (args.Count < 2)
        throw new BadCommandException();
      var tag = args[0];
      var val = args[1];
      var type = _client.GetDataType(GenerateRelativeTag(tag));
      switch(type.Name) {
        case "Int32":
          var val32 = Convert.ToInt32(val);
          _client.Write<int>(GenerateRelativeTag(tag), val32);
          break;
        case "Int16":
          var val16 = Convert.ToInt16(val);
          _client.Write<Int16>(GenerateRelativeTag(tag), val16);
          break;
        case "UInt16":
          var valuint16 = Convert.ToUInt16(val);
          _client.Write<UInt16>(GenerateRelativeTag(tag), valuint16);
          break;
        case "UInt32":
          var valuint32 = Convert.ToUInt32(val);
          _client.Write<UInt32>(GenerateRelativeTag(tag), valuint32);
          break;
        case "Boolean":
          var valBool = Convert.ToBoolean(val);
          _client.Write<Boolean>(GenerateRelativeTag(tag), valBool);
          break;
        case "Int64":
          var val64 = Convert.ToInt64(val);
          _client.Write<Int64>(GenerateRelativeTag(tag), val64);
          break;
        case "UInt64":
          var valuint64 = Convert.ToUInt64(val);
          _client.Write<UInt64>(GenerateRelativeTag(tag), valuint64);
          break;
        default:
          _client.Write<object>(GenerateRelativeTag(tag), val);
          break;
      }
    }

    private void Monitor(IList<string> args)
    {
      if (!args.Any())
        throw new BadCommandException();
      var stopped = false;
      _client.Monitor<object>(GenerateRelativeTag(args[0]), (o, stop) =>
      {
        // ReSharper disable once AccessToModifiedClosure
        if (stopped)
          stop();
        else
          Console.WriteLine("Value changed: " + o);
      });
      Console.WriteLine("Started monitoring. Press any key to interrupt.");
      Console.ReadKey(true);
      stopped = true;
    }

    private void Read(IList<string> args)
    {
      if (!args.Any())
        throw new BadCommandException();
      var value = _client.Read<object>(GenerateRelativeTag(args[0]));
      Console.WriteLine(value);
    }

    private static void ShowHelp()
    {
      Console.WriteLine("Supported commands:");
      Console.WriteLine("  ls: Display the subnodes");
      Console.WriteLine("  cd [tag]: Visit a children node");
      Console.WriteLine("  read [tag]: Read the node");
      Console.WriteLine("  write [tag] [value]: Write value on node");
      Console.WriteLine("  root: Go to root node");
      Console.WriteLine("  up: Go up one folder");
      Console.WriteLine("  monitor [node]: monitor the node");
      Console.WriteLine("subnodes are separated by '.' The tag is relative to the current folder");
    }

    private void ShowSubnodes()
    {
      var nodes = _client.ExploreFolder(_currentNode.Tag);
      if (nodes == null || !nodes.Any())
        Console.WriteLine("no subnodes");
      else foreach (var node in nodes)
        Console.WriteLine(node.Name);
    }
    #endregion

    private static Command CreateCommand(string line)
    {
      try
      {
        var cmd = CliUtils.SplitArguments(line);
        var args = cmd.Skip(1).ToList();
        SupportedCommands selectedCommand;
        if (!Enum.TryParse(cmd[0], true, out selectedCommand))
          selectedCommand = SupportedCommands.Help;
        return new Command(selectedCommand, args);
      }
      catch (Exception e)
      {
        throw new BadCommandException(e.Message, e);
      }
    }

    private string GenerateRelativeTag(string relativeTag)
    {
      var node = _client.ExploreFolder(_currentNode.Tag)
        .SingleOrDefault(n => n.Name == relativeTag);
      return node == null ? relativeTag : node.Tag;
    }
  }
}