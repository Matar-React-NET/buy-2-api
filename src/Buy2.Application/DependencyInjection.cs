using System;
using System.Collections.Generic;
using System.Text;
using Buy2.Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Buy2.Application.Abstractions.Decorators;

namespace Buy2.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .RegisterCQRSHandlers()
            .RegisterLoggingDecorator()
            .RegisterValidationDecorators();
    }

    private static IServiceCollection RegisterValidationDecorators(this IServiceCollection services)
    {
        return services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandHandler<>))
            .Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
    }

    private static IServiceCollection RegisterLoggingDecorator(this IServiceCollection services)
    {
        return services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandHandler<>))
            .Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>))
            .Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
    }

    private static IServiceCollection RegisterCQRSHandlers(this IServiceCollection services)
    {
        return services.Scan(scan =>
            scan.FromAssembliesOf(typeof(DependencyInjection))
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }
}


