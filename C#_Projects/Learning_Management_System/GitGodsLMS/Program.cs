using Microsoft.EntityFrameworkCore;
using GitGodsLMS.Data;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

StripeConfiguration.ApiKey = "sk_test_51QpBp5HwEsq58B0arkys0fa9jDjq5pUVGkFRfcuMvRhee0s7oBEAtq1N8H03evSrgIf0mLKr3mMOnoeszJrx89tS00RV8SWF1n";


builder.Services.AddRazorPages();


builder.Services.AddDistributedMemoryCache();


builder.Services.AddSession(options =>
{

});


builder.Services.AddDbContext<LMSPagesContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    ));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
