using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;
using TRMDataManagerLibrary;
using TRMDataManagerLibrary.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly IProductData _productData;
        private readonly ISqlDataAccess _sql;

        public SaleData(IProductData product, ISqlDataAccess sql)
        {
            this._productData = product;
            this._sql = sql;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: Make SOLID/DRY/Better

            // fill in the models which will be saved to the database
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();

            decimal taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                // get the information about this product
                var productInfo = _productData.GetProductById(item.ProductId);
                if (productInfo == null)
                {
                    throw new Exception($"The product of Id of {detail.ProductId } coule not be found in the database");

                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                details.Add(detail);
            }

            // crreate the sale model
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };
            sale.Total = sale.SubTotal + sale.Tax;

            // save the sale model



            try
            {
                _sql.StartTransaction("TRMData");
                _sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                // get the id from the sale model

                sale.Id = _sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { CashierId = sale.CashierId, SaleDate = sale.SaleDate }).FirstOrDefault();

                foreach (var item in details)
                {
                    item.SaleId = sale.Id;

                    // save the sale detail models
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                // finish the sale detail models
                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }

        }

        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

            return output;
        }

    }
}
