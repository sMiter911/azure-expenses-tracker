using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Data.Interfaces
{
    public interface ICosmosAttachmentsRepository
    {
        Task AddAsync(Attachment attachment);

        Task<IEnumerable<Attachment>> GetUnusedAttachementsAsync(int hours);

        Task DeleteAsync(string id, string uploadedByUserId);    
    }
}
