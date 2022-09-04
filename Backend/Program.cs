using Backend;
using Backend.Logic;
using Backend.Options;
using Backend.Repositorys;
using Backend.Security;
using Backend.Security.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITokenContext, TokenContext>();

builder.Services.AddScoped<TokenRepository>();
builder.Services.AddScoped<GroupRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<UserLogic>();

builder.Services.AddOptionsFromConfiguration<JwtTokenAuthenticationOptions>(builder.Configuration);
builder.Services.AddOptionsFromConfiguration<UserOptions>(builder.Configuration);
builder.Services.AddOptionsFromConfiguration<UserMessageOptions>(builder.Configuration);

builder.Services.AddCors();
builder.Services.AddAuthentication(JwtTokenAuthentication.Scheme).AddJwtTokenAuthentication(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {{
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,
        },
        ArraySegment<string>.Empty
    }});
});

var app = builder.Build();

GroupRepository.CompileGroups(app.Configuration);

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(
    options => options
        .WithOrigins(app.Configuration.GetSection("Origins").Get<string[]>())
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

app.MapControllers();
app.Run();