using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace Infrastructure.DomainEvents;

internal sealed class DomainEventsDispatcher(IServiceProvider serviceProvider) : IDomainEventsDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeDictionary = new();

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            using IServiceScope scope = serviceProvider.CreateScope();

            Type domainEventType = domainEvent.GetType();
            Type handlerType = HandlerTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(IDomainEventHandler<>).MakeGenericType(et));

            // since handlers when retrieved from DI are objects 
            // and c#  does not allow using runtime types in generics directly,
            // objects have only these methods:
            // - GetType()
            // - GetHashCode()
            // - Equals()
            //..
            // we cant call handler.handle directly 
            // and we cannot cast it handler = (IDomainEventHandler<domainEventType>)handler;
            IEnumerable<object?> handlers = scope.ServiceProvider.GetServices(handlerType);
            // so we are left with using reflection to invoke the method
            // the compiler needs to know the type at compile time, not runtime
            // to avoid changes in the type system at runtime

            /*** exp :
                * string message = "Hello World";
                * Type type = message.GetType(); // string
                * type message ;  // ERROR! 'type' is a variable but is used like a type.
                    ==: ILLEGAL: The compiler rejects this. It cannot use a variable 
                    // as a type in this way.

            ***/

            // this casting doesnt work :  (IDomainEventHandler<domainEventType>)handler
            // cuz "domainEventType" is a variable , we cannot use it directly in the generics 

            foreach (object? handler in handlers)
            {
                if (handler is null)
                {
                    continue;
                }

                var handlerWrapper = HandlerWrapper.Create(handler, domainEventType);

                await handlerWrapper.Handle(domainEvent, cancellationToken);
            }
        }
    }

    private abstract class HandlerWrapper
    {
        public abstract Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken);

        public static HandlerWrapper Create(object handler, Type domainEventType)
        {
            // handlerWrapper<UserRegisteredEvent> 
            Type wrapperType = WrapperTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(HandlerWrapper<>).MakeGenericType(et));


            return (HandlerWrapper)Activator.CreateInstance(wrapperType, handler);
        }
    }

    private sealed class HandlerWrapper<T>(object handler) : HandlerWrapper where T : IDomainEvent
    {
        // T now is know at compile time , after the generic type is created ( userRegisteredEvent )
        // so we can cast the handler to IDomainEventHandler<T>
        // and call the Handle method directly 
        private readonly IDomainEventHandler<T> _handler = (IDomainEventHandler<T>)handler;

        public override async Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await _handler.Handle((T)domainEvent, cancellationToken);
        }
    }
}
