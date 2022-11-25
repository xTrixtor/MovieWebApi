using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyAwesomeWebApi;
using MyAwesomeWebApi.DataStore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var _connectionString = builder.Configuration.GetConnectionString("MySQLNico");
var key = builder.Configuration.GetSection("JWTKey").Get<string>();

// Add services to the container.
builder.Services.AddScoped<MovieService>(x => new MovieService(_connectionString));
builder.Services.AddScoped<AuthService>(x => new AuthService(_connectionString));
builder.Services.AddScoped<UserService>(x => new UserService(_connectionString));
builder.Services.AddScoped<ResetService>(x => new ResetService(_connectionString));
builder.Services.AddScoped<MailService>(x => new MailService(builder.Configuration));
builder.Services.AddSingleton<JwtAuthenticationManager>(new JwtAuthenticationManager(key));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Auth
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


var app = builder.Build();

app.UseCors(policy => policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(origin => true)
                            .AllowCredentials());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
