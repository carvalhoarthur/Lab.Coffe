using Lab.Coffe.Application.DTOs;
using Lab.Coffe.Application.Interfaces;
using MediatR;

namespace Lab.Coffe.Application.UseCases.Coffee;

public class GetCoffeeByIdQuery : IRequest<CoffeeDto?>
{
    public Guid Id { get; set; }

    public GetCoffeeByIdQuery(Guid id)
    {
        Id = id;
    }
}

public class GetCoffeeByIdQueryHandler : IRequestHandler<GetCoffeeByIdQuery, CoffeeDto?>
{
    private readonly IRepository<Domain.Entities.Coffee> _repository;
    private readonly AutoMapper.IMapper _mapper;

    public GetCoffeeByIdQueryHandler(
        IRepository<Domain.Entities.Coffee> repository,
        AutoMapper.IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CoffeeDto?> Handle(GetCoffeeByIdQuery request, CancellationToken cancellationToken)
    {
        var coffee = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return coffee == null ? null : _mapper.Map<CoffeeDto>(coffee);
    }
}
