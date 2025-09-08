using Serilog;

namespace LibraryAPI.Endpoints.Books;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book> CreateBookAsync(CreateBookRequest request);
}

public class BookService : IBookService
{
    private readonly List<Book> _books = new();
    private int _nextId = 1;

    public BookService()
    {
        Log.Information("Initializing BookService with sample data");
        
        _books.Add(new Book
        {
            Id = _nextId++,
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            PublicationYear = 1925
        });

        _books.Add(new Book
        {
            Id = _nextId++,
            Title = "Harry Potter and the Sorcerer's Stone",
            Author = "J.K. Rowling",
            PublicationYear = 1998
        });
        
        Log.Information("BookService initialized with {BookCount} sample books", _books.Count);
    }

    public Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        Log.Information("Retrieving all books. Total count: {BookCount}", _books.Count);
        return Task.FromResult(_books.AsEnumerable());
    }

    public Task<Book> CreateBookAsync(CreateBookRequest request)
    {
        Log.Information("Creating new book: {Title} by {Author}", request.Title, request.Author);
        
        var book = new Book
        {
            Id = _nextId++,
            Title = request.Title,
            Author = request.Author,
            PublicationYear = request.PublicationYear
        };

        _books.Add(book);
        
        Log.Information("Successfully created book with ID {BookId}: {Title}", book.Id, book.Title);
        return Task.FromResult(book);
    }
}
