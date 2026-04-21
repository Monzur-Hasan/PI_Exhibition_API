using Dapper;
using Domain.Models.Access.DomainModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Application.IdentityObject
{
    using Application.Db_Helper;
    using Microsoft.AspNetCore.Identity;

    public class CustomRoleStore :
        IRoleStore<IdentityRole<Guid>>
    {
        private readonly DbHelper _db;

        public CustomRoleStore(DbHelper db)
        {
            _db = db;
        }

        public Task<IdentityResult> CreateAsync(
            IdentityRole<Guid> role, CancellationToken cancellationToken)
        {
            // insert role
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(
            IdentityRole<Guid> role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(
            IdentityRole<Guid> role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityRole<Guid>> FindByIdAsync(
            string roleId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IdentityRole<Guid>>(null);
        }

        public Task<IdentityRole<Guid>> FindByNameAsync(
            string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult<IdentityRole<Guid>>(null);
        }

        public Task<string> GetRoleIdAsync(
            IdentityRole<Guid> role, CancellationToken cancellationToken)
            => Task.FromResult(role.Id.ToString());

        public Task<string> GetRoleNameAsync(
            IdentityRole<Guid> role, CancellationToken cancellationToken)
            => Task.FromResult(role.Name);

        public Task SetRoleNameAsync(
            IdentityRole<Guid> role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(
            IdentityRole<Guid> role, CancellationToken cancellationToken)
            => Task.FromResult(role.NormalizedName);

        public Task SetNormalizedRoleNameAsync(
            IdentityRole<Guid> role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }
}
