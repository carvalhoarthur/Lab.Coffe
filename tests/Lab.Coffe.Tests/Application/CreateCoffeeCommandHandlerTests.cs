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

public class CreateCoffeeCommandHandlerTests
{
    private readonly Mock<IRepository<Coffee>> _repositoryMock;
    private readonly Mock<IMessagePublisher> _messagePublisherMock;
    private readonly IMapper _mapper;

    public CreateCoffeeCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Coffee>>();
        _messagePublisherMock = new Mock<IMessagePublisher>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateCoffeeAndPublishMessage()
    {
        // Arrange
        var request = new CreateCoffeeRequest
        {
            Name = "Espresso",
            Description = "Strong coffee",
            Price = 10.50m,
            Stock = 100
        };

        var createdCoffee = new Coffee(request.Name, request.Description, request.Price, request.Stock);
        
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Coffee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCoffee);

        _messagePublisherMock
            .Setup(p => p.PublishAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateCoffeeCommandHandler(
            _repositoryMock.Object,
            _messagePublisherMock.Object,
            _mapper);
        var command = new CreateCoffeeCommand(request);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Price.Should().Be(request.Price);
        
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Coffee>(), It.IsAny<CancellationToken>()), Times.Once);
        _messagePublisherMock.Verify(
            p => p.PublishAsync(It.IsAny<object>(), "coffee.created", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
