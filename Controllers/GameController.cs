using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NumberGuessGameApi.DataTransferObjects;
using NumberGuessGameApi.Services;

namespace NumberGuessGameApi.Controllers
{
    [ApiController]
    [Route("api/game/v1")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterPlayer([FromBody] RegisterPlayerRequest request)
        {
            try
            {
                var response = await _gameService.RegisterPlayerAsync(request);
                _logger.LogInformation("Jugador registrado exitosamente con ID: {PlayerId}", response.PlayerId);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al registrar jugador: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al registrar el jugador");
                return StatusCode(500, new { message = "Ocurrió un error inesperado al registrar el jugador." });
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            try
            {
                var response = await _gameService.StartGameAsync(request);
                _logger.LogInformation("Juego iniciado exitosamente con ID: {GameId} para el jugador: {PlayerId}",
 response.GameId, response.PlayerId);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al iniciar juego: {Message}", ex.Message);

                // Si el mensaje indica que el jugador no existe, devolver NotFound (404)
                if (ex.Message.Contains("no existe"))
                {
                    return NotFound(new { message = ex.Message });
                }

                // Para otros errores (ej. juego activo ya existe), devolver BadRequest (400)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al iniciar el juego");
                return StatusCode(500, new { message = "Ocurrió un error inesperado al iniciar el juego." });
            }
        }

        [HttpPost("guess")]
        public async Task<IActionResult> GuessNumber([FromBody] GuessNumberRequest request)
        {
            try
            {
                var response = await _gameService.GuessNumberAsync(request);
                _logger.LogInformation("Intento procesado exitosamente para el juego: {GameId}, resultado: {Message}",
  response.GameId, response.Message);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al procesar intento: {Message}", ex.Message);

                // Si el mensaje indica que el juego no existe, devolver NotFound (404)
                if (ex.Message.Contains("no existe"))
                {
                    return NotFound(new { message = ex.Message });
                }

                // Para otros errores de validación (no activo, dígitos no únicos), devolver BadRequest (400)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar el intento");
                return StatusCode(500, new { message = "Ocurrió un error inesperado al procesar el intento." });
            }
        }
    }
}
