using System;
using System.Runtime.Serialization;

namespace Hylasoft.Opc.Cli
{
  /// <summary>
  /// Thrown when the provided command is bad
  /// </summary>
  [Serializable]
  public class BadCommandException : Exception
  {
    public BadCommandException()
    {
    }

    public BadCommandException(string message)
      : base(message)
    {
    }

    public BadCommandException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected BadCommandException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}