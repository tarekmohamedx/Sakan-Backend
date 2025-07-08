using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sakan.Application.Interfaces.User;
using Sakan.Domain.Interfaces;
using Sakan.Domain.IUnitOfWork;
using Sakan.Domain.Models;
using Stripe;

namespace Sakan.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _stripeSecretKey;
        private readonly string _webhookSecret;

        public PaymentService(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _paymentRepository = paymentRepository;
            _stripeSecretKey = config["Stripe:SecretKey"];
            _webhookSecret = config["Stripe:WebhookSecret"];
            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        public async Task<string> CreatePaymentIntentAsync(int bookingId, string userId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            // 1. التحقق من صحة الحجز
            if (booking == null || booking.GuestId != userId || booking.PaymentStatus != "Pending" ||
                booking.Listing.IsActive == false || booking.Listing.IsApproved == false)
            {
                throw new Exception("Invalid booking or booking already processed.");
            }

            // 2. حساب المبلغ **دائماً في الباك اند** لضمان الأمان
            var amountInCents = (long)(booking.Price * 100); 

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "egp", // أو أي عملة أخرى
                Metadata = new Dictionary<string, string>
                {
                    { "booking_id", booking.Id.ToString() },
                    { "user_id", userId }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // 3. إنشاء سجل دفع في قاعدة بياناتك بحالة "pending"
            var newPayment = new Payment
            {
                BookingId = booking.Id,
                StripePaymentIntentId = paymentIntent.Id,
                Amount = booking.Price,
                Currency = "egp",
                Status = "pending", // أو "requires_payment_method"
                PaymentDate = DateTime.UtcNow
            };
            // سنحتاج لـ context لإضافة الدفعة أو repository متخصص
            // For simplicity, assuming UnitOfWork can access DbContext to add.
            // In a real app, you might have IPaymentRepository with an Add method.
            await _paymentRepository.AddAsync(newPayment);

            await _unitOfWork.SaveChangesAsync();

            // 4. إرجاع الـ client_secret فقط للـ Frontend
            return paymentIntent.ClientSecret;
        }

        public async Task HandleWebhookEventAsync(string json, string stripeSignature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _webhookSecret);

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntentSucceeded = (PaymentIntent)stripeEvent.Data.Object;
                        Console.WriteLine("Payment succeeded webhook triggered");
                        // **هذه هي نقطة تأكيد الدفع النهائية والمضمونة**
                        await ProcessSuccessfulPayment(paymentIntentSucceeded.Id);
                        break;

                    case "payment_intent.payment_failed":
                        var paymentIntentFailed = (PaymentIntent)stripeEvent.Data.Object;
                        // يمكنك هنا إرسال إيميل للمستخدم أو تحديث الحالة
                        Console.WriteLine($"Payment failed for Intent: {paymentIntentFailed.Id}");
                        break;

                    default:
                        Console.WriteLine($"Ignored event type: {stripeEvent.Type}");
                        break;
                }
            }
            catch (StripeException e)
            {
                // خطأ في التحقق من صحة الـ Webhook
                throw new StripeException("Webhook signature validation failed.", e);
            }
        }

        private async Task ProcessSuccessfulPayment(string paymentIntentId)
        {
            // هنا ستحتاجين لـ IPaymentRepository لجلب الدفعة وتحديثها
            var payment = await _paymentRepository.GetByIntentIdAsync(paymentIntentId);
            Console.WriteLine(payment == null ? "Payment not found in DB!" : "Payment found!");
            if (payment != null && payment.Status != "Paid")
            {
                Console.WriteLine($"Payment Succeeded for Intent: {paymentIntentId}");
                payment.Status = "Paid";
                payment.PaymentDate = DateTime.UtcNow;
                var booking = await _bookingRepository.GetByIdAsync(payment.BookingId.Value);
                booking.PaymentStatus = "Paid";
                await _unitOfWork.SaveChangesAsync();
                // يمكنك هنا إرسال إيميل تأكيد الحجز
                Console.WriteLine($"Payment Succeeded for Intent: {paymentIntentId}");
            }
        }
    }
}
