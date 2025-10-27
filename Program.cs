using Microsoft.EntityFrameworkCore;
using NumberGuessGameApi.Data;
using NumberGuessGameApi.Services;

namespace NumberGuessGameApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
   var builder = WebApplication.CreateBuilder(args);

            // Obtener la cadena de conexión
          var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Registrar GameDbContext
 builder.Services.AddDbContext<GameDbContext>(options =>
        options.UseSqlServer(connectionString));

            // Registrar el servicio del juego
  builder.Services.AddScoped<IGameService, GameService>();

// Add services to the container.
       builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();

  var app = builder.Build();

       // Configure the HTTP request pipeline.
  if (app.Environment.IsDevelopment())
   {
           app.UseSwagger();
        app.UseSwaggerUI();
    }

          app.UseHttpsRedirection();

            app.UseAuthorization();


        app.MapControllers();

            app.Run();
        }
    }
}
