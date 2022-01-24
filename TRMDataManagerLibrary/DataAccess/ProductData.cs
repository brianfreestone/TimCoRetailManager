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
    public class ProductData
    {
        private readonly IConfiguration _config;

        public ProductData(IConfiguration config)
        {
            this._config = config;
        }
        public List<ProductModel> GetProducts()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var products = sql.LoadData<ProductModel, dynamic>("spProduct_GetAll", new { }, "TRMData");

            return products;
        }

        public ProductModel GetProductById(int productId)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var product = sql.LoadData<ProductModel, dynamic>("spProduct_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

            return product;
        }

    }
}
