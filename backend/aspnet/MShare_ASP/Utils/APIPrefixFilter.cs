using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MShare_ASP.Utils {
    internal class APIPrefixFilter : IDocumentFilter {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context) {
            if (System.Environment.GetEnvironmentVariable("MSHARE_RUNNING_BEHIND_PROXY") == "true")
                swaggerDoc.BasePath = "/api";
        }
    }
}
