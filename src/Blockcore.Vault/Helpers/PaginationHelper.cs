using Blockcore.Vault.Filters;
using Blockcore.Vault.Models;
using Blockcore.Vault.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Helpers
{
    public static class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilter validFilter, int totalRecords, IUriService uriService, string route)
        {
            var respose = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            respose.Next =
                validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route)
                : null;
            respose.Previous =
                validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route)
                : null;

            respose.First = uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route);
            respose.Last = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route);
            respose.Pages = roundedTotalPages;
            respose.Total = totalRecords;

            return respose;
        }
    }
}
