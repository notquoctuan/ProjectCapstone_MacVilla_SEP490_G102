namespace BE_API.Configuration;

/// <summary>
/// Nạp file <c>.env</c> (KEY=value) vào biến môi trường tiến trình — giống cách bạn khai báo cho Docker,
/// để <c>dotnet run</c> không cần set tay PAYOS_* trên Windows.
/// </summary>
public static class DotEnvLoader
{
    public static void LoadOptional(string? path = null)
    {
        path ??= Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (!File.Exists(path))
            return;

        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            var eq = line.IndexOf('=');
            if (eq <= 0)
                continue;

            var key = line[..eq].Trim();
            if (key.Length == 0)
                continue;

            var val = line[(eq + 1)..].Trim();
            if (val.Length >= 2 && val[0] == '"' && val[^1] == '"')
                val = val[1..^1];

            Environment.SetEnvironmentVariable(key, val);
        }
    }

    /// <summary>
    /// ASP.NET Core bind <c>CloudinaryOptions</c> từ biến môi trường <c>Cloudinary__CloudName</c>, không tự nhận <c>CLOUDINARY_CLOUD_NAME</c>.
    /// Docker-compose map sẵn; với <c>.env</c> + DotEnvLoader thường dùng tên <c>CLOUDINARY_*</c> — copy sang dạng <c>Cloudinary__*</c> nếu chưa có.
    /// </summary>
    public static void ApplyCloudinaryAspNetEnvAliases()
    {
        CopyEnvIfTargetEmpty("CLOUDINARY_CLOUD_NAME", "Cloudinary__CloudName");
        CopyEnvIfTargetEmpty("CLOUDINARY_API_KEY", "Cloudinary__ApiKey");
        CopyEnvIfTargetEmpty("CLOUDINARY_API_SECRET", "Cloudinary__ApiSecret");
        CopyEnvIfTargetEmpty("CLOUDINARY_BASE_FOLDER", "Cloudinary__BaseFolder");
        CopyEnvIfTargetEmpty("CLOUDINARY_MAX_FILE_BYTES", "Cloudinary__MaxFileBytes");
    }

    private static void CopyEnvIfTargetEmpty(string sourceKey, string targetKey)
    {
        var src = Environment.GetEnvironmentVariable(sourceKey);
        if (string.IsNullOrWhiteSpace(src))
            return;
        var existing = Environment.GetEnvironmentVariable(targetKey);
        if (!string.IsNullOrWhiteSpace(existing))
            return;
        Environment.SetEnvironmentVariable(targetKey, src.Trim());
    }
}
