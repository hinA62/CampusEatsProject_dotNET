using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.MenuTests.IntegrationTests;

public class DeleteMenuHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly DeleteMenuHandler _handler;
    private readonly ILogger<DeleteMenuHandler> _logger = new LoggerFactory().CreateLogger<DeleteMenuHandler>();

    public DeleteMenuHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CampusEatsContext(options);
        _handler = new DeleteMenuHandler(_context, _logger);
    }

    [Fact]
    public async Task Given_ValidDeleteMenuRequest_When_Handle_Then_ShouldDeleteMenu()
    {
        //Arrange
        var menuId = Guid.NewGuid();
        var request = new DeleteMenuRequest(menuId);
        
        //Act
        await _handler.Handle(request);
        
        //Assert
        Assert.Null(await _context.Menu.FindAsync(menuId));
    }

    [Fact]
    public async Task Given_NullMenuId_When_Handle_Then_ShouldReturnNotFound()
    {
        //Arrange
        var request = new DeleteMenuRequest(Guid.Empty);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        
        GC.SuppressFinalize(this);
    }
}