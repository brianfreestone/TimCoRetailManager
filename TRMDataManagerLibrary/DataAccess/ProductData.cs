using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess _sql;

        public ProductData(ISqlDataAccess sql)
        {
            this._sql = sql;
        }
        public List<ProductModel> GetProducts()
        {

            var products = _sql.LoadData<ProductModel, dynamic>("spProduct_GetAll", new { }, "TRMData");

            return products;
        }

        public ProductModel GetProductById(int productId)
        {

            var product = _sql.LoadData<ProductModel, dynamic>("spProduct_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

            return product;
        }

    }
}
