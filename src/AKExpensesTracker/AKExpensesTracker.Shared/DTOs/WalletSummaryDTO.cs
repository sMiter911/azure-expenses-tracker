using AKExpensesTracker.Shared.Enum;

namespace AKExpensesTracker.Shared.DTOs
{
    public class WalletSummaryDTO
    {
        public string? Id { get; set; }

        public WalletType Type { get; set; }

        public decimal Balance { get; set; }

        public string? Name { get; set; }

        public string? Currency { get; set; }
    }
}
 