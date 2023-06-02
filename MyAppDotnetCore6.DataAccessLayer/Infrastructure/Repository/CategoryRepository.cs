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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private MyAppDbContext _context;
        public CategoryRepository(MyAppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var categoryDB = _context.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (categoryDB != null)
            {
                categoryDB.Name = category.Name;
                categoryDB.DisplayOrder = category.DisplayOrder;
            }
        }

        public void UpdateVM(CategoryVM categoryVM)
        {
            throw new NotImplementedException();
        }
    }
}
