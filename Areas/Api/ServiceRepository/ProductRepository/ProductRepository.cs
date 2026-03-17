using Azure.Core;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text.RegularExpressions;

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
                        CategoryId = Convert.ToInt32(dr["CategoryId"]),
                        CategoryName = dr["CategoryName"]?.ToString(),
                        Price =     Convert.ToDecimal(dr["Price"]),
                        //ImagePath = dr["ImagePath"]?.ToString(),
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
                    if (ds.Tables.Count > 0 && ds.Tables[2].Rows.Count > 0)
                    {
                        var dt = ds.Tables[2];
                        DataTable dtDetails = ds.Tables[3];

                        product.ProductVariantMapping = new List<ProductVariantMapping>();

                        foreach (DataRow row in dt.Rows)
                        {
                            var details = dtDetails.AsEnumerable()
                                .Where(x => Convert.ToInt32(x["VariantId"]) == Convert.ToInt32(row["Id"]))
                                .Select(x => new ProductVariantDetails()
                                {
                                    Id = Convert.ToInt32(x["Id"]),
                                    VariantId = Convert.ToInt32(x["VariantId"]),
                                    AttributeId = Convert.ToInt32(x["AttributeId"]),
                                    AttributeName = x["AttributeName"].ToString(),
                                    AttributeValueId = Convert.ToInt32(x["AttributeValueId"]),
                                    AttributeValueName = Convert.ToInt32(x["AttributeId"]) + "-" + x["AttributeValueId"].ToString()
                                })
                                .ToList();

                            product.ProductVariantMapping.Add(new ProductVariantMapping()
                            {
                                Id = Convert.ToInt32(row["Id"]),
                                Price = Convert.ToDecimal(row["Price"]),
                                SKU =  row["SKU"].ToString(),
                                ProductVariantDetails = details
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



        public async Task<List<DropdownModel?>> GetCategory_Dropdown()
        {
            try
            {
                List<DropdownModel?> obj = new List<DropdownModel?>();

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", -1)
                };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Category_Get", oParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        obj.Add(new DropdownModel()
                        {
                            Value = row["Id"].ToString(),
                            Text = (string.IsNullOrEmpty(row["ParentCategoryName"].ToString()) ? row["CategoryName"].ToString() : row["CategoryName"].ToString() + " - " + row["ParentCategoryName"].ToString()),
                            Value2 = row["ParentCategoryId"].ToString(),
                            Group = "CAT"
                        });
                    };

                  
                }

                return await Task.FromResult(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DropdownModel?>> GetAttributeValues()
        {
            try
            {
                List<DropdownModel?> obj = new List<DropdownModel?>();

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", -2)
                };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Attribute_Get", oParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {


                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        obj.Add(new DropdownModel()
                        {
                            Value = row["Value"].ToString(),
                            Text = row["Text"].ToString(),
                            Group = "ATTR"
                        });
                    };


                }

                return await Task.FromResult(obj);
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
                string uploadFolder = Path.Combine(AppHttpContextAccessor.WebRootPath, "Uploads", "Product");
                List<string> productImages = new();
                if (product.ProductImages != null && product.ProductImages.Count > 0)
                {
                    bool isFirst = true;

                    foreach (var file in product.ProductImages)
                    {
                        //string imagePath = imageModel.ImagePath;
                        
                        var match = Regex.Match(file.ImageFile.Name, @"\[(\d+)\]");
                        int index = int.Parse(match.Groups[1].Value);

                        ProductImages imageModel = new ProductImages();

                        if (product.ProductImages?.Count > index && product.ProductImages[index] != null)
                            imageModel = product.ProductImages[index];

                        //string imagePath = await FileUploadService.UploadImageAsync(imageModel.ImageFile, uploadFolder);
                        string imagePath = null;
                        if (imageModel.ImageFile != null)
                        {
                            imagePath = await FileUploadService.UploadImageAsync(imageModel.ImageFile, uploadFolder);
                        }

                        productImages.Add($"{imageModel.Id}|{imagePath}|{(isFirst ? 1 : 0)}|{(imageModel.IsRemove ? 1 : 0)}");

                        isFirst = false;
                    }
                }
                if (product.ProductImages != null)
                {
                    foreach (var img in product.ProductImages.Where(x => x.Id > 0 && !productImages.Any(z => z.StartsWith(x.Id.ToString()))))
                    {
                        productImages.Add($"{img.Id}|-|{(img.IsPrimary ? 1 : 0)}|{(img.IsRemove ? 1 : 0)}");
                    }
                }

                string imagesString = string.Join("||", productImages);
                List<string> productVariant = new List<string>();
                if (product.ProductVariantMapping != null && product.ProductVariantMapping.Any(x => x.ProductVariantDetails != null))
                {
                    for (int i = 0; i < product.ProductVariantMapping.Count; i++)
                    {
                        var variantImage = "-";

                        if (product.ProductVariantMapping[i].ImageFile != null)
                        {
                            var file = product.ProductVariantMapping[i].ImageFile;
                            var match = Regex.Match(file.Name, @"\[(\d+)\]");
                            int index = int.Parse(match.Groups[1].Value);
                            variantImage = await FileUploadService.UploadImageAsync(file, uploadFolder);
                        }

                        productVariant.Add($"{product.ProductVariantMapping[i].Id}|{string.Join("=", product.ProductVariantMapping[i].ProductVariantDetails.Select(x => x.AttributeValueName).ToArray())}|" +
                                            $"{product.ProductVariantMapping[i].Price}|{product.ProductVariantMapping[i].SKU}|{variantImage}");
                    }
                }

                string variantString = string.Join("||", productVariant);
                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", product.Id),
                    new SqlParameter("CategoryId", product.CategoryId),
                    new SqlParameter("ProductName", product.ProductName),
                    new SqlParameter("ProductDescription", product.ProductDescription),
                    //new SqlParameter("SKU", product.SKU),
                    new SqlParameter("Price", product.Price),
                    new SqlParameter("ProductImages", imagesString),
                    new SqlParameter("ProductVariants", variantString),
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
