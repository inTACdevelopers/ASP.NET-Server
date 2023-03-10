
using Server.Services.Authorization;
using Server.Services.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();


app.MapGet("/", () => "This is INTAC");

app.MapGrpcService<Authorization>();
app.MapGrpcService<Registration>();


app.Run();
