using System;
using System.Collections.Generic;
#pragma warning disable CA1873

using System.Text;
using Buy2.Application.Abstractions.Messaging;
using Buy2.Domain.SharedKernel;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Buy2.Application.Abstractions.Decorators;

internal static class LoggingDecorator
{
    // Command handler decorator without response
    internal sealed class CommandHandler<TCommand>(
            ILogger<CommandHandler<TCommand>> logger,
            ICommandHandler<TCommand> innerHandler
        ) : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> HandleAsync(TCommand command)
        {
            string commandTypeName = typeof(TCommand).Name;
            logger.LogInformation("Handling command of type {CommandType}", commandTypeName);
            Result result = await innerHandler.HandleAsync(command);
            if (result.IsSuccess)
            {
                logger.LogInformation("Command of type {CommandType} handled successfully", commandTypeName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Command of type {CommandType} failed with error", commandTypeName);
                }
            }
            return result;
        }
    }

    // Command handler decorator with response
    internal sealed class CommandHandler<TCommand, TResponse>(
            ILogger<CommandHandler<TCommand, TResponse>> logger,
            ICommandHandler<TCommand, TResponse> innerHandler
        ) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TCommand command)
        {
            string commandTypeName = typeof(TCommand).Name;
            logger.LogInformation("Handling command of type {CommandType}", commandTypeName);
            Result<TResponse> result = await innerHandler.HandleAsync(command);
            if (result.IsSuccess)
            {
                logger.LogInformation("Command of type {CommandType} handled successfully", commandTypeName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Command of type {CommandType} failed with error", commandTypeName);
                }
            }
            return result;
        }
    }

    // Query handler decorator
    internal sealed class QueryHandler<TQuery, TResponse>(
            ILogger<QueryHandler<TQuery, TResponse>> logger,
            IQueryHandler<TQuery, TResponse> innerHandler
        ) : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TQuery query)
        {
            string queryTypeName = typeof(TQuery).Name;

            logger.LogInformation("Handling query of type {QueryType}", queryTypeName);

            Result<TResponse> result = await innerHandler.HandleAsync(query);

            if (result.IsSuccess)
            {
                logger.LogInformation("Query of type {QueryType} handled successfully", queryTypeName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Query of type {QueryType} failed with error", queryTypeName);
                }
            }

            return result;
        }
    }
}
