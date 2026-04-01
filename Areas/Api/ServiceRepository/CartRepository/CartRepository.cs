using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Areas.Api.ServiceRepository.CartRepository;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;


    public class CartRepository : ICartRepository
{
    public async Task<(bool IsSuccess, string Message, long Id, List<Dictionary<string, object>> Data)> SaveCart(CartRequest obj)
    {
        try
        {
            List<SqlParameter> oParams = new()
                {
                    new SqlParameter("@CustomerId", obj.CustomerId),
                    new SqlParameter("@ProductId", obj.ProductId),
                    new SqlParameter("@VariantId", obj.VariantId),
                    new SqlParameter("@Quantity", obj.Quantity)
                };

            DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_AddToCart", oParams);

           
            var cartList = new List<Dictionary<string, object>>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }

                    cartList.Add(dict);
                }

                return (true, "Cart updated successfully", 0, cartList);
            }
            else
            {
                return (false, "No data found", 0, null);
            }
        }
        catch (SqlException ex)
        {
            return (false, ex.Message, 0, null);
        }
        catch (Exception)
        {
            return (false, "Something went wrong", 0, null);
        }
    }
    public async Task<(bool IsSuccess, long Id)> RemoveCart(CartRequest obj)
    {
        try
        {
            List<SqlParameter> oParams = new()
        {
            new SqlParameter("@CustomerId", obj.CustomerId),
            new SqlParameter("@ProductId", obj.ProductId),
            new SqlParameter("@VariantId", obj.VariantId),
            new SqlParameter("@Quantity", (object?)obj.Quantity ?? DBNull.Value)
        };

            DataContext.ExecuteStoredProcedure("SP_RemoveFromCart", oParams, false);

            // If no exception → success
            return await Task.FromResult((true, 0));
        }
        catch (Exception ex)
        {
            // Optional: log error
            return await Task.FromResult((false, 0));
        }
    }
    public async Task<object> ApplyOffersToCart(OfferRequest obj)
    {
        var oParams = new List<SqlParameter>()
            {
                 new SqlParameter("@CustomerId", obj.CustomerId),
            new SqlParameter("@CouponCode", obj.CouponCode),
            new SqlParameter("@SelectedOfferId", obj.SelectedOfferId),
            new SqlParameter("@RemoveOfferId", (object?)obj.RemoveOfferId ?? DBNull.Value)
            };

        DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_ApplyOffersToCart", oParams);

        var table1 = new List<Dictionary<string, object>>();
        var table2 = new List<Dictionary<string, object>>();
        var table3 = new List<Dictionary<string, object>>();

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
        if (ds.Tables.Count > 2)
        {
            foreach (DataRow row in ds.Tables[2].Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in ds.Tables[2].Columns)
                {
                    dict[col.ColumnName] = row[col];
                }

                table3.Add(dict);
            }
        }
        var result = new
        {
            table1 = table1,
            table2 = table2,
            table3 = table3
        };

        return await Task.FromResult(result);
    }
    public async Task<(bool IsSuccess, object Data)> Checkout(int customerId, int createdBy)
    {
        try
        {
            List<SqlParameter> oParams = new()
        {
            new SqlParameter("@CustomerId", customerId),
            new SqlParameter("@CreatedBy", createdBy)
        };

            DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Checkout", oParams);

            var table1 = new List<Dictionary<string, object>>();
            var table2 = new List<Dictionary<string, object>>();

            //long orderId = 0;
            //string orderNumber = string.Empty;

            // 🔷 Table 0 (Order Header)
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                    }

                    table1.Add(dict);

                 
                }
            }

            // 🔷 Table 1 (Order Items)
            if (ds != null && ds.Tables.Count > 1)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in ds.Tables[1].Columns)
                    {
                        dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                    }

                    table2.Add(dict);
                }
            }

            var result = new
            {
                Order = table1,
                Items = table2
            };

            return await Task.FromResult((true, result));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}



