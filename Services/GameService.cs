using GameCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NumberGuessGameApi.Data;
using NumberGuessGameApi.DataTransferObjects;
using NumberGuessGameApi.Models;

namespace NumberGuessGameApi.Services
{
    public class GameService : IGameService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<GameService> _logger;

        public GameService(GameDbContext context, ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RegisterPlayerResponse> RegisterPlayerAsync(RegisterPlayerRequest request)
        {
            _logger.LogInformation("Registrando nuevo jugador: {FirstName} {LastName}", request.FirstName, request.LastName);

            // Verificar si el jugador ya existe
            var existingPlayer = await _context.Players
                .FirstOrDefaultAsync(p => p.FirstName == request.FirstName && p.LastName == request.LastName);

            if (existingPlayer != null)
            {
                _logger.LogWarning("El jugador ya existe: {PlayerId}", existingPlayer.Id);
                throw new InvalidOperationException($"El jugador con nombre {request.FirstName} {request.LastName} ya existe.");
            }

            // Crear nuevo jugador
            var player = new Player
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                RegistrationDate = DateTime.UtcNow
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Jugador registrado exitosamente con ID: {PlayerId}", player.Id);

            return new RegisterPlayerResponse
            {
                PlayerId = player.Id
            };
        }

        public async Task<StartGameResponse> StartGameAsync(StartGameRequest request)
        {
            _logger.LogInformation("Iniciando nuevo juego para el jugador: {PlayerId}", request.PlayerId);

            // Verificar si el jugador existe
            var player = await _context.Players.FindAsync(request.PlayerId);
            if (player == null)
            {
                _logger.LogWarning("Jugador no encontrado: {PlayerId}", request.PlayerId);
                throw new InvalidOperationException($"El jugador con ID {request.PlayerId} no existe.");
            }

            // Verificar si el jugador ya tiene un juego activo
            var activeGame = await _context.Games
                .FirstOrDefaultAsync(g => g.PlayerId == request.PlayerId && g.Status == GameStatus.Active);

            if (activeGame != null)
            {
                _logger.LogWarning("El jugador {PlayerId} ya tiene un juego activo: {GameId}", request.PlayerId, activeGame.Id);
                throw new InvalidOperationException($"El jugador ya tiene un juego activo con ID {activeGame.Id}.");
            }

            // Generar número secreto de 4 dígitos únicos
            var secretNumber = GenerateSecretNumber();
            _logger.LogDebug("Número secreto generado para el juego: {SecretNumber}", secretNumber);

            // Crear nuevo juego
            var game = new Game
            {
                PlayerId = request.PlayerId,
                SecretNumber = secretNumber,
                CreatedAt = DateTime.UtcNow,
                Status = GameStatus.Active
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Juego iniciado exitosamente con ID: {GameId}", game.Id);

            return new StartGameResponse
            {
                GameId = game.Id,
                PlayerId = game.PlayerId,
                CreatedAt = game.CreatedAt
            };
        }

        public async Task<GuessNumberResponse> GuessNumberAsync(GuessNumberRequest request)
        {
            _logger.LogInformation("Procesando intento para el juego: {GameId}, número intentado: {AttemptedNumber}",
                request.GameId, request.AttemptedNumber);

            // Verificar si el juego existe
            var game = await _context.Games
                .Include(g => g.Attempts)
                .FirstOrDefaultAsync(g => g.Id == request.GameId);

            if (game == null)
            {
                _logger.LogWarning("Juego no encontrado: {GameId}", request.GameId);
                throw new InvalidOperationException($"El juego con ID {request.GameId} no existe.");
            }

            // Verificar si el juego está activo
            if (game.Status != GameStatus.Active)
            {
                _logger.LogWarning("El juego {GameId} no está activo", request.GameId);
                throw new InvalidOperationException($"El juego con ID {request.GameId} no está activo.");
            }

            // Validar que el número tenga 4 dígitos únicos
            if (!HasUniqueDigits(request.AttemptedNumber))
            {
                _logger.LogWarning("Número intentado inválido: {AttemptedNumber} - los dígitos no son únicos", request.AttemptedNumber);
                throw new InvalidOperationException("El número intentado debe tener 4 dígitos únicos.");
            }

            // Obtener el mensaje de resultado usando ESCMB.GameCore
            var resultMessage = GetResultMessage(game.SecretNumber, request.AttemptedNumber);
            _logger.LogDebug("Mensaje de resultado: {ResultMessage}", resultMessage);

            // Registrar el intento
            var attempt = new Attempt
            {
                GameId = request.GameId,
                AttemptedNumber = request.AttemptedNumber,
                ResultMessage = resultMessage,
                AttemptDate = DateTime.UtcNow
            };

            _context.Attempts.Add(attempt);

            // Verificar si el juego ha finalizado
            if (request.AttemptedNumber == game.SecretNumber)
            {
                game.Status = GameStatus.Finished;
                _logger.LogInformation("Juego {GameId} finalizado - el jugador adivinó correctamente", request.GameId);
            }

            await _context.SaveChangesAsync();

            return new GuessNumberResponse
            {
                GameId = request.GameId,
                AttemptedNumber = request.AttemptedNumber,
                Message = resultMessage
            };
        }

        private string GenerateSecretNumber()
        {
            var random = new Random();
            var digits = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var secretNumber = "";

            for (int i = 0; i < 4; i++)
            {
                var index = random.Next(digits.Count);
                secretNumber += digits[index];
                digits.RemoveAt(index);
            }

            return secretNumber;
        }

        private bool HasUniqueDigits(string number)
        {
            return number.Distinct().Count() == number.Length;
        }

        private string GetResultMessage(string secretNumber, string attemptedNumber)
        {
            // Si adivinó correctamente
            if (secretNumber == attemptedNumber)
            {
                return "¡Felicidades! Adivinaste el número.";
            }

            // Llamar al método del paquete NuGet ESCMB.GameCore
            // El método ValidateAttempt recibe strings directamente
            var evaluationResult = Evaluator.ValidateAttempt(secretNumber, attemptedNumber);

            // Devolver el mensaje formateado proporcionado por el paquete
            return evaluationResult.Message;
        }
    }
}
