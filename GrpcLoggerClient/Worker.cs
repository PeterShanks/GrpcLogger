using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GrpcLogger.Services;
using Microsoft.Extensions.Configuration;
using Grpc.Net.Client;
using LogLevel = GrpcLogger.Services.LogLevel;
using Grpc.Core;

namespace GrpcLoggerClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private Logger.LoggerClient _client = null;

        protected Logger.LoggerClient Client
        {
            get
            {
                if (_client == null)
                {
                    var channel = GrpcChannel.ForAddress(_config["Service:ServerUrl"]);
                    _client = new Logger.LoggerClient(channel);
                }

                return _client;
            }
        }

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var counter = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var request = new AddLogRequest
                {
                    LogLevel = LogLevel.Critical,
                    Message = $"{counter++}"
                };

                try
                {
                    var result = await Client.AddLogAsync(request);

                    if (result.Success)
                    {
                        _logger.LogInformation($"Successfully sent: {result.Message}");
                    }
                    else
                    {
                        _logger.LogInformation("Failed to send");
                    }
                }
                catch (RpcException ex)
                {
                    _logger.LogError($"Exception Thrown: {ex}");
                }
               

                await Task.Delay(_config.GetValue<int>("Service:DelayInterval"), stoppingToken);
            }
        }
    }
}
