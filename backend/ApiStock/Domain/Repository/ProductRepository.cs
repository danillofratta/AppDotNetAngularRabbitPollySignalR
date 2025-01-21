using ApiStock.Service.Redis;
using Domain.Repository.Base;
using Microsoft.EntityFrameworkCore;
using SharedDatabase.Models;

namespace ApiStock.Domain.Repository
{
    public class ProductRepository : RepositoryBase<Product>
    {
        public ProductRepository(DBDevContext appDbContext, IRedisCacheService redisCacheService) : base(appDbContext, redisCacheService)
        {
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await Task.Run(() => _AppDbContext.Product.ToListAsync());
        }

        public async Task<Product> GetById(int id)
        {
            var cachedProduct = await _RedisCacheService.GetAsync<Product>($"product:{id}");
            if (cachedProduct != null)
            {
                return cachedProduct;
            }

            return await _AppDbContext.Product.SingleOrDefaultAsync(x => x.Id == id);
        }

        public override async Task AfterSave(Product obj)
        {
            await _RedisCacheService.SetAsync($"product:{obj.Id}", obj, TimeSpan.FromHours(1));
            base.AfterSave(obj);
            //return base.AfterSave(obj);
        }

        public override async Task AfterUpdate(Product obj)
        {
            await _RedisCacheService.SetAsync($"product:{obj.Id}", obj, TimeSpan.FromHours(1));
            base.AfterUpdate(obj);
        }

        public override async Task AfterDelete(Product obj)
        {
            await _RedisCacheService.RemoveAsync($"product:{obj.Id}");
            base.AfterDelete(obj);
        }


    }
}
