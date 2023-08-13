using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace LoggingAuto.Interceptors;

public class LoggingInterceptor : IInterceptor
{
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(LoggingInterceptor));
    public void Intercept(IInvocation invocation)
    {
        // Check if the method has the NoIOLoggingAttribute
        if (HasNoIOLoggingAttribute(invocation))
        {
            // _log.Information($"Executing with NoIOLoggingAttribute: {invocation.Method.Name}");
            // Execute the original method
            invocation.Proceed();
        }
        else
        {
            LogInput(invocation);
            // Execute the original method
            invocation.Proceed();

            // Log return value
            LogOutput(invocation);
        }
    }
    private void LogInput(IInvocation invocation){
        // var methodName = invocation.Method.Name;
        // var arguments = string.Join(", ", invocation.Arguments);
        // _log.Information($"Calling method: {methodName} with arguments: {arguments}");
        var className = invocation.TargetType.Name;
        string methodName = invocation.Method.Name;
        IEnumerable<string?>? argumentNames = invocation.Method.GetParameters().Select(p => p.Name);
        object[] argumentValues = invocation.Arguments;
        // Convert argument names and values to a dictionary for logging
        var arguments = argumentNames.Zip(argumentValues, (name, value) => new { Name = name, Value = value });

        // Log the method name and arguments
        _log.Information($"Calling method {className}.{methodName}");
        // _log.Information("Arguments: {arguments}", arguments);
        string logStr = "Arguments:";
        foreach(var a in arguments){
            logStr += $"{a.Name}={a.Value}, ";
        } 
        _log.Information(logStr);
    }
    private void LogOutput(IInvocation invocation){
        var className = invocation.TargetType.Name;
        var methodName = invocation.Method.Name;
        var returnValue = invocation.ReturnValue;
            _log.Information($"Method {className}.{methodName} returned: {returnValue}");
    }
    private bool HasNoIOLoggingAttribute(IInvocation invocation){
        bool ans = false;
        Type targetType = invocation.TargetType;
        var targetMethod = targetType.GetMethod(invocation.Method.Name);

        if (targetMethod is not null && targetMethod.GetCustomAttributes(typeof(NoIOLoggingAttribute), true).Any()){
            _log.Information($"Method has NoIOLoggingAttribute");
            ans = true;
        }
        return ans;
    }
}