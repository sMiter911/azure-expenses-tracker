using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Data.Repo
{
    public class CosmosAttachmentsRepository : ICosmosAttachmentsRepository
    {
        private readonly CosmosClient _db;
        private const string DATABASE_NAME = "ExpenseTrackerDb";
        private const string CONTAINER_NAME = "Attachments";

        public CosmosAttachmentsRepository(CosmosClient db)
        {
            _db = db;
        }

        #region
        public async Task AddAsync(Attachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
            await container.CreateItemAsync(attachment);
        }

        #endregion

        #region
        public async Task<IEnumerable<Attachment>> GetUnusedAttachementsAsync(int hours)
        {
            var queryText = $"SELECT * FROM c WHERE DateTimeDiff('hour', c.uploadedDate, GetCurrentDateTime()) > @hours";
            var query = new QueryDefinition(queryText).WithParameter("@hours", hours);

            var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

            var iterator = container.GetItemQueryIterator<Attachment>(query);
            var result = await iterator.ReadNextAsync();
            var attachements = new List<Attachment>();

            if (result.Any())
            {
                attachements.AddRange(result.Resource);
            }

            while (result.ContinuationToken != null)
            {
                iterator = container.GetItemQueryIterator<Attachment>(query, result.ContinuationToken);
                result = await iterator.ReadNextAsync();
                attachements.AddRange(result.Resource);
            }

            return attachements;
        }
        #endregion


        #region
        public async Task DeleteAsync(string id, string uploadedByUserId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (string.IsNullOrWhiteSpace(uploadedByUserId))
            {
                throw new ArgumentNullException(nameof(uploadedByUserId));
            }


            var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);
            await container.DeleteItemAsync<Attachment>(id, new PartitionKey(uploadedByUserId));
        }
        #endregion

    }
}
