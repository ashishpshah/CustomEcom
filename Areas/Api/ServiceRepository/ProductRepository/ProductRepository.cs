using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.ProductRepository
{
    public class ProductRepository : IProductRepository
    {

        public async Task<object> GetAllProduct(PagingRequest request)
        {
            var oParams = new List<SqlParameter>()
    {
        new SqlParameter("@Id", DBNull.Value),
        new SqlParameter("@Search", request.Search),
        new SqlParameter("@Start", request.Start),
        new SqlParameter("@Length", request.Length),
        new SqlParameter("@SortColumnIndex", request.SortColumnIndex),
        new SqlParameter("@SortDirection", request.SortDirection)
    };

            DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Product_Get", oParams);

            var table1 = new List<Dictionary<string, object>>();
            var table2 = new List<Dictionary<string, object>>();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }

                    table1.Add(dict);
                }
            }

            if (ds.Tables.Count > 1)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in ds.Tables[1].Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }

                    table2.Add(dict);
                }
            }

            var result = new
            {
                table1 = table1,
                table2 = table2
            };

            return await Task.FromResult(result);
        }
        public async Task<Product?> GetProductById(int id)
        {
            try
            {
                Product? product = null;

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", id)
                };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Product_Get", oParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var dr = ds.Tables[0].Rows[0];

                    product = new Product()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        ProductName = dr["ProductName"]?.ToString(),
                        CategoryId = dr["CategoryId"] != DBNull.Value ? Convert.ToInt32(dr["CategoryId"]) : 0,
                        CategoryName = dr["CategoryName"]?.ToString(),
                        Price =     Convert.ToDecimal(dr["Price"]),
                        ImagePath = dr["ImagePath"]?.ToString(),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        ProductVariantMapping = new List<ProductVariantMapping>()
                    };

                    if (ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                    {
                        product.ProductImages = new List<ProductImages>();

                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            product.ProductImages.Add(new ProductImages()
                            {
                                Id = Convert.ToInt32(row["Id"]),
                                ImagePath = row["ImagePath"]?.ToString(),
                                IsPrimary = Convert.ToBoolean(row["IsPrimary"]),
                                VariantId = Convert.ToInt32(row["VariantId"])
                            });
                        }
                    }
                }

                return await Task.FromResult(product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveProduct(Product product)
        {
            try
            {
                List<string> productImages = new();

                if (product.ProductImages != null)
                {
                    bool isFirst = true;

                    foreach (var img in product.ProductImages)
                    {
                        productImages.Add($"{img.Id}|{img.ImagePath}|{(isFirst ? 1 : 0)}|{(img.IsRemove ? 1 : 0)}");
                        isFirst = false;
                    }
                }

                string imagesString = string.Join("||", productImages);

                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", product.Id),
                    new SqlParameter("CategoryId", product.CategoryId),
                    new SqlParameter("ProductName", product.ProductName),
                    new SqlParameter("ProductDescription", product.ProductDescription),
                    new SqlParameter("SKU", product.SKU),
                    new SqlParameter("Price", product.Price),
                    new SqlParameter("ProductImages", imagesString),
                    new SqlParameter("IsActive", product.IsActive ? 1 : 0),
                    new SqlParameter("Mode", "SAVE"),
                    new SqlParameter("OperatedBy", 1)
                };

                var result = DataContext.ExecuteStoredProcedure("SP_Product_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving product", ex);
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteProduct(long id)
        {
            try
            {
                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Mode", "DELETE"),
                    new SqlParameter("OperatedBy", Common.LoggedUser_Id())
                };

                var result = DataContext.ExecuteStoredProcedure("SP_Product_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch
            {
                throw;
            }
        }
    }
}
