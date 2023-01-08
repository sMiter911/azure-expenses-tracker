using Newtonsoft.Json;

namespace AKExpensesTracker.Server.Data.Models
{
    public class Wallet
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("type")]
        public string? WalletType { get; set; }
        
        public WalletType? Type => GetWalletTypeFromString(WalletType);

        [JsonProperty("bankName")]
        public string? BankName { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("iban")]
        public string? Iban { get; set; }

        [JsonProperty("accountType")]
        public string? AccountType { get; set; }

        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("swift")]
        public string? Swift { get; set; }

        [JsonProperty("balance")]
        public int Balance { get; set; }

        [JsonProperty("currency")]
        public string? Currency { get; set; }

        [JsonProperty("userName")]
        public string? UserName { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("modificationDate")]
        public DateTime ModificationDate { get; set; }

        private WalletType? GetWalletTypeFromString(string? typeName)
        {
            return typeName switch
            {
                "Bank" => Shared.Enum.WalletType.Bank,
                "Cash" => Shared.Enum.WalletType.Cash,
                "CreditCard" => Shared.Enum.WalletType.CreditCard,
                "DebitCard" => Shared.Enum.WalletType.DebitCard,
                "Crypto" => Shared.Enum.WalletType.Crypto,
                "PayPal" => Shared.Enum.WalletType.PayPal,
                _ => Shared.Enum.WalletType.Other
            };
        }
    }
}
