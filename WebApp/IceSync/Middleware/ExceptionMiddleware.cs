using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net;

namespace IceSync.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        => this.next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = string.Empty;

            switch (exception)
            {
                case NullReferenceException _:
                    code = HttpStatusCode.BadRequest;
                    result = SerializeObject(new[] { "Invalid request." });
                    break;
                case HttpRequestException _:
                    code = HttpStatusCode.BadRequest;
                    result = SerializeObject(new[] { "Invalid external API request." });
                    break;
                default:
                    code = HttpStatusCode.BadRequest;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (string.IsNullOrEmpty(result))
            {
                var error = exception.Message;
                result = SerializeObject(new[] { error });
            }

            return context.Response.WriteAsync(result);
        }

        private static string SerializeObject(object obj)
        => JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(true, true)
            }
        });

    }
}
