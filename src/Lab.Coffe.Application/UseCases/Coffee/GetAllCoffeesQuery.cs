using Lab.Coffe.Application.DTOs;
using Lab.Coffe.Application.Interfaces;
using MediatR;

namespace Lab.Coffe.Application.UseCases.Coffee;

public class GetAllCoffeesQuery : IRequest<IEnumerable<CoffeeDto>>
{
}

public class GetAllCoffeesQueryHandler : IRequestHandler<GetAllCoffeesQuery, IEnumerable<CoffeeDto>>
{
    private readonly IRepository<Domain.Entities.Coffee> _repository;
    private readonly AutoMapper.IMapper _mapper;

    public GetAllCoffeesQueryHandler(
        IRepository<Domain.Entities.Coffee> repository,
        AutoMapper.IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CoffeeDto>> Handle(GetAllCoffeesQuery request, CancellationToken cancellationToken)
    {
        var coffees = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<CoffeeDto>>(coffees);
    }
}
