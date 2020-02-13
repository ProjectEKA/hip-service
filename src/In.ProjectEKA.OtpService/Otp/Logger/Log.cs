namespace In.ProjectEKA.OtpService.Otp.Logger
{
    using System;

    public static class Log
    {
        public static void Information(string format, params object[] arg)
        {
            Serilog.Log.Information(format, arg);
        }
        
        public static void Error(string format, params object[] arg)
        {
            Serilog.Log.Error(format, arg);
        }
        
        public static void Fatal(Exception exception, string format, params object[] arg)
        {
            Serilog.Log.Error(exception, format, arg);
        }
        
        public static void CloseAndFlush()
        {
            Serilog.Log.CloseAndFlush();
        }
        
        public static void Debug(string format, params object[] arg)
        {
            Serilog.Log.Information(format, arg);
        }
        
        public static void Information(params object[] arg)
        {
            Serilog.Log.Information(LogTemplate.InformationTemplate, arg);
        }
        
        public static void Error(params object[] arg)
        {
            Serilog.Log.Error(LogTemplate.ErrorTemplate, arg);
        }

        public static void Fatal(Exception exception, params object[] arg)
        {
            Serilog.Log.Error(exception, LogTemplate.ExceptionTemplate, arg);
        }
    }
}
