using System.Collections.Generic;
using DashAccountingSystem.Extensions;

namespace DashAccountingSystem.Data.Models
{
    public class PagedResult<TModel> where TModel : class
    {
        public Pagination Pagination { get; set; }

        public IEnumerable<TModel> Results { get; set; }

        public PagedResult()
        {
            Results = new List<TModel>();
        }

        public bool HasAny()
        {
            return Results.HasAny();
        }
    }
}
