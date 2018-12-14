using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
