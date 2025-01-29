using System.Collections.Generic;

namespace Domain.Repository.Base
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        public void ValidBeforeSave(){}

        public void ValidBeforeUpdate(){}

        public void ValidBeforeDelete(){}

        Task AfterSave(TEntity obj);
        Task AfterUpdate(TEntity obj);
        Task AfterDelete(TEntity obj);

        Task BeforeSave(TEntity obj);
        Task BeforeUpdate(TEntity obj);
        Task BeforeDelete(TEntity obj);

        //Task<IEnumerable<TEntity>> GetAll();
        //Task<TEntity> GetById(int id);

        Task SaveAsync(TEntity obj);
        Task UpdateAsync(TEntity obj);
        Task DeleteAsync(TEntity obj);
    }
}
