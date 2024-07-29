using SoPro24Team03.Models;
using SoPro24Team03.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// configure language
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "de-de" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

/// <summary>
/// Adds Authentication-Services to the application
/// </summary>
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        //TODO: rename?
        options.Cookie.Name = "ReplayAuth";
        //TODO: change ExpireTimeSpan to something reasonable
        options.ExpireTimeSpan = TimeSpan.FromMinutes(200);
        options.SlidingExpiration = true;
        options.LoginPath = "/Authentication";
        options.LogoutPath = "/Authentication/Logout";
        //TODO: add paths to Logout and permission denied
    });

builder.Services.AddAuthorization();
builder.WebHost.UseStaticWebAssets();

builder.Services.AddDbContext<ReplayContext>();
builder.Services.AddTransient<ITaskTemplateRepository, TaskTemplateRepository>();
builder.Services.AddTransient<IProcedureTemplateRepository, ProcedureTemplateRepository>();
builder.Services.AddTransient<IProcedureRepository, ProcedureRepository>();
builder.Services.AddTransient<ITaskInstRepository, TaskInstRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddSingleton<ISessionService, SessionService>();

var app = builder.Build();

// // setup database
using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    SeedData.initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<SessionValidationMiddleware>();

// enable cookies
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();