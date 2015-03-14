using System;
using System.Collections.Generic;
using System.Linq;
using Hylasoft.Opc.Common;
using Hylasoft.Opc.Common.Nodes;

namespace Hylasoft.Opc.Cli
{
  internal class Repl
  {
    private readonly IClient<Node> _client;
    private Node _currentNode;

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
      while (true)
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
      _client.Write<object>(GenerateRelativeTag(args[0]), args[1]);
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
      var nodes = _currentNode.SubNodes;
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
      if (string.IsNullOrEmpty(_currentNode.Tag))
        return relativeTag;
      return _currentNode.Tag + '.' + relativeTag;
    }
  }
}