namespace hoikun.Services
{
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Sas;
    using hoikun.Data;
    using Microsoft.EntityFrameworkCore;
    using System.IO;

    public class BlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public BlobStorageService(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // BLOBアップロード
        public async Task UploadFilesAsync(int userId, IList<(Stream Stream, string FileName)> fileStreams)
        {
            BlobContainerClient containerClient = GetContainerClient();
            List<Rout> routeModels = new();

            foreach ((Stream stream, string fileName) in fileStreams)
            {
                string newFileName = $"{userId}_map_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(fileName)}";
                BlobClient blobClient = containerClient.GetBlobClient(newFileName);

                try
                {
                    await UploadWithRetryAsync(blobClient, stream);
                    routeModels.Add(new Rout
                    {
                        Id = userId,
                        PhotoLocation = newFileName,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to upload file {fileName}: {ex.Message}");
                }
            }

            // データベース登録
            await SaveRoutesToDatabaseAsync(routeModels);
        }

        private BlobContainerClient GetContainerClient()
        {
            string? accountName = _configuration["AzureStorageConfig:AccountName"];
            string? accountKey = _configuration["AzureStorageConfig:AccountKey"];
            string? containerName = _configuration["AzureStorageConfig:ContainerName"];

            string connectionString = $"DefaultEndpointsProtocol=https;AccountName=hoikunstorage;AccountKey=ySwkO2vNpImvCSej/4Bo0vt7MOElbDu/pQXbXMzeL/fs9g8iPs5PLPd3ZtiiHmmB2EjjDSdazLtE+AStohT0IA==;EndpointSuffix=core.windows.net";
            BlobServiceClient blobServiceClient = new(connectionString);
            return blobServiceClient.GetBlobContainerClient(containerName);
        }

        private async Task UploadWithRetryAsync(BlobClient blobClient, Stream fileStream, int retryCount = 3)
        {
            for (int attempt = 1; attempt <= retryCount; attempt++)
            {
                try
                {
                    await blobClient.UploadAsync(fileStream, overwrite: true);
                    return;
                }
                catch (Exception ex) when (attempt < retryCount)
                {
                    Console.WriteLine($"Retry {attempt} for blob {blobClient.Name} failed: {ex.Message}");
                    await Task.Delay(2000); // リトライ間隔
                }
            }
        }

        private async Task SaveRoutesToDatabaseAsync(List<Rout> routes)
        {
            if (routes == null || !routes.Any()) return;

            try
            {
                _dbContext.Routs.AddRange(routes);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save routes to database: {ex.Message}");
            }
        }

        // BLOBの削除
        public async Task DeleteFileAsync(string blobName, int userId)
        {
            BlobContainerClient containerClient = GetContainerClient();
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            try
            {
                // BLOB削除
                await blobClient.DeleteIfExistsAsync();

                // データベース更新
                Rout? route = await _dbContext.Routs
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.PhotoLocation == blobName);
                if (route != null)
                {
                    _dbContext.Routs.Remove(route);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete blob {blobName}: {ex.Message}");
            }
        }

        // BLOB一覧の取得
        public async Task<List<BlobFileInfo>> GetFilesAsync(int userId)
        {
            BlobContainerClient containerClient = GetContainerClient();
            List<string?> routes = await _dbContext.Routs
                .Where(r => r.UserId == userId && !string.IsNullOrEmpty(r.PhotoLocation))
                .Select(r => r.PhotoLocation)
                .ToListAsync();

            List<BlobFileInfo> files = new();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                if (routes.Contains(blobItem.Name))
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    files.Add(new BlobFileInfo
                    {
                        Name = blobItem.Name,
                        Url = GenerateSasUri(blobClient)
                    });
                }
            }

            return files;
        }

        private string GenerateSasUri(BlobClient blobClient)
        {
            string? accountName = _configuration["AzureStorageConfig:AccountName"];
            string? accountKey = _configuration["AzureStorageConfig:AccountKey"];
            StorageSharedKeyCredential credential = new(accountName, accountKey);

            BlobSasBuilder sasBuilder = new()
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return $"{blobClient.Uri}?{sasBuilder.ToSasQueryParameters(credential)}";
        }
    }

    public class BlobFileInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
