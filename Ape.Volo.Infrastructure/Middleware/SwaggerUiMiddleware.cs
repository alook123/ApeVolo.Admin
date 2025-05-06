using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper.Serilog;
using Ape.Volo.Core;
using Ape.Volo.Core.ConfigOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Ape.Volo.Infrastructure.Middleware;

/// <summary>
/// Swagger UI 中间件
/// </summary>
public static class SwaggerUiMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(SwaggerUiMiddleware));

    public static void UseSwaggerUiMiddleware(this IApplicationBuilder app, Func<Stream> streamHtml)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));
        var swaggerOptions = App.GetOptions<SwaggerOptions>();
        if (swaggerOptions.Enabled)
        {
            //app.UseSwagger();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((doc, item) =>
                {
                    //根据代理服务器提供的协议、地址和路由，生成api文档服务地址
                    doc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer
                            { Url = $"{item.Scheme}://{item.Host.Value}" }
                    };
                });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{swaggerOptions.Name}/swagger.json",
                    swaggerOptions.Version);
                c.RoutePrefix = "swagger/api";

                var stream = streamHtml?.Invoke();
                if (stream == null)
                {
                    const string msg = "index.html attribute error";
                    Logger.Error(msg);
                    throw new Exception(msg);
                }

                c.IndexStream = streamHtml;
            });
        }
    }
}
