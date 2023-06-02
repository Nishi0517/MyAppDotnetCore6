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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private MyAppDbContext _context;
        public OrderHeaderRepository(MyAppDbContext context) : base(context)
        {
            _context = context;
        }

        public void PaymentStatus(int Id, string SessionId, string PaymentIntentId)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            orderHeader.DateOfPayment = DateTime.Now;
            orderHeader.PaymentIntentId = PaymentIntentId;
            orderHeader.SessionId = SessionId;
        }

        public void Update(OrderHeader orderHeader)
        {

            _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null)
        {
            var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            if(order!= null)
            {
                order.OrderStatus = orderStatus;
            }

        }
    }
}

