using System;

namespace Hylasoft.Opc.Ua
{

    /// <summary>
    /// This class defines the configuration options for the setup of the UA client session
    /// </summary>
    public class UaClientOptions
    {
        /// <summary>
        /// Specifies the ApplicationName for the client application. 
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Specifies the ConfigSectionName for the client configuration
        /// </summary>
        public string ConfigSectionName { get; set; }

        internal UaClientOptions()
        {
            ApplicationName = "h-opc-client";
            ConfigSectionName = "h-opc-client";
        }
    }
}
