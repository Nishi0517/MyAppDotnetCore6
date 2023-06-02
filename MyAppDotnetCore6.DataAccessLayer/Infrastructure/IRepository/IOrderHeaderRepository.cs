using MyAppDotnetCore6.Models;
using MyAppDotnetCore6.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderDetail);
        void UpdateStatus(int Id, string orderStatus,string? paymentStatus=null);
        void PaymentStatus(int Id, string SessionId,string PaymentIntentId);
    }
}
