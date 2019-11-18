using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Crumbs.EFCore.Session
{
    public interface ISessionScopedContextFactory<TContextInterface> where TContextInterface : IScopedContex
    {
        Task<TContextInterface> CreateContext<TInterface>(Guid? session = null);
        void Initialize(Func<DbContextOptions<SessionScopedContext>, TContextInterface> factoryMethod);
    }
}