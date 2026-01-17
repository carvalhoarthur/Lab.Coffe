namespace Lab.Coffe.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
