using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using MShare_ASP.Configurations;

namespace MShare_ASP.Utils
{

    internal class APIPrefixFilter : IDocumentFilter
    {

        private IURIConfiguration UriConf { get; }

        public APIPrefixFilter(IURIConfiguration uriConf)
        {
            UriConf = uriConf;
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            if (System.Environment.GetEnvironmentVariable("MSHARE_RUNNING_BEHIND_PROXY") == "true")
                swaggerDoc.BasePath = UriConf.SwaggerProxyUri;
        }
    }
}
