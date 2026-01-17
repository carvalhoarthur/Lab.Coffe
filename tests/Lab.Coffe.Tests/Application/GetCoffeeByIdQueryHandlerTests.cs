using FluentAssertions;
using Lab.Coffe.Application.DTOs;
using Lab.Coffe.Application.Interfaces;
using Lab.Coffe.Application.UseCases.Coffee;
using Lab.Coffe.Domain.Entities;
using Moq;
using Xunit;
using AutoMapper;
using Lab.Coffe.Application.Mappings;

namespace Lab.Coffe.Tests.Application;

public class GetCoffeeByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Coffee>> _repositoryMock;
    private readonly IMapper _mapper;

    public GetCoffeeByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Coffee>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldReturnCoffeeDto()
    {
        // Arrange
        var coffeeId = Guid.NewGuid();
        var coffee = new Coffee("Espresso", "Strong coffee", 10.50m, 100);
        
        _repositoryMock
            .Setup(r => r.GetByIdAsync(coffeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coffee);

        var handler = new GetCoffeeByIdQueryHandler(_repositoryMock.Object, _mapper);
        var query = new GetCoffeeByIdQuery(coffeeId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(coffee.Name);
        result.Price.Should().Be(coffee.Price);
        _repositoryMock.Verify(r => r.GetByIdAsync(coffeeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var coffeeId = Guid.NewGuid();
        
        _repositoryMock
            .Setup(r => r.GetByIdAsync(coffeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coffee?)null);

        var handler = new GetCoffeeByIdQueryHandler(_repositoryMock.Object, _mapper);
        var query = new GetCoffeeByIdQuery(coffeeId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(r => r.GetByIdAsync(coffeeId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
