using CleanArch.Api.Controllers;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Repository;
using CleanArch.Test.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArch.Test.IntegrationTests
{
    [TestClass]
    public class ContactControllerShould
    {
        #region ===[ Private Members ]=============================================================

        protected readonly IConfigurationRoot _configuration;
        private readonly Mock<IUnitOfWork> _moqRepo;

        #endregion ===[ Private Members ]=============================================================

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactControllerShould()
        {
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                      .AddJsonFile("appsettings.json")
                                                      .Build();

            var accountRepository = new AccountRepository(_configuration);
            var rankRepository = new RankRepository(_configuration);
            var transactionRepository = new TransactionBanking(_configuration);
            var unitofWork = new UnitOfWork(accountRepository, rankRepository, transactionRepository);
            _moqRepo = new Mock<IUnitOfWork>();
        }

        #endregion ===[ Constructor ]=================================================================
    }
}