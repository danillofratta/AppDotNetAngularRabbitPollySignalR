using ApiStock.Service.Redis;
using Domain.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        //todo improve search by name
        public async Task<List<Product>> GetByName(string name)
        {
            var productIds = await _RedisCacheService.GetSetMembersAsync("all:products");

            List<Product> products = new List<Product>();

            if (productIds.Any())
            {
                foreach (var id in productIds)
                {
                    var product = await _RedisCacheService.GetAsync<Product>($"product:{id}");                    
                    if (product != null && product.Name.ToUpper().Contains(name.ToUpper()))
                        products.Add(product);
                }                
            }

            if (!products.Any())
            {
                products = await _AppDbContext.Product.Where(x => x.Name.ToUpper().Contains(name.ToUpper())).ToListAsync();

                foreach (var product in products)
                {
                    await _RedisCacheService.SetAsync($"product:{product.Id}", product, TimeSpan.FromHours(1));
                    await _RedisCacheService.AddToSetAsync("all:products", product.Id.ToString());                    
                }
            }


            return products;
        }

        public override async Task AfterSave(Product obj)
        {
            await _RedisCacheService.SetAsync($"product:{obj.Id}", obj, TimeSpan.FromHours(1));
            await _RedisCacheService.AddToSetAsync("all:products", obj.Id.ToString());
            base.AfterSave(obj);            
        }

        public override async Task AfterUpdate(Product obj)
        {
            await _RedisCacheService.SetAsync($"product:{obj.Id}", obj, TimeSpan.FromHours(1));

            base.AfterUpdate(obj);
        }

        public override async Task BeforeUpdate(Product obj)
        {
            await _RedisCacheService.RemoveAsync($"product:{obj.Id}");
            
            base.BeforeUpdate(obj);
        }

        public override async Task AfterDelete(Product obj)
        {
            await _RedisCacheService.RemoveAsync($"product:{obj.Id}");
            await _RedisCacheService.RemoveFromSetAsync("all:products", obj.Id.ToString());


            base.AfterDelete(obj);
        }


    }
}
