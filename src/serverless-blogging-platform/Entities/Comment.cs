using ServerlessBloggingPlatform.Pagination;

namespace ServerlessBloggingPlatform.Entities;

public class Comment {
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public Guid ParentId { get; set; }
    public string Content { get; set; } = String.Empty;
}

public class CreateCommentDTO
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public Guid ParentId { get; set; }
    public string Content { get; set; } = String.Empty;
}

public class CommentDTO {
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public Guid ParentId { get; set; }
    public string Content { get; set; } = String.Empty;
}

public class CommentDTOPaged : PaginationDTO {
    public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
}