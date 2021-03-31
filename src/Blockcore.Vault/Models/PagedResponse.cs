using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Uri First { get; set; }
        public Uri Last { get; set; }

        public int Pages { get; set; }
        public int Total { get; set; }
        public Uri Next { get; set; }
        public Uri Previous { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize)
        {
            this.Data = data;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;

            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
