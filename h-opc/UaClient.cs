using System;

namespace Hylasoft.Opc
{
    public class UaClient : IClient
    {
      public void Connect()
      {
        throw new NotImplementedException();
      }

      public OpcStatus Status { get; set; }

      public T Read<T>(string tag)
      {
        throw new NotImplementedException();
      }

      public void Write<T>(string tag, T item)
      {
        throw new NotImplementedException();
      }

      public void Monitor<T>(string tag, Action<T> callback)
      {
        throw new NotImplementedException();
      }
    }
}
