using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace OrderDeliverySystem.Middleware
{
    public class BlobService
    {
        private readonly string _blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=shuting;AccountKey=TuucCZnM09Temqt0lWRfbE8Z1Pj4UXfJ9ZSWBQMdZZveLyz9yiXNSlPfuGb88auJE/V/fRMXqSb1+AStkuuw0A==;EndpointSuffix=core.windows.net";
        private readonly string _containerName = "images";

        public string GetSasTokenForBlob(string blobName)
        {
            var blobServiceClient = new BlobServiceClient(_blobStorageConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Ensure container exists
            containerClient.CreateIfNotExists();

            var blobClient = containerClient.GetBlobClient(blobName);

            // Generate a SAS Token valid for a limited time
            var sasToken = blobClient.GenerateSasUri(BlobSasPermissions.Write | BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));

            return sasToken.ToString();
        }
    }
}
