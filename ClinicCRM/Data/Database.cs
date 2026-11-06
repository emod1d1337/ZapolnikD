using Microsoft.Data.Sqlite;
using ClinicCRM.Models;

namespace ClinicCRM;

public static class Database
{
    public static string DbPath => Path.Combine(AppContext.BaseDirectory, "clinic.db");
    public static string ConnectionString => $"Data Source={DbPath}";

    public static void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"
PRAGMA foreign_keys = ON;
CREATE TABLE IF NOT EXISTS Departments(Id INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT NOT NULL, BaseRate REAL NOT NULL);
CREATE TABLE IF NOT EXISTS Doctors(Id INTEGER PRIMARY KEY AUTOINCREMENT, FullName TEXT NOT NULL, DepartmentId INTEGER, Category TEXT NOT NULL, BaseSalary REAL NOT NULL, FOREIGN KEY(DepartmentId) REFERENCES Departments(Id));
CREATE TABLE IF NOT EXISTS Appointments(Id INTEGER PRIMARY KEY AUTOINCREMENT, DoctorId INTEGER, PatientName TEXT NOT NULL, Cost REAL NOT NULL, Date TEXT NOT NULL, Status TEXT NOT NULL, FOREIGN KEY(DoctorId) REFERENCES Doctors(Id));
";
        command.ExecuteNonQuery();
        command.CommandText = "SELECT COUNT(*) FROM Departments";
        if (Convert.ToInt32(command.ExecuteScalar()) == 0)
        {
            command.CommandText = "INSERT INTO Departments(Title,BaseRate) VALUES ('Терапия',1000),('Хирургия',1800),('Кардиология',1500);";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO Doctors(FullName,DepartmentId,Category,BaseSalary) VALUES ('Иванов Иван Иванович',1,'Высшая',120000),('Петрова Анна Сергеевна',2,'Первая',145000),('Сидоров Алексей Олегович',3,'Высшая',130000);";
            command.ExecuteNonQuery();
            command.CommandText = @"INSERT INTO Appointments(DoctorId,PatientName,Cost,Date,Status) VALUES
(1,'Алексей Смирнов',3500,'2026-10-20','Completed'),(2,'Мария Кузнецова',7200,'2026-10-21','Completed'),
(1,'Дмитрий Волков',2800,'2026-10-22','Planned'),(3,'Елена Попова',4100,'2026-10-23','Completed'),
(2,'Алексей Смирнов',6500,'2026-10-24','Completed'),(1,'Ольга Соколова',3000,'2026-10-25','Cancelled');";
            command.ExecuteNonQuery();
        }
    }

    public static SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var pragma = connection.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys=ON";
        pragma.ExecuteNonQuery();
        return connection;
    }
}

public sealed class SqliteRepository<T> : IRepository<T> where T : class
{
    private readonly Func<SqliteDataReader, T> _map;
    private readonly string _selectSql;
    private readonly Action<SqliteCommand, T> _insert;

    public SqliteRepository(string selectSql, Func<SqliteDataReader, T> map, Action<SqliteCommand, T> insert)
    { _selectSql = selectSql; _map = map; _insert = insert; }

    public List<T> GetAll()
    {
        using var connection = Database.OpenConnection();
        using var command = connection.CreateCommand(); command.CommandText = _selectSql;
        using var reader = command.ExecuteReader();
        var result = new List<T>(); while (reader.Read()) result.Add(_map(reader));
        return result;
    }

    public T? GetById(int id)
    {
        using var connection = Database.OpenConnection();
        using var command = connection.CreateCommand(); command.CommandText = _selectSql + " WHERE Id=$id";
        command.Parameters.AddWithValue("$id", id);
        using var reader = command.ExecuteReader(); return reader.Read() ? _map(reader) : null;
    }

    public void Add(T item)
    {
        using var connection = Database.OpenConnection();
        using var command = connection.CreateCommand(); _insert(command, item); command.ExecuteNonQuery();
    }
}
