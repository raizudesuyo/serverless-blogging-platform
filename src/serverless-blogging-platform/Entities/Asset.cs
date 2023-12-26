namespace ServerlessBloggingPlatform.Entities;

public class Asset
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Url { get; set; } = String.Empty;
}

public class CreateAssetDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Url { get; set; } = String.Empty;
}

public class UpdateAssetDTO
{
    public string Name { get; set; } = String.Empty;
    public string Url { get; set; } = String.Empty;
}

public class AssetDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Url { get; set; } = String.Empty;
}

