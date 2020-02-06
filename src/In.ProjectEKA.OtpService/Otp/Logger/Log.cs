
namespace In.ProjectEKA.OtpService.Otp.Logger
{
    using System;
    using Serilog;
    
    public class Log
    {
        public static ILogger Logger
        {
            set => Serilog.Log.Logger = value;
        }

        public static void Information(string format, params object[] arg)
        {
            Serilog.Log.Information(format, arg);
        }
        
        public static void Error(string format, params object[] arg)
        {
            Serilog.Log.Error(format, arg);
        }
        
        public static void Error(Exception exception, string format, params object[] arg)
        {
            Serilog.Log.Error(exception, format, arg);
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
    }
}
