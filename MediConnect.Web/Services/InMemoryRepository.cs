using MediConnect.Web.Models;

namespace MediConnect.Web.Services
{
    /// <summary>
    /// Simple in-memory repository with optional JSON snapshot support.
    /// </summary>
    /// <typeparam name="T">Domain model inheriting BaseEntity</typeparam>
    public class InMemoryRepository<T> where T : BaseEntity
    {
        private readonly List<T> _items = new();
        private int _nextId = 1;

        /// <summary>Return all items (enumerable view).</summary>
        public IEnumerable<T> GetAll() => _items;

        /// <summary>Count of items (instance method so controllers can call repo.Count()).</summary>
        public int Count() => _items.Count;

        /// <summary>Find by id, or null.</summary>
        public T? Find(int id) => _items.FirstOrDefault(e => e.Id == id);

        /// <summary>Alias to match older controllers.</summary>
        public T? GetById(int id) => Find(id);

        /// <summary>Adds a new entity. Assigns an Id if missing.</summary>
        public T Add(T entity)
        {
            if (entity.Id == 0)
                entity.Id = _nextId++;
            _items.Add(entity);
            return entity;
        }

        /// <summary>Replace an existing entity by Id.</summary>
        public bool Update(T entity)
        {
            var i = _items.FindIndex(e => e.Id == entity.Id);
            if (i < 0) return false;
            entity.UpdatedAt = DateTime.UtcNow;
            _items[i] = entity;
            return true;
        }

        /// <summary>Overload to support Update(id, entity) call sites.</summary>
        public bool Update(int id, T entity)
        {
            entity.Id = id;
            return Update(entity);
        }

        /// <summary>Delete by id.</summary>
        public bool Delete(int id)
        {
            var found = Find(id);
            if (found is null) return false;
            _items.Remove(found);
            return true;
        }

        /// <summary>
        /// Snapshot shallow copy for JSON persistence.
        /// </summary>
        public List<T> Snapshot() => _items.Select(x => x).ToList();

        /// <summary>
        /// Replace contents from a previously saved snapshot.
        /// Resets next Id to max(existing)+1.
        /// </summary>
        public void Load(IEnumerable<T> items)
        {
            _items.Clear();
            if (items != null)
                _items.AddRange(items);

            _nextId = (_items.Count == 0) ? 1 : _items.Max(x => x.Id) + 1;
        }
    }
}
