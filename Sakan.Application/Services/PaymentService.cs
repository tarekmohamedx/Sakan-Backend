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
        private readonly IBookingRequestRepository _requestRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _stripeSecretKey;
        private readonly string _webhookSecret;

        public PaymentService(IBookingRequestRepository requestRepository, IBookingRepository bookingRepository, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _requestRepository = requestRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _paymentRepository = paymentRepository;
            _stripeSecretKey = config["Stripe:SecretKey"];
            _webhookSecret = config["Stripe:WebhookSecret"];
            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        public async Task<string> CreatePaymentIntentAsync(int requestId, string userId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);

            // 1. التحقق من صحة الحجز
            if (request == null || !request.IsActive || request.GuestId != userId)
            {
                throw new InvalidOperationException("This booking request is not ready for payment.");
            }

            // 2. حساب المبلغ **دائماً في الباك اند** لضمان الأمان
            var price = CalculatePrice(request);
            var amountInCents = (long)(price * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "egp",
                Metadata = new Dictionary<string, string>
            {
                { "booking_request_id", requestId.ToString() },
                { "user_id", userId }
            }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

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
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            // 1. استخراج booking_request_id من الـ Metadata
            var requestIdStr = paymentIntent.Metadata["booking_request_id"];
            if (!int.TryParse(requestIdStr, out var requestId))
            {
                throw new InvalidOperationException("Booking Request ID not found in payment metadata.");
            }

            // 2. جلب طلب الحجز الأصلي
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                throw new KeyNotFoundException("Original booking request not found.");
            }

            // 3. **الآن فقط، نقوم بإنشاء سجل الـ Booking**
            var newBooking = new Booking
            {
                GuestId = request.GuestId,
                ListingId = request.ListingId,
                RoomId = request.RoomId,
                BedId = request.BedId,
                FromDate = request.FromDate.Value,
                ToDate = request.ToDate.Value,
                Price = (decimal)paymentIntent.Amount / 100, // السعر من الدفعة المؤكدة
                PaymentStatus = "Paid", // <-- مدفوع مباشرة
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _bookingRepository.AddAsync(newBooking);

            // 4. إنشاء سجل الـ Payment
            var newPayment = new Payment
            {
                // يجب الانتظار حتى يتم حفظ الحجز للحصول على BookingId
                // سنقوم بتعديل هذا الجزء
                Booking = newBooking, // ربط مباشر
                StripePaymentIntentId = paymentIntent.Id,
                Amount = newBooking.Price,
                Currency = paymentIntent.Currency,
                Status = "Paid",
                PaymentDate = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(newPayment);

            // 5. حفظ كل شيء في عملية واحدة
            await _unitOfWork.SaveChangesAsync();
        }
        private decimal CalculatePrice(BookingRequest request)
        {
            // 1. إذا كان السرير محدداً
            if (request.BedId.HasValue && request.Bed != null)
            {
                return request.Bed.Price ?? 0;
            }
            // 2. إذا كانت الغرفة محددة (بدون سرير)
            else if (request.RoomId.HasValue && request.Room != null)
            {
                // نفترض أن السعر هنا لليلة الواحدة
                var nights = (request.ToDate.Value - request.FromDate.Value).Days;
                return (request.Room.PricePerNight ?? 0) * nights;
            }
            // 3. إذا كانت الشقة كاملة محددة
            else if (request.ListingId.HasValue && request.Listing != null)
            {
                return request.Listing.PricePerMonth ?? 0; // أو أي منطق آخر للشقة كاملة
            }

            throw new InvalidOperationException("Could not determine the price for the booking request.");
        }
    }
}
