using Microsoft.AspNetCore.Mvc;
using Futzin.Api.Application.Services;

namespace Futzin.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeladaGroupController : ControllerBase
{
    private readonly PeladaGroupService _service;

    public PeladaGroupController(PeladaGroupService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var group = await _service.GetByIdAsync(id);
        if (group == null)
            return NotFound();

        return Ok(group);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserGroups(int userId)
    {
        var groups = await _service.GetUserGroupsAsync(userId);
        return Ok(groups);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        try
        {
            var group = await _service.CreateGroupAsync(
                request.Name,
                request.Description,
                request.CreatedById
            );

            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGroupRequest request)
    {
        try
        {
            var group = await _service.UpdateGroupAsync(id, request.Name, request.Description);
            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteGroupAsync(id);
        return NoContent();
    }

    [HttpPost("{groupId}/members")]
    public async Task<IActionResult> AddMember(int groupId, [FromBody] AddMemberRequest request)
    {
        try
        {
            var member = await _service.AddMemberAsync(groupId, request.UserId, request.IsAdmin);
            return Ok(member);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{groupId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(int groupId, int userId)
    {
        await _service.RemoveMemberAsync(groupId, userId);
        return NoContent();
    }
}

public record CreateGroupRequest(string Name, string? Description, int CreatedById);
public record UpdateGroupRequest(string Name, string? Description);
public record AddMemberRequest(int UserId, bool IsAdmin = false);
