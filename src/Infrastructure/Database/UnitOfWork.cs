using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Infrastructure.Database
{
    internal sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(IApplicationDbContext context)
        {
            _context = (DbContext)context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }
            await _transaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}