using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;

namespace Sakan.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly sakanContext _context;

        public PaymentRepository(sakanContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task<Payment> GetByIntentIdAsync(string paymentIntentId)
        {
            Console.WriteLine($"Payment Succeeded: {paymentIntentId}");
            var payment= await _context.Payments
                .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId);
            Console.WriteLine(payment == null ? "Payment not found in DB!" : "Payment found!");
            return payment;
        }
    }
}
