using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.JSInterop;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.Menu;
using CampusEatsFrontend.Models.MenuItem;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class CartServiceTests
{
    private readonly Mock<IJSRuntime> _mockJs;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _mockJs = new Mock<IJSRuntime>();
        _cartService = new CartService(_mockJs.Object);
    }

    [Fact]
    public void Items_WhenEmpty_ReturnsEmptyList()
    {
        _cartService.Items.Should().BeEmpty();
        _cartService.TotalItems.Should().Be(0);
        _cartService.TotalPrice.Should().Be(0);
    }

    [Fact]
    public async Task AddMenuAsync_AddsItemToCart()
    {
        var menu = new MenuDto { Id = Guid.NewGuid(), Name = "Test Menu", Price = 25.99m };

        await _cartService.AddMenuAsync(menu);

        _cartService.Items.Should().HaveCount(1);
        _cartService.Items.First().Name.Should().Be("Test Menu");
        _cartService.TotalPrice.Should().Be(25.99m);
    }

    [Fact]
    public async Task AddMenuAsync_WithQuantity_AddsCorrectQuantity()
    {
        var menu = new MenuDto { Id = Guid.NewGuid(), Name = "Test Menu", Price = 10.00m };

        await _cartService.AddMenuAsync(menu, 3);

        _cartService.Items.First().Quantity.Should().Be(3);
        _cartService.TotalItems.Should().Be(3);
        _cartService.TotalPrice.Should().Be(30.00m);
    }

    [Fact]
    public async Task AddMenuAsync_SameMenuTwice_IncreasesQuantity()
    {
        var menuId = Guid.NewGuid();
        var menu = new MenuDto { Id = menuId, Name = "Test Menu", Price = 15.00m };

        await _cartService.AddMenuAsync(menu, 2);
        await _cartService.AddMenuAsync(menu, 3);

        _cartService.Items.Should().HaveCount(1);
        _cartService.Items.First().Quantity.Should().Be(5);
        _cartService.TotalPrice.Should().Be(75.00m);
    }

    [Fact]
    public async Task AddMenuAsync_WithZeroPrice_DoesNotAddToCart()
    {
        var menu = new MenuDto { Id = Guid.NewGuid(), Name = "Free Menu", Price = 0m };

        await _cartService.AddMenuAsync(menu);

        _cartService.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task AddMenuItemAsync_AddsItemToCart()
    {
        var menuItem = new MenuItemDto { Id = Guid.NewGuid(), Name = "Pizza Margherita", Price = 12.50m };

        await _cartService.AddMenuItemAsync(menuItem);

        _cartService.Items.Should().HaveCount(1);
        _cartService.Items.First().Name.Should().Be("Pizza Margherita");
        _cartService.Items.First().MenuItemId.Should().Be(menuItem.Id);
    }

    [Fact]
    public async Task RemoveItemAsync_RemovesMenuFromCart()
    {
        var menuId = Guid.NewGuid();
        var menu = new MenuDto { Id = menuId, Name = "Test Menu", Price = 20.00m };
        await _cartService.AddMenuAsync(menu);

        await _cartService.RemoveItemAsync(menuId);

        _cartService.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveMenuItemAsync_RemovesItemFromCart()
    {
        var menuItemId = Guid.NewGuid();
        var menuItem = new MenuItemDto { Id = menuItemId, Name = "Test Item", Price = 8.00m };
        await _cartService.AddMenuItemAsync(menuItem);

        await _cartService.RemoveMenuItemAsync(menuItemId);

        _cartService.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateQuantityAsync_UpdatesItemQuantity()
    {
        var menuId = Guid.NewGuid();
        var menu = new MenuDto { Id = menuId, Name = "Test Menu", Price = 10.00m };
        await _cartService.AddMenuAsync(menu, 2);

        await _cartService.UpdateQuantityAsync(menuId, 5);

        _cartService.Items.First().Quantity.Should().Be(5);
        _cartService.TotalPrice.Should().Be(50.00m);
    }

    [Fact]
    public async Task UpdateQuantityAsync_WithZero_RemovesItem()
    {
        var menuId = Guid.NewGuid();
        var menu = new MenuDto { Id = menuId, Name = "Test Menu", Price = 10.00m };
        await _cartService.AddMenuAsync(menu);

        await _cartService.UpdateQuantityAsync(menuId, 0);

        _cartService.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task ClearAsync_RemovesAllItems()
    {
        var menu1 = new MenuDto { Id = Guid.NewGuid(), Name = "Menu 1", Price = 10.00m };
        var menu2 = new MenuDto { Id = Guid.NewGuid(), Name = "Menu 2", Price = 15.00m };
        await _cartService.AddMenuAsync(menu1);
        await _cartService.AddMenuAsync(menu2);

        await _cartService.ClearAsync();

        _cartService.Items.Should().BeEmpty();
        _cartService.TotalPrice.Should().Be(0);
    }

    [Fact]
    public async Task TotalPrice_CalculatesCorrectly()
    {
        var menu1 = new MenuDto { Id = Guid.NewGuid(), Name = "Menu 1", Price = 10.00m };
        var menu2 = new MenuDto { Id = Guid.NewGuid(), Name = "Menu 2", Price = 15.50m };
        var menuItem = new MenuItemDto { Id = Guid.NewGuid(), Name = "Item", Price = 5.00m };

        await _cartService.AddMenuAsync(menu1, 2);     // 20.00
        await _cartService.AddMenuAsync(menu2, 1);     // 15.50
        await _cartService.AddMenuItemAsync(menuItem, 3); // 15.00

        _cartService.TotalPrice.Should().Be(50.50m);
        _cartService.TotalItems.Should().Be(6);
    }
}
