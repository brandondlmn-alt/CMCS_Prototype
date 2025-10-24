using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Use In-Memory Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("CMCS_Prototype"));

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Ensure the in-memory database is created (applies model and Seed data from OnModelCreating)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// AUTO-LOGIN MIDDLEWARE - This automatically logs user in
app.Use(async (context, next) =>
{
    // Check if user is not logged in and trying to access root or home
    if (context.Session.GetString("UserID") == null &&
        (context.Request.Path == "/" || context.Request.Path == "/Home" || context.Request.Path == "/Home/Index"))
    {
        // Auto-login as lecturer
        context.Session.SetString("UserID", "1");
        context.Session.SetString("Email", "lecturer@cmcs.edu");
        context.Session.SetString("FirstName", "John");
        context.Session.SetString("LastName", "Smith");
        context.Session.SetString("Role", "lecturer");
        context.Session.SetString("FullName", "John Smith");

        // Redirect to home page
        context.Response.Redirect("/Home/Index");
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
