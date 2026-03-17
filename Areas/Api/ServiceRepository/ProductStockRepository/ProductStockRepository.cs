using JewelryStore.Areas.Admin.Models;

using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.ProductStockRepository
{
    public class ProductStockRepository : IProductStockRepository
    {
        public async Task<object> GetAllProductStock(PagingRequest request)
        {
            try
            {
                var oParams = new List<SqlParameter>()
        {
            new SqlParameter("@ProductId", DBNull.Value),
            new SqlParameter("@Search", request.Search),
            new SqlParameter("@Start", request.Start),
            new SqlParameter("@Length", request.Length),
            new SqlParameter("@SortColumnIndex", request.SortColumnIndex),
            new SqlParameter("@SortDirection", request.SortDirection)
        };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_ProductStock_Get", oParams);

                var table1 = new List<Dictionary<string, object>>();
                var table2 = new List<Dictionary<string, object>>();

                // Counts
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
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

                //  Data
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> GetStockHistory(JsonParameters request)
        {
            try
            {
                var ProductId = request.ProductId;
                var VariantId = request.VariantId;
                ProductStock? attribute = null;

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@ProductId", ProductId),
            
          
                };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_ProductStock_Get", oParams);

                var table1 = new List<Dictionary<string, object>>();
                var table2 = new List<Dictionary<string, object>>();


                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
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

                //  Data
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AdjustStock(ProductStockHistory obj)
        {
            try
            {
                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("@ProductId", obj.ProductId));
                oParams.Add(new SqlParameter("@VariantIds", obj.VariantIds));
                oParams.Add(new SqlParameter("@ChangeType", obj.ChangeType));
                oParams.Add(new SqlParameter("@Quantity", obj.Quantity));
                oParams.Add(new SqlParameter("@Remarks", obj.Remarks));

                oParams.Add(new SqlParameter("@OperatedBy", Common.LoggedUser_Id()));

                var result = DataContext.ExecuteStoredProcedure("SP_ProductStock_Adjust", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving category", ex);
            }
        }

        //public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteAttribute(long id, long operatedBy)
        //{
        //    try
        //    {
        //        List<SqlParameter> oParams = new()
        //{
        //    new SqlParameter("Id", id),
        //    new SqlParameter("Mode", "DELETE"),
        //    new SqlParameter("OperatedBy", operatedBy)
        //};

        //        var result = DataContext.ExecuteStoredProcedure("SP_Attribute_Save", oParams, true);

        //        return await Task.FromResult(result);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}


