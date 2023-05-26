using feedbacktrx.filehandlermicroservice.Exceptions;
using feedbacktrx.filehandlermicroservice.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Inject Services
builder.Services.AddSingleton<IClamAVService>(_ => new ClamAVService(builder.Configuration["ClamAVConfig:Host"], int.Parse(builder.Configuration["ClamAVConfig:Port"])));
builder.Services.AddScoped<IFileHandlerService, FileHandlerService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create folder for fileuploads
FileUploadDirectory.Create();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("_myAllowSpecificOrigins");

app.UseMiddleware<ExceptionHandler>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
