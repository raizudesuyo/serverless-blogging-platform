using Microsoft.AspNetCore.Mvc;
using ServerlessBloggingPlatform.Entities;
using ServerlessBloggingPlatform.Pagination;
using ServerlessBloggingPlatform.Services;

namespace ServerlessBloggingPlatform.Controllers;

[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(
        IPostService postService
    )
    {
        _postService = postService;
    }

    // GET api/post
    [HttpGet]
    public async Task<ActionResult<PostDTOPaged>> GetPosts([FromQuery] PaginationDTO pagination)
    {
        var result = await _postService.GetPosts(pagination);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDTO>> GetPost([FromRoute] Guid id)
    {
        var result = await _postService.GetPost(id);
        return Ok(result);
    }

    [HttpGet("/user/{id}")]
    public async Task<ActionResult<PostDTOPaged>> GetPostsByUser([FromRoute] Guid id, [FromQuery] PaginationDTO pagination)
    {
        var result = await _postService.GetPostsByUser(id, pagination);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost(CreatePostDTO post)
    {
        var result = await _postService.CreatePost(post);
        return Ok(result);
    }

    [HttpPut("{postId}")]
    public async Task<ActionResult<PostDTO>> UpdatePost(Guid postId, CreatePostDTO post)
    {
        var result = await _postService.UpdatePost(postId, post);
        return Ok(result);
    }
}

