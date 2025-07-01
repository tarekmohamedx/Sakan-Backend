using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.Interfaces
{
    public interface IPaymentService
    {
        // هذه الدالة تنشئ "نية دفع" وترجع مفتاح سري للـ Frontend
        Task<string> CreatePaymentIntentAsync(int bookingId, string userId);

        // هذه الدالة تتعامل مع إشعارات Stripe (Webhooks) لتأكيد الدفع
        Task HandleWebhookEventAsync(string json, string stripeSignature);
    }
}
