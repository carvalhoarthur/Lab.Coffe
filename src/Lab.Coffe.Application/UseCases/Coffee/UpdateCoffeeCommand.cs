using Lab.Coffe.Application.DTOs;
using Lab.Coffe.Application.Interfaces;
using MediatR;

namespace Lab.Coffe.Application.UseCases.Coffee;

public class UpdateCoffeeCommand : IRequest<CoffeeDto>
{
    public Guid Id { get; set; }
    public UpdateCoffeeRequest Request { get; set; }

    public UpdateCoffeeCommand(Guid id, UpdateCoffeeRequest request)
    {
        Id = id;
        Request = request;
    }
}

public class UpdateCoffeeCommandHandler : IRequestHandler<UpdateCoffeeCommand, CoffeeDto>
{
    private readonly IRepository<Domain.Entities.Coffee> _repository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly AutoMapper.IMapper _mapper;

    public UpdateCoffeeCommandHandler(
        IRepository<Domain.Entities.Coffee> repository,
        IMessagePublisher messagePublisher,
        AutoMapper.IMapper mapper)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    public async Task<CoffeeDto> Handle(UpdateCoffeeCommand request, CancellationToken cancellationToken)
    {
        var coffee = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (coffee == null)
            throw new KeyNotFoundException($"Coffee with ID {request.Id} not found");

        coffee.UpdateName(request.Request.Name);
        coffee.UpdatePrice(request.Request.Price);
        coffee.UpdateStock(request.Request.Stock);

        await _repository.UpdateAsync(coffee, cancellationToken);

        // Publicar mensagem no RabbitMQ
        await _messagePublisher.PublishAsync(
            new { CoffeeId = coffee.Id, Name = coffee.Name, Action = "Updated" },
            "coffee.updated",
            cancellationToken);

        return _mapper.Map<CoffeeDto>(coffee);
    }
}
