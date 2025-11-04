namespace MediConnect.Web.Services
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<T, bool>? predicate = null);
        T? GetById(int id);
        T Add(T entity);
        bool Update(int id, Action<T> updater);
        bool Delete(int id);
        int Count(Func<T, bool>? predicate = null);
        IReadOnlyCollection<T> Snapshot();
        void Load(IEnumerable<T> items);
    }
}
