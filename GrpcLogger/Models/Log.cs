using System;

namespace GrpcLogger.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}
