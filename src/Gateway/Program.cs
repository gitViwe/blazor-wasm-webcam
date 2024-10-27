using Gateway.Feature;
using Gateway.SignalRHub;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddScoped<RealtimeServerHubContext>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddCors(options =>
    {
        options.AddDefaultPolicy(
            policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
    })
    .AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapHub<RealtimeServerHub>("/notification");
app.MapVideoFrameCapturedEndpoint();

app.Run();
