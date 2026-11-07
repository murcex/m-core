namespace Configurator.Storage.Internal
{
    using Azure.Storage.Blobs;

    class StorageClient
    {
        /// <summary>
        /// Blob storage container.
        /// </summary>
        private static BlobContainerClient _blobContainer;

        /// <summary>
        /// Initilize storage blob client.
        /// </summary>
        /// <param name="storageAccount"></param>
        /// <param name="storageContainer"></param>
        /// <returns></returns>
        public static bool Initialize(string storageAccount, string storageContainer)
        {
            try
            {
                _blobContainer = new BlobContainerClient(storageAccount, storageContainer);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get a document from storage blob by file name.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The blob content, or null if it does not exist or cannot be read.</returns>
        public static byte[] GetDocument(string file)
        {
            try
            {
                return _blobContainer.GetBlobClient(file).DownloadContent().Value.Content.ToArray();
            }
            catch
            {
                return null;
            }
        }
    }
}
