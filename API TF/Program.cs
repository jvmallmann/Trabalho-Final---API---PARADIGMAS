using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.IO;
using System;
using System.Reflection;
using API_TF.Services;
using API_TF.DataBase;
using FluentValidation.AspNetCore;
using FluentValidation;
using API_TF.Services.DTOs;
using API_TF.Services.Validate;
using AutoMapper;
using API_TF.MappingProfiles;
;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddControllers();
builder.Services.AddDbContext<TfDbContext>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<SaleService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<PromotionService>();
builder.Services.AddScoped<SaleDTOValidator>();
builder.Services.AddScoped<ProductValidate>(); 
builder.Services.AddScoped<ProductService>();
builder.Services.AddTransient<IValidator<PromotionDTO>, PromotionValidate>();
builder.Services.AddScoped<IValidator<ProductDTO>, ProductValidate>();
builder.Services.AddScoped<IValidator<ProductUpDTO>, ProductUpValidate>();
builder.Services.AddTransient<IValidator<SaleDTO>, SaleDTOValidator>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
//builder.Services.AddScoped<EnderecoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Logging.AddFile("Logs/API_TF-{Date}.log");
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API RESTFull",
        Description = " Trabalho Final",
        //TermsOfService = new Uri("https://example.com/terms"),
        //Contact = new OpenApiContact
        //{
        //    Name = "Example Contact",
        //    Url = new Uri("https://example.com/contact")
        //},
        //License = new OpenApiLicense
        //{
        //    Name = "Example License",
        //    Url = new Uri("https://example.com/license")
        //}
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();