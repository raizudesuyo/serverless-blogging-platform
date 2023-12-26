using Microsoft.AspNetCore.Mvc;
using ServerlessBloggingPlatform.Services;

namespace ServerlessBloggingPlatform.Controllers;

[Route("api/[controller]")]
public class AssetController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetController(IAssetService assetService)
    {
        _assetService = assetService;
    }   

    // POST api/asset
    [HttpPost]
    public async Task<IActionResult> UploadAsset(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        // Upload the file to S3 using the asset service
        var assetUrl = await _assetService.UploadAsset(file);

        return Ok(assetUrl);
    }

    

    // GET api/asset/{id}/url
    [HttpGet("{id}/url")]
    public async Task<ActionResult<string>> GetAssetUrl(Guid id)
    {
        var assetUrl = await _assetService.GetAssetUrl(id);

        if(assetUrl == null)
        {
            return NotFound();
        }

        return Ok(assetUrl);
    }
}

