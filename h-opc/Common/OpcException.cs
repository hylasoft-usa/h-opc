using System;
using System.Runtime.Serialization;

namespace Hylasoft.Opc.Common
{
  /// <summary>
  /// Identifies an exception occurred during OPC Communication
  /// </summary>
  [Serializable]
  public class OpcException : Exception
  {
    public OpcException()
    {
    }

    public OpcException(string message)
      : base(message)
    {
    }

    public OpcException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected OpcException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}