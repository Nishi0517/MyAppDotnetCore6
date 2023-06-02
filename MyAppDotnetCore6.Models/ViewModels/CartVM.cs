using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDotnetCore6.Models.ViewModels
{
    public class CartVM
    {
        public IEnumerable<Cart> ListOfCart { get; set; }
        public Double Total { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
