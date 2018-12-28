using System;
using System.IO;
using Engine.Core;
using NotImplementedException = System.NotImplementedException;

namespace Engine
{
    public enum LogSeverity
    {
        Info,
        Warn,
        Error
    }

    public class Logger : EngineSystem
    {

        public LogSeverity LogSeverity { get; set; }

        private TextWriter output;

        public Logger(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            LogSeverity = LogSeverity.Error;
            output = Console.Out;
        }

        public override void Init()
        {
            
        }

        public void Info(string message)
        {
            if (LogSeverity >= LogSeverity.Info)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                output.WriteLine($"[INFO] {message}");
                output.Flush();
                Console.ResetColor();
            }
        }

        public void Warn(string message)
        {
            if (LogSeverity >= LogSeverity.Warn)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                output.WriteLine($"[WARN] {message}");
                output.Flush();
                Console.ResetColor();
            }
        }

        public void Error(string message)
        {
            if (LogSeverity >= LogSeverity.Error)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                output.WriteLine($"[ERROR] {message}");
                output.Flush();
                Console.ResetColor();
            }
        }
    }
}