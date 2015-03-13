using System.Collections.Generic;

namespace Hylasoft.Opc.Cli
{
  /// <summary>
  /// Represent a command
  /// </summary>
  public class Command
  {
    public Command(SupportedCommands cmd, IList<string> args = null)
    {
      Cmd = cmd;
      Args = args ?? new string[0];
    }

    public SupportedCommands Cmd { get; private set; }

    public IList<string> Args { get; private set; }
  }
}