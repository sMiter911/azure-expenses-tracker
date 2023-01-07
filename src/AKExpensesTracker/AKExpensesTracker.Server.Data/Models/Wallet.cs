using System.Text.Json.Serialization;

namespace AKExpensesTracker.Server.Data.Models
{
    public class Wallet
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName("type")]
        public string? TypeName { get; set; }
        public WalletType? Type => GetWalletTypeFromString(TypeName);

        [JsonPropertyName("bankName")]
        public string? BankName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("iban")]
        public string? Iban { get; set; }

        [JsonPropertyName("accountType")]
        public string? AccountType { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("swift")]
        public string? Swift { get; set; }

        [JsonPropertyName("balance")]
        public int Balance { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        private WalletType? GetWalletTypeFromString(string? typeName)
        {
            return typeName switch
            {
                "BankAccount" => WalletType.BankAccount,
                "Cash" => WalletType.Cash,
                "CreditCard" => WalletType.CreditCard,
                "DebitCard" => WalletType.DebitCard,
                "Other" => WalletType.Other,
                _ => null,
            };
        }
    }
}
