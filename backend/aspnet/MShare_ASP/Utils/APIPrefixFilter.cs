using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using MShare_ASP.Configurations;

namespace MShare_ASP.Utils {
    internal class APIPrefixFilter : IDocumentFilter {
		private IURIConfiguration _uriConf;
		public APIPrefixFilter(IURIConfiguration uriConf){
			_uriConf = uriConf;
		}
		
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context) {
            if (System.Environment.GetEnvironmentVariable("MSHARE_RUNNING_BEHIND_PROXY") == "true")
                swaggerDoc.BasePath = _uriConf.SwaggerProxyUri;
        }
    }
}
