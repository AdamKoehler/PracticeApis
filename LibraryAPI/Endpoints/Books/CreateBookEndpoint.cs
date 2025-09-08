using FastEndpoints;
using LibraryAPI.Endpoints.Books;
using LibraryAPI.Auth;
using Serilog;

namespace LibraryAPI.Endpoints.Books;

public class CreateBookEndpoint : Endpoint<CreateBookRequest, Book>
{
    private readonly IBookService _bookService;

    public CreateBookEndpoint(IBookService bookService)
    {
        _bookService = bookService;
    }

    public override void Configure()
    {
        Post("/api/books");
        Roles(AuthRoles.BeyondTrust);
        Description(d => d
            .WithName("Create Book")
            .WithTags("Books")
            .Produces<Book>(201, "application/json")
            .WithSummary("Create a new book")
            .WithDescription("Adds a new book to the library"));
    }

    public override async Task<Book> ExecuteAsync(CreateBookRequest req, CancellationToken ct)
    {
        Log.Information("POST /api/books - Creating book: {Title} by {Author}", req.Title, req.Author);
        
        var book = await _bookService.CreateBookAsync(req);
        
        Log.Information("POST /api/books - Successfully created book with ID {BookId}", book.Id);
        return book;
    }
}
