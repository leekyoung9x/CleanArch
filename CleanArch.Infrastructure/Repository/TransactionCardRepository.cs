using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Microsoft.Extensions.Configuration;

namespace CleanArch.Infrastructure.Repository
{
    public class TransactionCardRepository : BaseRepository<transaction_card>, ITransactionCardRepository
    {
        public TransactionCardRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
