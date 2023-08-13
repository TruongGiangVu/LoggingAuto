using System.Net;
using LoggingAuto.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace LoggingAuto.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(LoggingMiddleware));
    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        HttpRequest request = context.Request;
        string requestBody;
        // Check if the current endpoint has NoIOLoggingAttribute
        bool hasNoIOLoggingAttribute = this.HasNoIOLoggingAttribute(context);

        if (!hasNoIOLoggingAttribute) // do not have NoIOLoggingAttribute
        {
            using (var requestBodyStream = new MemoryStream())
            {
                await request.Body.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                requestBody = new StreamReader(requestBodyStream).ReadToEnd();
            }

            // Log the request information here
            _log.Information($"Request: {request.Method} {request.Path}{request.QueryString}");
            if (!string.IsNullOrEmpty(requestBody))
                _log.Information($"Request Body: {requestBody}");

            // Capture the response
            var originalResponseBody = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(responseBodyStream).ReadToEnd();

            // Log the response information here
            _log.Information($"Response Status: {context.Response.StatusCode}");
            _log.Information($"Response Body: {responseBody}");

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalResponseBody);
        }
        else // have NoIOLoggingAttribute
        {
            await _next(context);
        }
    }
    private bool IsAction(HttpContext context, out ControllerActionDescriptor? actionDescriptor)
    {
        bool ans = false;
        actionDescriptor = null;
        try
        {
            Endpoint? endpoint = context.GetEndpoint();
            actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (actionDescriptor is not null)
            {
                _log.Information($"------Action {actionDescriptor.ControllerName}.{actionDescriptor.ActionName}-------------");
                ans = true;
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex.ToString());
        }

        return ans;
    }
    private bool HasNoIOLoggingAttribute(HttpContext context)
    {
        bool ans = false;

        // Check if the current endpoint is an action
        ControllerActionDescriptor? actionDescriptor = null;
        if (this.IsAction(context, out actionDescriptor))
        {
            // Check for [NoIOLoggingAttribute]
            NoIOLoggingAttribute? NoIOLoggingAttribute = actionDescriptor?.MethodInfo.GetCustomAttributes(typeof(NoIOLoggingAttribute), true).FirstOrDefault() as NoIOLoggingAttribute;
            // Log action information along with NoIOLoggingAttribute attributes
            _log.Information($"NoIOLogging: {(NoIOLoggingAttribute != null ? "Yes" : "No")}");
            ans = NoIOLoggingAttribute is not null;
        }
        return ans;
    }
}