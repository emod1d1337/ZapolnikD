namespace IndexatorsNet9;

// Задание 1. Книга и библиотека
public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }

    public Book(string title, string author)
    {
        Title = title;
        Author = author;
    }

    public override string ToString() => $"«{Title}» — {Author}";
}

public class Library
{
    private readonly Book[] books;

    public Library(Book[] initialBooks)
    {
        books = initialBooks ?? throw new ArgumentNullException(nameof(initialBooks));
    }

    public Book this[int index]
    {
        get
        {
            if (index < 0 || index >= books.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс книги вне диапазона.");

            return books[index];
        }
        set
        {
            if (index < 0 || index >= books.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс книги вне диапазона.");

            books[index] = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
public class Person
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }

    public Person(string name, string email, string role)
    {
        Name = name;
        Email = email;
        Role = role;
    }

    public override string ToString() => $"{Name} ({Email}), должность: {Role}";
}

public class CompanyCatalog
{
    private readonly Person[] staff;

    public CompanyCatalog(Person[] people)
    {
        staff = people ?? throw new ArgumentNullException(nameof(people));
    }
    public Person this[int index]
    {
        get
        {
            if (index < 0 || index >= staff.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс сотрудника вне диапазона.");

            return staff[index];
        }
        set
        {
            if (index < 0 || index >= staff.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс сотрудника вне диапазона.");

            staff[index] = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
    public Person this[string name]
    {
        get
        {
            foreach (Person person in staff)
            {
                if (string.Equals(person.Name, name, StringComparison.OrdinalIgnoreCase))
                    return person;
            }

            throw new KeyNotFoundException($"Сотрудник с именем '{name}' не найден.");
        }
    }
}
public class UserContainer
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }

    public UserContainer(string name, string email, string role)
    {
        Name = name;
        Email = email;
        Role = role;
    }

    public string this[string field]
    {
        get => field.ToLowerInvariant() switch
        {
            "name" => Name,
            "email" => Email,
            "role" => Role,
            _ => throw new ArgumentException("Неизвестное поле. Используйте name, email или role.")
        };
        set
        {
            switch (field.ToLowerInvariant())
            {
                case "name":
                    Name = value;
                    break;
                case "email":
                    Email = value;
                    break;
                case "role":
                    Role = value;
                    break;
                default:
                    throw new ArgumentException("Неизвестное поле. Используйте name, email или role.");
            }
        }
    }
}
public class GameGrid
{
    private readonly int[,] matrix;

    public GameGrid(int rows, int columns)
    {
        if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows));
        if (columns <= 0) throw new ArgumentOutOfRangeException(nameof(columns));

        matrix = new int[rows, columns];
    }

    public int this[int row, int col]
    {
        get
        {
            CheckBounds(row, col);
            return matrix[row, col];
        }
        private set
        {
            CheckBounds(row, col);
            matrix[row, col] = value;
        }
    }

    public void SetCell(int row, int col, int value)
    {
        this[row, col] = value;
    }

    private void CheckBounds(int row, int col)
    {
        if (row < 0 || row >= matrix.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(row), "Номер строки вне диапазона.");

        if (col < 0 || col >= matrix.GetLength(1))
            throw new ArgumentOutOfRangeException(nameof(col), "Номер столбца вне диапазона.");
    }
}

internal class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("========== ЗАДАНИЕ 1. БИБЛИОТЕКА ==========");

        Book[] initialBooks =
        {
            new Book("Война и мир", "Лев Толстой"),
            new Book("Преступление и наказание", "Фёдор Достоевский"),
            new Book("Мастер и Маргарита", "Михаил Булгаков"),
            new Book("Отцы и дети", "Иван Тургенев")
        };

        Library library = new Library(initialBooks);

        Console.WriteLine($"Книга по индексу 1: {library[1].Title}");

        library[0] = new Book("Евгений Онегин", "Александр Пушкин");
        Console.WriteLine($"После замены нулевого элемента: {library[0]}");

        try
        {
            Console.WriteLine(library[99].Title);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Перехвачено исключение: {ex.GetType().Name}");
        }
        Console.WriteLine();
        Console.WriteLine("========== ЗАДАНИЕ 2. ИНДЕКСАТОРЫ И ПЕРЕГРУЗКА ==========");

        Person[] people =
        {
            new Person("Alice", "alice@company.com", "Разработчик"),
            new Person("Bob", "bob@company.com", "Тестировщик"),
            new Person("Charlie", "charlie@company.com", "Дизайнер")
        };

        CompanyCatalog catalog = new CompanyCatalog(people);

        Console.WriteLine($"catalog[0]: {catalog[0]}");
        Console.WriteLine($"catalog[\"Alice\"]: {catalog["Alice"]}");

        UserContainer user = new UserContainer("Alice", "alice@example.com", "Admin");
        Console.WriteLine($"Поле name: {user["name"]}");
        Console.WriteLine($"Поле email: {user["email"]}");
        Console.WriteLine($"Поле role: {user["role"]}");

        user["role"] = "Manager";
        Console.WriteLine($"Роль после изменения: {user.Role}");

        try
        {
            Console.WriteLine(catalog["Diana"]);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Перехвачено исключение: {ex.Message}");
        }
        Console.WriteLine();
        Console.WriteLine("========== ЗАДАНИЕ 3. ИГРОВОЕ ПОЛЕ ==========");

        GameGrid grid = new GameGrid(3, 3);

        int[,] values =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grid.SetCell(i, j, values[i, j]);
            }
        }

        Console.WriteLine("Матрица 3x3:");
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write($"{grid[i, j],3}");
            }
            Console.WriteLine();
        }
        Console.WriteLine("Прямая запись grid[0, 0] = 5 недоступна извне: set объявлен private.");

        Console.WriteLine();
        Console.WriteLine("Все три задания выполнены.");
    }
}
