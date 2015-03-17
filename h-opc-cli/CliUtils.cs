using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hylasoft.Opc.Cli
{
  internal static class CliUtils
  {
    /// <summary>
    /// Split arguments in a string taking in consideration quotes
    /// E.g: 'aaa bbb "cc cc"' => ['aaa', 'bbb', 'cc cc']
    /// </summary>
    public static IList<string> SplitArguments(string input)
    {
      return Regex.Split(input, @"(?:([^\s""]+)|""([^""]*)"")+")
        .Where(s => s.Any(c => c != ' '))
        .ToList();
    }
  }
}
