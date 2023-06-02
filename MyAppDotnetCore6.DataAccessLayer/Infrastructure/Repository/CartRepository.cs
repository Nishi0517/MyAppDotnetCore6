using MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository;
using MyAppDotnetCore6.Models;
using MyAppDotnetCore6.Models.ViewModels;
using MyAppWebCore6.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDotnetCore6.DataAccessLayer.Infrastructure.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private MyAppDbContext _context;
        public CartRepository(MyAppDbContext context) : base(context)
        {
            _context = context;
        }

        public int IncrementCartItem(Cart cart, int count)
        {
            cart.Count += count;
            return cart.Count;
        }

        public int DecrementCartItem(Cart cart, int count)
        {
            cart.Count -= count;
            return cart.Count;
        }
    }
}
