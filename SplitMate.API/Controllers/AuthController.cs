using Microsoft.AspNetCore.Mvc;
using SplitMate.Application.DTOs;
using SplitMate.Application.UseCases.Auth;

namespace SplitMate.API.Controllers;

// o controller é apenas uma porta de entrada — recebe a requisição e chama o use case
// não tem nenhuma regra de negócio aqui, só orquestração — isso é Clean Architecture
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterUseCase _registerUseCase;
    private readonly LoginUseCase _loginUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;

    public AuthController(RegisterUseCase registerUseCase, LoginUseCase loginUseCase, RefreshTokenUseCase refreshTokenUseCase)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var response = await _registerUseCase.ExecuteAsync(request);
            // 201 Created é o status correto para criação de recursos — não 200 OK
            return Created(string.Empty, response);
        }
        catch (InvalidOperationException ex)
        {
            // 409 Conflict — o recurso já existe
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await _loginUseCase.ExecuteAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            // 401 Unauthorized — credenciais inválidas
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        try
        {
            var response = await _refreshTokenUseCase.ExecuteAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}