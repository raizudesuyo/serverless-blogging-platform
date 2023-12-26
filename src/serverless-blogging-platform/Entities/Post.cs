using ServerlessBloggingPlatform.Pagination;

namespace ServerlessBloggingPlatform.Entities;

public class Post {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Content { get; set; } = String.Empty;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public class CreatePostDTO
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Content { get; set; } = String.Empty;
}

public class PostDTO {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Content { get; set; } = String.Empty;
}

public class PostDTOPaged : PaginationDTO {
    public List<PostDTO> Posts { get; set; } = new List<PostDTO>();
}