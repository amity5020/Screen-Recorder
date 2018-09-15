using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace ScreenRecorderNew
{
    public class ReturnData
    {
        public bool error { get; set; }
        public bool isLastBlock { get; set; }
        public string message { get; set; }
    }
    public class CloudFile
    {
        public string FileName { get; set; }
        public string URL { get; set; }
        public long Size { get; set; }
        public long BlockCount { get; set; }
        public CloudBlockBlob BlockBlob { get; set; }
        public DateTime StartTime { get; set; }
        public string UploadStatusMessage { get; set; }
        public bool IsUploadCompleted { get; set; }
        public string AssetId { get; set; }
        public static CloudFile CreateFromIListBlobItem(IListBlobItem item)
        {
            if (item is CloudBlockBlob)
            {
                var blob = (CloudBlockBlob)item;
                return new CloudFile
                {
                    FileName = blob.Name,
                    URL = blob.Uri.ToString(),
                    Size = blob.Properties.Length
                };
            }
            return null;
        }
    }
   
    class UploadToAzure
    {
        
        /// <summary>
        /// this method creates blob file and its metadata
        /// </summary>
        /// <param name="blocksCount">in how many blocks file will be pushed</param>
        /// <param name="fileName">name of file</param>
        /// <param name="fileSize">size of file</param>
        /// <param name="AssetIds">optional can be empty</param>
        /// <returns></returns>
        public CloudFile SetMetadata(int blocksCount, string fileName, long fileSize, string AssetIds)
        {
            var container = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=videostoraged1;AccountKey=8vWyv5J4XOgk6ymkdLdunZV6tdhVMC1qCu59gFADVKJzfhtklIZkMP0KJrb+KtdJSgNOv4R2KKn/dN3Mg+SiiQ==;EndpointSuffix=core.windows.net").CreateCloudBlobClient()
                .GetContainerReference("video");
            container.CreateIfNotExists();
            var fileToUpload = new CloudFile()
            {
                AssetId = AssetIds,
                BlockCount = blocksCount,
                FileName = fileName,
                Size = fileSize,
                BlockBlob = container.GetBlockBlobReference(fileName),
                StartTime = DateTime.Now,
                IsUploadCompleted = false,
                UploadStatusMessage = string.Empty
            };
            Program.cloudFile = fileToUpload;
            return fileToUpload;
        }


        /// <summary>
        /// This method upload chunks
        /// </summary>
        /// <param name="id">this is serial no of chunk</param>
        /// <param name="chunk">byte array of file slice</param>
        /// <returns></returns>
        public ReturnData UploadChunk(int id,byte[] chunk)
        {
            // HttpPostedFileBase request = Request.Files["Slice"];
            //byte[] chunk = new byte[request.ContentLength];
            //request.InputStream.Read(chunk, 0, Convert.ToInt32(request.ContentLength));
            ReturnData returnData = null;
           // string fileSession = "CurrentFile";
            if (Program.cloudFile != null)
            {
                CloudFile model = Program.cloudFile;
                //  model.AssetId = AssetId;
                returnData = UploadCurrentChunk(model, chunk, id);
                if (returnData != null)
                {
                    return returnData;
                }
                if (id == model.BlockCount)
                {
                    return CommitAllChunks(model);
                }
            }
            else
            {
                returnData = new ReturnData();
                returnData.error = true;
                returnData.isLastBlock = false;
                returnData.message = string.Format(CultureInfo.CurrentCulture, "Failed to Upload file.", "Session Timed out");
               
                return returnData;
            }
            return new ReturnData{ error = false, isLastBlock = false, message = string.Empty };
        }

        /// <summary>
        /// This method commite all chunk to blob file
        /// </summary>
        /// <param name="model">Takes an argument of cloud file to be uploaded.</param>
        /// <returns></returns>
        private ReturnData CommitAllChunks(CloudFile model)
        {
            model.IsUploadCompleted = true;
            bool errorInOperation = false;
            try
            {
                var blockList = Enumerable.Range(1, (int)model.BlockCount).ToList<int>().ConvertAll(rangeElement =>
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                string.Format(CultureInfo.InvariantCulture, "{0:D4}", rangeElement))));
                model.BlockBlob.PutBlockList(blockList);
                var duration = DateTime.Now - model.StartTime;
                float fileSizeInKb = model.Size / 1024;
                string fileSizeMessage = fileSizeInKb > 1024 ?
                    string.Concat((fileSizeInKb / 1024).ToString(CultureInfo.CurrentCulture), " MB") :
                    string.Concat(fileSizeInKb.ToString(CultureInfo.CurrentCulture), " KB");
                model.UploadStatusMessage = string.Format(CultureInfo.CurrentCulture,
                    "File uploaded successfully. {0} took {1} seconds to upload",
                    fileSizeMessage, duration.TotalSeconds);
                ///CreateMediaAsset(model);
            }
            catch (StorageException e)
            {
                model.UploadStatusMessage = "Failed to Upload file. Exception - " + e.Message;
                errorInOperation = true;
            }
            finally
            {
                // Session.Remove("CurrentFile");
            }
            return new ReturnData
            {
                error = errorInOperation,
                isLastBlock = model.IsUploadCompleted,
                message = model.UploadStatusMessage
                //assetId = model.AssetId
            };
        }

        /// <summary>
        /// this method finally upload chunk from memory stream
        /// </summary>
        /// <param name="model"> cloud file that hes to be uploaded</param>
        /// <param name="chunk">current chunk</param>
        /// <param name="id">serial no of chunk</param>
        /// <returns></returns>
        private ReturnData UploadCurrentChunk(CloudFile model, byte[] chunk, int id)
        {
            using (var chunkStream = new MemoryStream(chunk))
            {
                var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        string.Format(CultureInfo.InvariantCulture, "{0:D4}", id)));
                try
                {
                    model.BlockBlob.PutBlock(
                        blockId,
                        chunkStream, null, null,
                        new BlobRequestOptions()
                        {
                            RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(10), 3)
                        },
                        null);
                    return null;
                }
                catch (StorageException e)
                {

                    model.IsUploadCompleted = true;
                    model.UploadStatusMessage = "Failed to Upload file. Exception - " + e.Message;
                    ReturnData returnData = new ReturnData{
                        error = true,
                        isLastBlock = false,
                        message = model.UploadStatusMessage
                    };
                    return returnData;
                }
            }
        }
    }
}
