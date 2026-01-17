using FluentAssertions;
using Lab.Coffe.Domain.Entities;
using Xunit;

namespace Lab.Coffe.Tests.Domain;

public class CoffeeTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCoffee()
    {
        // Arrange
        var name = "Espresso";
        var description = "Strong coffee";
        var price = 10.50m;
        var stock = 100;

        // Act
        var coffee = new Coffee(name, description, price, stock);

        // Assert
        coffee.Should().NotBeNull();
        coffee.Name.Should().Be(name);
        coffee.Description.Should().Be(description);
        coffee.Price.Should().Be(price);
        coffee.Stock.Should().Be(stock);
        coffee.IsActive.Should().BeTrue();
        coffee.Id.Should().NotBeEmpty();
        coffee.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var coffee = new Coffee("Old Name", "Description", 10m, 50);
        var newName = "New Name";

        // Act
        coffee.UpdateName(newName);

        // Assert
        coffee.Name.Should().Be(newName);
        coffee.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateName_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var coffee = new Coffee("Name", "Description", 10m, 50);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => coffee.UpdateName(string.Empty));
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
    {
        // Arrange
        var coffee = new Coffee("Name", "Description", 10m, 50);
        var newPrice = 15.99m;

        // Act
        coffee.UpdatePrice(newPrice);

        // Assert
        coffee.Price.Should().Be(newPrice);
    }

    [Fact]
    public void UpdatePrice_WithNegativePrice_ShouldThrowException()
    {
        // Arrange
        var coffee = new Coffee("Name", "Description", 10m, 50);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => coffee.UpdatePrice(-10m));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var coffee = new Coffee("Name", "Description", 10m, 50);

        // Act
        coffee.Deactivate();

        // Assert
        coffee.IsActive.Should().BeFalse();
    }
}
