using System.Collections.Generic;
using TRMDataManager.Library.Models;
using TRMDataManagerLibrary.Models;

namespace TRMDataManager.Library.DataAccess
{
    public interface ISaleData
    {
        List<SaleReportModel> GetSaleReport();
        void SaveSale(SaleModel saleInfo, string cashierId);
    }
}