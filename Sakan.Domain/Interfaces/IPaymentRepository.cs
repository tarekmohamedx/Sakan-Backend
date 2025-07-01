using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakan.Domain.Models;

namespace Sakan.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        // إضافة سجل دفع جديد
        Task AddAsync(Payment payment);

        // جلب سجل الدفع باستخدام الـ Stripe Payment Intent ID
        Task<Payment> GetByIntentIdAsync(string paymentIntentId);
    }
}
