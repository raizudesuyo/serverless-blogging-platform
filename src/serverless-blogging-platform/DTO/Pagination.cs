using Microsoft.AspNetCore.Mvc;

namespace ServerlessBloggingPlatform.Pagination
{
    public class PaginationDTO {
        
        [FromQuery]
        public Guid? LastKey { get; set; }
        
        [FromQuery]
        public int PageSize { get; set; } = 10;
        
        [FromQuery]
        public bool FirstPage { get; set; } = true;
    }
}
