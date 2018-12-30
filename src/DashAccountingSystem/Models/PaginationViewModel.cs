using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class PaginationViewModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public Pagination ToModel()
        {
            return new Pagination() { PageNumber = PageNumber, PageSize = PageSize };
        }
    }
}
