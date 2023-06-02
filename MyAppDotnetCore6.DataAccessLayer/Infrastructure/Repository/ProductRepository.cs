using MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository;
using MyAppDotnetCore6.Models;
using MyAppWebCore6.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDotnetCore6.DataAccessLayer.Infrastructure.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private MyAppDbContext _context;
        public ProductRepository(MyAppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productDB = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productDB != null)
            {
                productDB.Name = product.Name;
                productDB.Description= product.Description;
                productDB.Price= product.Price;
                if (product.ImageUrl != null)
                    productDB.ImageUrl = product.ImageUrl;
            }
        }
    }
}
