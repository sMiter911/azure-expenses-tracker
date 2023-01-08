using AKExpensesTracker.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AKExpensesTracker.Shared.DTOs
{
    public class WalletDTO
    {
        public string? Id { get; set; }

        public WalletType Type { get; set; }

        public string? BankName { get; set; }

        public string? Name { get; set; }

        public string? Iban { get; set; }

        public string? AccountType { get; set; }
        

        public string? Swift { get; set; }

        public int Balance { get; set; }

        public string? Currency { get; set; }

        public string? UserName { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
 