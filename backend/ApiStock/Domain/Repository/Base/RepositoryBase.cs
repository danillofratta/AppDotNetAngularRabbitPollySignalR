

using ApiStock.Service.Redis;
using SharedDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Domain.Repository.Base
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly DBDevContext _AppDbContext;
        protected readonly IRedisCacheService _RedisCacheService;

        public RepositoryBase(DBDevContext appDbContext, IRedisCacheService redisCacheService)
        {
            this._AppDbContext = appDbContext;
            this._RedisCacheService = redisCacheService;
        }
        
        public virtual void ValidBeforeSave()
        {

        }

        public virtual void ValidBeforeUpdate()
        {

        }

        public virtual void ValidBeforeDelete()
        {

        }

        public async Task SaveAsync(TEntity obj)
        {
            try
            {
                this.ValidBeforeSave();
                await _AppDbContext.Set<TEntity>().AddAsync(obj);
                await _AppDbContext.SaveChangesAsync();
                await this.AfterSave(obj);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task DeleteAsync(TEntity obj)
        {
            try
            {
                this.ValidBeforeDelete();
                _AppDbContext.Set<TEntity>().Remove(obj);
                await _AppDbContext.SaveChangesAsync();
                await this.AfterDelete(obj);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task UpdateAsync(TEntity obj)
        {
            try
            {                
                this.ValidBeforeUpdate();
                this.BeforeUpdate(obj);
                _AppDbContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _AppDbContext.SaveChangesAsync();
                await this.AfterUpdate(obj);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public virtual async Task BeforeSave(TEntity obj)
        {
            //throw new NotImplementedException();
        }

        public virtual async Task BeforeUpdate(TEntity obj)
        {
            //throw new NotImplementedException();
        }

        public virtual async Task BeforeDelete(TEntity obj)
        {
            //throw new NotImplementedException();
        }

        public virtual async Task AfterSave(TEntity obj)
        {
            //throw new NotImplementedException();
        }

        public virtual async Task AfterUpdate(TEntity obj)
        {
            //throw new NotImplementedException();
        }

        public virtual async Task AfterDelete(TEntity obj)
        {
            //throw new NotImplementedException();
        }

        //public async Task<IEnumerable<TEntity>> GetAll()
        //{
        //    //throw new NotImplementedException();
        //}

        //public async Task<TEntity> GetById(int id)
        //{
        //    //throw new NotImplementedException();
        //}

        //public IEnumerable<TEntity> GetAll()
        //{
        //    return _AppDbContext.Set<TEntity>().ToList();

        //}

        //public TEntity GetById(int id)
        //{
        //    return _AppDbContext.Set<TEntity>().Find(id);
        //}        
    }
}
