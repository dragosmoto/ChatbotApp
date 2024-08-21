using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI_API;
using ChatbotApp.Services;
using ChatbotApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register SignalR
builder.Services.AddSignalR();

// Register the OpenAI service with your API key.
builder.Services.AddSingleton(new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API")));
builder.Services.AddSingleton<OpenAIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chatbot}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chathub");

app.Run();
