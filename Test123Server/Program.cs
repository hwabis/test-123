using Test123Server.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();

app.MapHub<TestHub>("/testHub");

app.Run();
