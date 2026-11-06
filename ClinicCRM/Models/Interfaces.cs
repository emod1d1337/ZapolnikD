namespace ClinicCRM.Models;

public interface IExportable { string GetSummary(); }

public interface IRepository<T> where T : class
{
    List<T> GetAll();
    T? GetById(int id);
    void Add(T item);
}
