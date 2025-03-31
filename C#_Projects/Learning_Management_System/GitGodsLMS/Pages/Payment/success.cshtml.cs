using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Stripe.Checkout;
using GitGodsLMS.Data;
using System;
using System.Threading.Tasks;

namespace GitGodsLMS.wwwroot
{
    public class successModel : PageModel
    {
        private readonly LMSPagesContext _context;
        public successModel(LMSPagesContext context)
        {
            _context = context;
        }
        public Session? StripeSession { get; set; }
        public string? CustomerEmail { get; set; }
        public string? PaymentAmount { get; set; }
        public string? PaymentDate { get; set; }
        public string? PaymentId { get; set; }
        public string? ReceiptUrl { get; set; }
        public string? DebugMessage { get; set; } // For debugging purposes

        public async Task<IActionResult> OnGetAsync()
        {
            var sessionId = Request.Query["session_id"];
            
            // Debug: Add session ID check
            if (string.IsNullOrEmpty(sessionId))
            {
                DebugMessage = "No session_id found in query parameters";
                return Page(); // Return the page with debug message instead of redirecting
            }

            var sessionService = new SessionService();
            try
            {
                StripeSession = await sessionService.GetAsync(sessionId);

                // Debug: Add status check
                DebugMessage = $"Session status: {StripeSession.Status}, Payment status: {StripeSession.PaymentStatus}";

                // Check both status and payment status with more permissive conditions
                if (StripeSession.Status == "complete" ||
                    StripeSession.PaymentStatus == "paid" ||
                    StripeSession.PaymentStatus == "complete" ||
                    !string.IsNullOrEmpty(StripeSession.PaymentIntentId))
                {
                    CustomerEmail = StripeSession.CustomerDetails?.Email;
                    PaymentAmount = StripeSession.AmountTotal.HasValue
                        ? (StripeSession.AmountTotal.Value / 100.0m).ToString("C")
                        : "$0.00";
                    PaymentDate = DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss");
                    PaymentId = StripeSession.PaymentIntentId;

                    if (!string.IsNullOrEmpty(PaymentId))
                    {
                        try
                        {
                            var paymentIntentService = new PaymentIntentService();
                            var paymentIntent = await paymentIntentService.GetAsync(PaymentId);

                            if (paymentIntent.LatestChargeId != null)
                            {
                                var chargeService = new ChargeService();
                                var charge = await chargeService.GetAsync(paymentIntent.LatestChargeId);
                                ReceiptUrl = charge.ReceiptUrl;
                            }
                            else
                            {
                                DebugMessage += " | No charge ID available on payment intent";
                            }
                        }
                        catch (StripeException ex)
                        {
                            DebugMessage += $" | Error getting charge: {ex.Message}";
                        }
                    }
                    else
                    {
                        DebugMessage += " | No PaymentIntentId available";
                    }

                    return Page();
                }
                else
                {
                    // Instead of redirecting, return the page with debug info
                    DebugMessage += " | Payment conditions not met";
                    return Page();
                }
            }
            catch (StripeException ex)
            {
                // Add detailed error logging
                DebugMessage = $"Stripe Error: {ex.Message}, StripeError: {ex.StripeError?.Message}, Type: {ex.StripeError?.Type}";
                return Page(); // Return the page with error info instead of redirecting
            }
            catch (Exception ex)
            {
                // Catch any other exceptions
                DebugMessage = $"General Error: {ex.Message}";
                return Page(); // Return the page with error info instead of redirecting
            }
        }
    }
}