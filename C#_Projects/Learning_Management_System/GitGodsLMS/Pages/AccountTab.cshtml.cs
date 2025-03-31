using GitGodsLMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe.Checkout;
namespace GitGodsLMS.Pages
{
    public class AccountTabModel : PageModel
    {
        private readonly LMSPagesContext _context;
        // Binds the amount from the form input
        [BindProperty]
        public decimal PaymentAmount { get; set; }
        public decimal TotalBalance { get; set; }
        public AccountTabModel(LMSPagesContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                RedirectToPage("/Index");
                return;
            }
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.isProfessor)
            {
                RedirectToPage("/Index");
                return;
            }
            TotalBalance = _context.StudentClasses
                .Where(sc => sc.UserId == user.Id)
                .Join(_context.Classes, sc => sc.ClassId, c => c.Id, (sc, c) => c.CreditHours)
                .Sum() * 10;
        }
        public IActionResult OnPostPayBalance()
        {
            long amountInCents = (long)(PaymentAmount * 100);
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = amountInCents,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "LMS Balance Payment",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                // Fix: Add the session_id parameter to the success URL
                SuccessUrl = Url.Page("/Payment/Success", null, null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Page("/Payment/Cancel", null, null, Request.Scheme),
            };
            var service = new SessionService();
            Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}