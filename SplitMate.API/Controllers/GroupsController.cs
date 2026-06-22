using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitMate.Application.DTOs;
using SplitMate.Application.UseCases.Groups;

namespace SplitMate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // todas as rotas desse controller exigem token JWT válido
public class GroupsController : ControllerBase
{
    private readonly CreateGroupUseCase _createGroupUseCase;
    private readonly JoinGroupUseCase _joinGroupUseCase;
    private readonly GetUserGroupsUseCase _getUserGroupsUseCase;

    public GroupsController(
        CreateGroupUseCase createGroupUseCase,
        JoinGroupUseCase joinGroupUseCase,
        GetUserGroupsUseCase getUserGroupsUseCase)
    {
        _createGroupUseCase = createGroupUseCase;
        _joinGroupUseCase = joinGroupUseCase;
        _getUserGroupsUseCase = getUserGroupsUseCase;
    }

    // método auxiliar que extrai o userId do token JWT
    // o token carrega o id do usuário nas claims — não precisamos consultar o banco pra isso
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateGroupRequest request)
    {
        try
        {
            var response = await _createGroupUseCase.ExecuteAsync(request, GetUserId());
            return Created(string.Empty, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("join")]
    public async Task<IActionResult> Join(JoinGroupRequest request)
    {
        try
        {
            var response = await _joinGroupUseCase.ExecuteAsync(request, GetUserId());
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyGroups()
    {
        var response = await _getUserGroupsUseCase.ExecuteAsync(GetUserId());
        return Ok(response);
    }
}