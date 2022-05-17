namespace NetExam.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetByName(string name);
        void Add(T entity);
    }
}
