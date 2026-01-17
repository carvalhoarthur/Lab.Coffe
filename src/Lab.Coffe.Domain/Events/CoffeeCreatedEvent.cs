using Lab.Coffe.Domain.Entities;

namespace Lab.Coffe.Domain.Events;

public class CoffeeCreatedEvent : IDomainEvent
{
    public Coffee Coffee { get; }
    public DateTime OccurredOn { get; }

    public CoffeeCreatedEvent(Coffee coffee)
    {
        Coffee = coffee;
        OccurredOn = DateTime.UtcNow;
    }
}
