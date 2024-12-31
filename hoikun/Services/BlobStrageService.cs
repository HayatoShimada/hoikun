namespace hoikun.Services
{
    using Azure.Identity;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Sas;
    using hoikun.Data;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.EntityFrameworkCore;

    public class BlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public BlobStorageService(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        // ファイルの一覧を取得
        public async Task<List<BlobFileInfo>> LoadFilesAsync(int userId)
        {
            string? accountName = _configuration["AzureStorageConfig:AccountName"];
            string? containerName = _configuration["AzureStorageConfig:ContainerName"];
            string? accountKey = _configuration["AzureStorageConfig:AccountKey"];

            string containerEndPoint = string.Format("https://{0}.blob.core.windows.net/{1}", accountName, containerName);

            StorageSharedKeyCredential credential = new(accountName, accountKey);
            BlobContainerClient containerClient = new(new Uri(containerEndPoint), credential);

            // データベースから PhotoLocation を取得
            List<string?> routes = await _dbContext
                .Routs
                .Where(r => r.UserId == userId && r.PhotoLocation != null)
                .Select(r => r.PhotoLocation)
                .ToListAsync();

            // BLOB のリストを取得
            List<BlobFileInfo> files = new();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                if (routes.Contains(blobItem.Name))
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

                    // SAS トークンの生成
                    BlobSasBuilder sasBuilder = new()
                    {
                        BlobContainerName = containerName,
                        BlobName = blobItem.Name,
                        Resource = "b",
                        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                    };
                    sasBuilder.SetPermissions(BlobSasPermissions.Read);

                    BlobSasQueryParameters sasToken = sasBuilder.ToSasQueryParameters(credential);
                    Uri sasUri = new($"{blobClient.Uri}?{sasToken}");

                    files.Add(new BlobFileInfo
                    {
                        Name = blobItem.Name,
                        Url = sasUri.ToString()
                    });
                }
            }
            return files;
        }

        // Avatarのダウンロード
        public async Task<BlobFileInfo> LoadAvatarAsync(string filePath)
        {
            string? accountName = _configuration["AzureStorageConfig:AccountName"];
            string? containerName = _configuration["AzureStorageConfig:ContainerName"];
            string? accountKey = _configuration["AzureStorageConfig:AccountKey"];

            string containerEndPoint = string.Format("https://{0}.blob.core.windows.net/{1}", accountName, containerName);

            StorageSharedKeyCredential credential = new(accountName, accountKey);
            BlobContainerClient containerClient = new(new Uri(containerEndPoint), credential);


            // BLOB のリストを取得
            BlobFileInfo file = new();


            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                if (filePath.Contains(blobItem.Name))
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

                    // SAS トークンの生成
                    BlobSasBuilder sasBuilder = new()
                    {
                        BlobContainerName = containerName,
                        BlobName = blobItem.Name,
                        Resource = "b",
                        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                    };
                    sasBuilder.SetPermissions(BlobSasPermissions.Read);

                    BlobSasQueryParameters sasToken = sasBuilder.ToSasQueryParameters(credential);
                    Uri sasUri = new($"{blobClient.Uri}?{sasToken}");

                    file.Name = blobItem.Name;
                    file.Url = sasUri.ToString();

                    return file;
                }
            }
            return file;
        }

        public async Task UploadFilesAsync(int UserId, IList<(Stream Stream, string FileName)> fileStreams)
        {
            string? accountName = _configuration["AzureStorageConfig:AccountName"];
            string? containerName = _configuration["AzureStorageConfig:ContainerName"];
            string? clientId = _configuration["AzureStorageConfig:ClientId"];

            string containerEndPoint = string.Format("https://{0}.blob.core.windows.net/{1}", accountName, containerName);

            BlobContainerClient containerClient = new(new Uri(containerEndPoint),
                                                       new ManagedIdentityCredential(clientId));

            List<Rout> routeModels = new();

            foreach ((Stream Stream, string FileName) file in fileStreams)
            {
                string newFileName = $"{UserId}_map_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
                BlobClient blobClient = containerClient.GetBlobClient(newFileName);
                routeModels.Add(new Rout
                {
                    Id = UserId,
                    PhotoLocation = newFileName,
                    CreatedAt = DateTime.Now
                });

                // Stream を使ってアップロード
                await blobClient.UploadAsync(file.Stream, overwrite: true);
            }

            // データベースにファイル情報を登録
            _dbContext.Routs.AddRange(routeModels);
            await _dbContext.SaveChangesAsync();
        }


        // Avatar のアップロード
        private bool _isUploading = false;
        public async Task<bool> UploadAvatarAsync(string fileName, IBrowserFile file)
        {
            if (string.IsNullOrEmpty(fileName) || file == null)
            {
                Console.WriteLine("File name or file is null or empty.");
                return false;
            }

            try
            {
                BlobClient blobClient = GetBlobClient(fileName);
                using Stream stream = file.OpenReadStream(maxAllowedSize: 5242880);
                await blobClient.UploadAsync(stream, overwrite: true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Avatar upload failed: {ex.Message}");
                return false;
            }
        }

        private BlobClient GetBlobClient(string fileName)
        {
            string? accountName = _configuration["AzureStorageConfig:AccountName"];
            string? containerName = _configuration["AzureStorageConfig:ContainerName"];
            string? clientId = _configuration["AzureStorageConfig:ClientId"];

            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("Azure Storage configuration is incomplete.");
            }

            string containerEndPoint = string.Format("https://{0}.blob.core.windows.net/{1}", accountName, containerName);
            BlobContainerClient containerClient = new(new Uri(containerEndPoint), new ManagedIdentityCredential(clientId));
            return containerClient.GetBlobClient(fileName);
        }


        // BlobStorageService のメソッド
        public async Task DeleteFileAsync(BlobFileInfo file, int userId)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "ファイル情報が null です。");
            }

            string? accountName = _configuration["AzureStorageConfig:AccountName"];
            string? containerName = _configuration["AzureStorageConfig:ContainerName"];
            string? clientId = _configuration["AzureStorageConfig:ClientId"];

            string containerEndPoint = string.Format("https://{0}.blob.core.windows.net/{1}", accountName, containerName);

            BlobContainerClient containerClient = new(new Uri(containerEndPoint),
                                                                          new ManagedIdentityCredential(clientId));

            BlobClient blobClient = containerClient.GetBlobClient(file.Name);

            // BLOB ファイルを削除
            await blobClient.DeleteIfExistsAsync();

            // データベースからエントリを削除
            Rout? route = await _dbContext.Routs.FirstOrDefaultAsync(r => r.UserId == userId && r.PhotoLocation == file.Name);
            if (route != null)
            {
                _dbContext.Routs.Remove(route);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    public class BlobFileInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
