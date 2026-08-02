
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DIn.Service.IDInService, DIn.Service.DInService>();
builder.Services.AddHttpClient<DIn.Repository.IDInRepository, DIn.Repository.DInRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
