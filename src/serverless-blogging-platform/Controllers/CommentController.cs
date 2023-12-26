using Microsoft.AspNetCore.Mvc;
using ServerlessBloggingPlatform.Entities;
using ServerlessBloggingPlatform.Pagination;
using ServerlessBloggingPlatform.Services;

namespace ServerlessBloggingPlatform.Controllers;

[Route("api/post/{postId}/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentController(
        ICommentService commentService
    )
    {
        _commentService = commentService;
    }   

    // GET api/post/{postId}/comment
    [HttpGet]
    public async Task<ActionResult<CommentDTOPaged>> GetComments([FromRoute] Guid postId, [FromRoute] PaginationDTO pagination)
    {
        var result = await _commentService.GetComments(postId, pagination);
        return Ok(result);
    }

    // POST api/post/{postId}/comment
    [HttpPost]
    public async Task<ActionResult<CommentDTO>> CreateComment([FromRoute] Guid postId, CreateCommentDTO comment)
    {
        var result = await _commentService.CreateComment(postId, comment);
        return Ok(result);
    }
}

