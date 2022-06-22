using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var ProductDB = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if(ProductDB != null)
            {
                ProductDB.Name = product.Name;
                ProductDB.Description = product.Description;
                ProductDB.Price = product.Price;
                if (product.ImageUrl != null)
                {
                    ProductDB.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
