﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.CmsKit.EntityFrameworkCore;
using System.Linq;
using System.Data.Common;

namespace Volo.CmsKit.Blogs
{
    public class EfCoreBlogPostRepository : EfCoreRepository<CmsKitDbContext, BlogPost, Guid>, IBlogPostRepository
    {
        public EfCoreBlogPostRepository(IDbContextProvider<CmsKitDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public Task<BlogPost> GetByUrlSlugAsync(Guid blogId, string urlSlug, CancellationToken cancellationToken = default)
        {
            return GetAsync(x => x.BlogId == blogId && x.UrlSlug.ToLower() == urlSlug, cancellationToken: cancellationToken);
        }

        public async Task<List<BlogPost>> GetPagedListAsync(Guid blogId, int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
        {

            var queryable = (await GetQueryableAsync())
                    .Include(i => i.Creator)
                    .Where(x => x.BlogId == blogId);

            if (!sorting.IsNullOrWhiteSpace())
            {
                queryable = queryable.OrderBy(sorting);
            }

            return await queryable
                    .Skip(skipCount)
                    .Take(maxResultCount)
                    .ToListAsync(cancellationToken);
        }

        public async Task<bool> SlugExistsAsync(Guid blogId, string slug, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet.AnyAsync(x => x.BlogId == blogId && x.UrlSlug.ToLower() == slug, cancellationToken);
        }
    }
}
