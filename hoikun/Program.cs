using hoikun.Data;
using hoikun.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Extensions.Azure;
using Microsoft.Azure.SignalR;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Azure AD B2C �F�؂ƃg�[�N���L���b�V���ݒ�
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();
builder.Services.AddScoped<UserStateService>();


// Add services to the container.
builder.Services.AddScoped<BlobStorageService>();

// �O���[�o���F�؃|���V�[�̐ݒ�
builder.Services.AddControllersWithViews(options =>
{
    AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();


// SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AppointmentService ��o�^
builder.Services.AddScoped<AppointmentService>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor();
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options =>
{
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// �F�؂ƔF��L����
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
