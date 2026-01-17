using Lab.Coffe.Application.DTOs;
using Lab.Coffe.Application.Interfaces;
using CoffeeEntity = Lab.Coffe.Domain.Entities.Coffee;
using MediatR;

namespace Lab.Coffe.Application.UseCases.Coffee;

public class CreateCoffeeCommand : IRequest<CoffeeDto>
{
    public CreateCoffeeRequest Request { get; set; }

    public CreateCoffeeCommand(CreateCoffeeRequest request)
    {
        Request = request;
    }
}

public class CreateCoffeeCommandHandler : IRequestHandler<CreateCoffeeCommand, CoffeeDto>
{
    private readonly IRepository<CoffeeEntity> _repository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly AutoMapper.IMapper _mapper;

    public CreateCoffeeCommandHandler(
        IRepository<CoffeeEntity> repository,
        IMessagePublisher messagePublisher,
        AutoMapper.IMapper mapper)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    public async Task<CoffeeDto> Handle(CreateCoffeeCommand request, CancellationToken cancellationToken)
    {
        var coffee = _mapper.Map<CoffeeEntity>(request.Request);
        var createdCoffee = await _repository.AddAsync(coffee, cancellationToken);

        // Publicar mensagem no RabbitMQ
        await _messagePublisher.PublishAsync(
            new { CoffeeId = createdCoffee.Id, Name = createdCoffee.Name, Action = "Created" },
            "coffee.created",
            cancellationToken);

        return _mapper.Map<CoffeeDto>(createdCoffee);
    }
}
