namespace NetExam.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Repository<T> : IRepository<T> where T : class
    {
        private List<T> context { get; set; }
        public Repository()
        {
            context = new List<T>();
        }

        public void Add(T entity)
        {
            context.Add(entity);
        }

        public List<T> GetAll()
        {
            return context.ToList();
        }

        public T GetByName(string name)
        {
            return context.FirstOrDefault();
        }
    }
}
