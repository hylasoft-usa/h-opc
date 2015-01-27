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
    /// <summary>
    /// Initialize a new instance of the OpcException class
    /// </summary>
    public OpcException()
    {
    }

    /// <summary>
    /// Initialize a new instance of the OpcException class
    /// </summary>
    public OpcException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initialize a new instance of the OpcException class
    /// </summary>
    public OpcException(string message, Exception inner)
      : base(message, inner)
    {
    }

    /// <summary>
    /// Initialize a new instance of the OpcException class
    /// </summary>
    protected OpcException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}