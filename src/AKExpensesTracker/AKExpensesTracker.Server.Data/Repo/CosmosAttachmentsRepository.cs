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


    }
}
