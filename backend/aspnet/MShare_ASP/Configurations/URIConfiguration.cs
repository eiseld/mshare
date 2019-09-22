using System;

namespace MShare_ASP.Configurations
{

    /// <summary>URI related configurations go here</summary>
    public interface IURIConfiguration
    {

        /// <summary>URI that the end user sees (like in an email)</summary>
        String URIForEndUsers { get; }

        /// <summary>Swagger URI to use when behind proxy</summary>
        String SwaggerProxyUri { get; }
    }

    internal class URIConfiguration : IURIConfiguration
    {

        public String URIForEndUsers { get; set; }

        public String SwaggerProxyUri { get; set; }
    }
}
