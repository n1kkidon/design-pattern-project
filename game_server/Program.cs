using game_server.Sockets;
using shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHostedService<CoinBackgroundService>();
builder.Services.AddSingleton<CoinBackgroundService>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseForwardedHeaders();
//app.UseAuthorization();
app.RegisterSocketEndpoints();
app.MapRazorPages();
app.Run();
