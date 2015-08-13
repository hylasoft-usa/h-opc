using Opc.Ua;
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
    /// Returns an (optional) associated OPC UA StatusCode for the exception.
    /// </summary>
    public StatusCode? Status { get; private set; }

    /// <summary>
    /// Initialize a new instance of the OpcException class
    /// </summary>
    public OpcException(string message, StatusCode status)
      : base(message)
    {
      Status = status;
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