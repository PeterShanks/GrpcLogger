using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcLogger.Services
{
    public class LoggerService : Logger.LoggerBase
    {
        private readonly LoggerDbContext _dbContext;

        public LoggerService(LoggerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override async Task<StatusMessage> AddLog(AddLogRequest request, ServerCallContext context)
        {
            try
            {
                if (Convert.ToInt32(request.Message) % 3 == 0)
                {
                    var trailer = new Metadata()
                    {
                        {"BadValue", request.Message},
                        {"Field", nameof(request.Message)},
                        {"Message", "Message value cannot be devided by 3"}
                    };

                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Message value cannot be devided by 3"), trailer);
                }


                var log = new Models.Log
                {
                    CreationDate = DateTime.Now,
                    Message = request.Message,
                    LogLevel = (Models.LogLevel) request.LogLevel
                };
                await _dbContext.Logs.AddAsync(log);

                await _dbContext.SaveChangesAsync();

                return new StatusMessage
                {
                    Message = $"Log was added successfully with Id {log.Id}",
                    Success = true
                };
            }
            catch (RpcException)
            {
                throw;
            }
            catch
            {
                throw new RpcException(Status.DefaultCancelled, "An Error has occurred during the process");
            }
        }
    }
}
