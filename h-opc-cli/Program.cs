using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Hylasoft.Opc.Common;
using Hylasoft.Opc.Common.Nodes;
using Hylasoft.Opc.Ua;

namespace Hylasoft.Opc.Cli
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        var assembly = Assembly.GetExecutingAssembly();
        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        var version = fvi.FileVersion;
        Console.WriteLine("h-opc-cli v" + version);
        Initialize(args);
      }
      catch (Exception e)
      {
        // GLOBAL EXCEPTION HANDLER
        Console.WriteLine("The application ended unexpectedly.");
        Console.WriteLine(e);
        Console.WriteLine("To file an issue, visit http://github.com/hylasoft-usa/h-opc/issues");
      }
    }

    private static void Initialize(string[] args)
    {
      if (args.Count() != 2)
      {
        Console.WriteLine("Usage: h-opc-cli [Type] [serverurl]");
        Console.WriteLine("Supported types: " + GetSupportedTypes());
        return;
      }
      SupportedTypes type;
      try
      {
        type = GetOpcType(args[0]);
      }
      catch (ArgumentException)
      {
        Console.WriteLine(args[0] + " is not a supported type");
        Console.WriteLine("Supported types: " + GetSupportedTypes());
        return;
      }
      IClient<Node> client;
      try
      {
        client = GetClient(args[1], type);
        client.Connect();
      }
      catch (Exception)
      {
        Console.WriteLine("An error occured when trying connecting to the server");
        throw;
      }
      var repl = new Repl(client);
      repl.Start();
    }

    private static SupportedTypes GetOpcType(string s)
    {
      switch (s.ToUpper())
      {
        case "UA": return SupportedTypes.Ua;
        default: throw new ArgumentException("Type not supported");
      }
    }

    private static string GetSupportedTypes()
    {
      return string.Join(", ", Enum.GetNames(typeof(SupportedTypes)));
    }

    private static IClient<Node> GetClient(string url, SupportedTypes type)
    {
      switch (type)
      {
        case SupportedTypes.Ua:
          return new UaClient(new Uri(url));
        default:
          throw new ArgumentOutOfRangeException("type");
      }
    }
  }

  public enum SupportedTypes
  {
    Ua
  }
}
