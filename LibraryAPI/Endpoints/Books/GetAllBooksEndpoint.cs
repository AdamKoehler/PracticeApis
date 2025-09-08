using FastEndpoints;
using LibraryAPI.Endpoints.Books;
using LibraryAPI.Auth;
using Serilog;

namespace LibraryAPI.Endpoints.Books;

public class GetAllBooksEndpoint : EndpointWithoutRequest<IEnumerable<Book>>
{
    private readonly IBookService _bookService;

    public GetAllBooksEndpoint(IBookService bookService)
    {
        _bookService = bookService;
    }

    public override void Configure()
    {
        Get("/api/books");
        Roles(AuthRoles.BeyondTrust);
        Description(d => d
            .WithName("Get All Books")
            .WithTags("Books")
            .Produces<IEnumerable<Book>>(200, "application/json")
            .WithSummary("Retrieve all books")
            .WithDescription("Returns a list of all books in the library"));
    }

    public override async Task<IEnumerable<Book>> ExecuteAsync(CancellationToken ct)
    {
        Log.Information("GET /api/books - Retrieving all books");
        
        var books = await _bookService.GetAllBooksAsync();
        
        Log.Information("GET /api/books - Returning {BookCount} books", books.Count());
        return books;
    }
}
