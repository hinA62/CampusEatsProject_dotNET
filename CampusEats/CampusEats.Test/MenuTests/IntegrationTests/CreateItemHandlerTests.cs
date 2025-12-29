using CampusEats.Features.Menu;
using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.MenuTests.IntegrationTests;

public class CreateItemHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly CreateItemHandler _handler;
    private readonly ILogger<CreateItemHandler> _logger = new LoggerFactory().CreateLogger<CreateItemHandler>();

    public CreateItemHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CampusEatsContext(options);
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CreateItemHandler>();
        _handler = new CreateItemHandler(_context, _logger);
    }

    [Fact]
    public async Task Given_ValidCreateItemRequest_When_Handle_Then_ShouldCreateItem()
    {
        //Arrange
        var request = new CreateItemRequest(Guid.NewGuid(), "Pizza",
            10.00m, "http://img.com", ["gluten"]);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        var createdResult = result.Should()
            .BeOfType<Microsoft.AspNetCore.Http.HttpResults.Created<MenuItem>>().Subject;
        
        var idDinHandler = createdResult.Value!.Id;
        var itemInDb = await _context.MenuItem.FindAsync(idDinHandler);
        
        itemInDb.Should().NotBeNull();
        itemInDb!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task Given_FailedValidation_When_Handle_Then_ShouldReturnBadRequest()
    {
        //Arrange
        var request = new CreateItemRequest(Guid.NewGuid(), "Ab", -1m, null, null);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults
            .BadRequest<List<FluentValidation.Results.ValidationFailure>>>();
        
        var count = await _context.MenuItem.CountAsync();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
