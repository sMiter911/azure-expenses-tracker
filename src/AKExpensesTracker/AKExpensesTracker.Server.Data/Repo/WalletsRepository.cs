namespace AKExpensesTracker.Server.Data.Repo
{
    internal class WalletsRepository : IWalletsRepository
    {
        private readonly CosmosClient _db;
        private const string DATABASE_NAME = "ExpenseTrackerDb";
        private const string CONTAINER_NAME = "Wallets";

        public WalletsRepository(CosmosClient db)
        {
            _db = db;
        }

        #region List
        public async Task<IEnumerable<Wallet>> ListByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var queryText = $"SELECT * FROM c WHERE c.userId = @userId";
            var query = new QueryDefinition(queryText).WithParameter("@userId", userId);

            var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

            var iterator = container.GetItemQueryIterator<Wallet>(query);
            var result = await iterator.ReadNextAsync();

            return result.Resource;

        }
        #endregion

        #region Get By Id
        public async Task<Wallet?> GetByIdAsync(string walletId, string userId)
        {
            if (string.IsNullOrWhiteSpace(walletId))
            {
                throw new ArgumentNullException(nameof(walletId));
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var queryText = $"SELECT * FROM c WHERE c.id = @walletId AND c.userId = @userId";
            var query = new QueryDefinition(queryText)
                .WithParameter("@walletId", walletId)
                .WithParameter("@userId", userId);

            var container = _db.GetContainer(DATABASE_NAME, CONTAINER_NAME);

            var iterator = container.GetItemQueryIterator<Wallet>(query);
            var result = await iterator.ReadNextAsync();

            return result.Resource.FirstOrDefault();
        }
        #endregion
    }
}
