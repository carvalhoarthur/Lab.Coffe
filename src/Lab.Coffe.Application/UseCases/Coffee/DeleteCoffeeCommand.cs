using Lab.Coffe.Application.Interfaces;
using MediatR;

namespace Lab.Coffe.Application.UseCases.Coffee;

public class DeleteCoffeeCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteCoffeeCommand(Guid id)
    {
        Id = id;
    }
}

public class DeleteCoffeeCommandHandler : IRequestHandler<DeleteCoffeeCommand, bool>
{
    private readonly IRepository<Domain.Entities.Coffee> _repository;
    private readonly IMessagePublisher _messagePublisher;

    public DeleteCoffeeCommandHandler(
        IRepository<Domain.Entities.Coffee> repository,
        IMessagePublisher messagePublisher)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
    }

    public async Task<bool> Handle(DeleteCoffeeCommand request, CancellationToken cancellationToken)
    {
        var coffee = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (coffee == null)
            return false;

        await _repository.DeleteAsync(request.Id, cancellationToken);

        // Publicar mensagem no RabbitMQ
        await _messagePublisher.PublishAsync(
            new { CoffeeId = coffee.Id, Name = coffee.Name, Action = "Deleted" },
            "coffee.deleted",
            cancellationToken);

        return true;
    }
}
