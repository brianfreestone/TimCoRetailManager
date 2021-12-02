using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;
using TRMDataManagerLibrary;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: Make SOLID/DRY/Better

            // fill in the models which will be saved to the database
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();

            decimal taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                // get the information about this product
                var productInfo = products.GetProductById(item.ProductId);
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

            using (SqlDataAccess sql = new SqlDataAccess())
            {

                try
                {
                    sql.StartTransaction("TRMData");
                    sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                    // get the id from the sale model

                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { CashierId = sale.CashierId, SaleDate = sale.SaleDate }).FirstOrDefault();

                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;

                        // save the sale detail models
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }

                    // finish the sale detail models
                    // sql.CommitTransaction();
                }
                catch 
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }
        }


    }
}
