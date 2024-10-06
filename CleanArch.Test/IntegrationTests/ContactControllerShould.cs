using CleanArch.Application.Interfaces;
using CleanArch.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;

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
            var transactionCardRepository = new TransactionCardRepository(_configuration);
            var postRepository = new PostRepository(_configuration);
            var unitofWork = new UnitOfWork(accountRepository, rankRepository, transactionRepository, transactionCardRepository, postRepository);
            _moqRepo = new Mock<IUnitOfWork>();
        }

        #endregion ===[ Constructor ]=================================================================
    }
}