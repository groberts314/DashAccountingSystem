using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Extensions
{
    public static class PaginationExtensions
    {
        /// <summary>
		/// Extension method to get paged result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <param name="pagination"></param>
		/// <returns></returns>
		public static PagedResult<T> GetPaged<T>(this IQueryable<T> query, Pagination pagination)
            where T : class
        {
            var result = new PagedResult<T>();
            result.Pagination = pagination;
            result.Pagination.RecordCount = query.Count();

            // If pageNumber is null, default to the first page
            var pageNumber = pagination.PageNumber.HasValue ? (pagination.PageNumber.Value > 0 ? pagination.PageNumber.Value : 1) : 1;
            pagination.PageNumber = pageNumber;

            // If pageSize is null, defaul to the record count
            var pageSize = pagination.PageSize.HasValue ? (pagination.PageSize.Value > 0 ? pagination.PageSize.Value : result.Pagination.RecordCount) : result.Pagination.RecordCount;
            pagination.PageSize = pageSize;

            var pageCount = (double)result.Pagination.RecordCount / pageSize;
            result.Pagination.PageCount = (int)Math.Ceiling(pageCount);

            result.Pagination.ContainsMoreRecords = pagination.RecordCount > (pageNumber * pageSize);

            var skip = (pageNumber - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
    }
}
