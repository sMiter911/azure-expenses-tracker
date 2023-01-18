using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Functions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AKExpensesTracker.Server.Functions
{
    public class DeleteUnusedAttachements
    {
        private readonly IStorageServices _storageServices;
        private readonly ICosmosAttachmentsRepository _attachmentsRepository;

        public DeleteUnusedAttachements(IStorageServices storageServices, ICosmosAttachmentsRepository attachmentsRepository)
        {
            _storageServices = storageServices;
            _attachmentsRepository = attachmentsRepository;
        }

        [FunctionName("DeleteUnusedAttachements")]
        public async Task Run([TimerTrigger("0 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Delete Unused Attachements triggered at : {DateTime.Now}");

            // Retrieve the files to be delete
            var attachments = await _attachmentsRepository.GetUnusedAttachementsAsync(6);

            if (attachments.Any())
            {
                log.LogInformation($"{attachments.Count()} Attachements to be deleted");
                int deletedCount = 0;
                foreach(var item in attachments)
                {
                    var fileName = Path.GetFileName(item.Url);
                    try
                    {
                        await _storageServices.DeleteFileAsync(fileName);
                        await _attachmentsRepository.DeleteAsync(item.Id, item.UploadedByUserId);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        log.LogError($"Error deleting the file {fileName} ", ex);
                    }
                }
                log.LogInformation($"{deletedCount}/{attachments.Count()} attachments have been deleted succesfully!");
            }
            else
            {
                log.LogInformation($"No unused attachements found!");
            }
        }
    }
}
