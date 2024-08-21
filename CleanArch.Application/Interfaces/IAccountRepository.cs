using CleanArch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Application.Interfaces
{
    public interface IAccountRepository : IRepository<account>
    {
        Task<account?> Login(string username, string password);
        Task<bool> IsExist(string username);
        Task<bool> Register(string username, string password, string ip);
        Task<bool> ChangePassword(int id, string passwordNew);
    }
}
