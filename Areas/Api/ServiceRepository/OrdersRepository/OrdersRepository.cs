using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.OrdersRepository
{
    public class OrdersRepository : IOrdersRepository
    {

        public async Task<object> GetAllOrders(PagingRequest request)
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

            DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Orders_Get", oParams);

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

            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in ds.Tables[1].Columns)
                    {
                        if (col.ColumnName == "OrderDate" && row[col] != DBNull.Value)
                        {
                            dict[col.ColumnName] = Convert.ToDateTime(row[col]).ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            dict[col.ColumnName] = row[col];
                        }
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
        public async Task<Orders?> GetOrderById(int id)
        {
            try
            {
                Orders? orders = null;

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", id)
                };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Orders_Get", oParams);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    orders = new Orders()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        OrderNumber = dr["OrderNumber"]?.ToString(),
                        OrderStatus = dr["OrderStatus"]?.ToString(),
                        OrderDate = Convert.ToDateTime(dr["OrderDate"]) ,
                        No_Of_Items = Convert.ToInt32(dr["No_Of_Items"]),
                        TotalAmount =     Convert.ToDecimal(dr["TotalAmount"]),
                        DiscountAmount =     Convert.ToDecimal(dr["DiscountAmount"]),
                        FinalAmount =     Convert.ToDecimal(dr["FinalAmount"]),                       
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        
                    };
                   
                }
                oParams = new List<SqlParameter>();
                oParams.Add(new SqlParameter("@Id", id));
                DataTable dt1 = DataContext.ExecuteStoredProcedure_DataTable("SP_Order_Detail_Get", oParams);

                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    orders.ListOrderItems = new List<OrderItems>();
                   

                    foreach (DataRow row in dt1.Rows)
                    {
                        orders.ListOrderItems.Add(new OrderItems()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            VariantId = Convert.ToInt32(row["VariantId"]),
                            ProductId = Convert.ToInt32(row["ProductId"]),
                            VariantName = row["VariantName"].ToString() ,
                            ProductName = row["ProductName"].ToString(),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                            TotalPrice = Convert.ToDecimal(row["TotalPrice"]),
                            IsActive = Convert.ToBoolean(row["IsActive"]),
                        });
                    }

                }
                return await Task.FromResult(orders);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Orders?> GetOrderNumber()
        {
            try
            {
                Orders? orders = null;

                var dt = new DataTable();
                dt = DataContext.ExecuteStoredProcedure_DataTable("SP_GenerateOrderNumber", null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    orders = new Orders()
                    {

                        OrderNumber = dr["OrderNumber"].ToString(),
                        OrderDate = DateTime.Now

                    };
                }


                return await Task.FromResult(orders);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveOrder(Orders obj)
        {
            try
            {
                DataTable order_items_table = new DataTable();
                order_items_table.Columns.Add("VariantId", typeof(int));
                order_items_table.Columns.Add("Quantity", typeof(int));
                order_items_table.Columns.Add("UnitPrice", typeof(decimal));
                order_items_table.Columns.Add("TotalPrice", typeof(decimal));

                if (obj != null && obj.ListOrderItems.Count > 0)
                {
                    foreach (var orderitems in obj.ListOrderItems)
                    {
                        order_items_table.Rows.Add(orderitems.VariantId, orderitems.Quantity, orderitems.UnitPrice, orderitems.TotalPrice);
                    }
                }
                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", obj.Id));
                oParams.Add(new SqlParameter("OrderNumber", obj.OrderNumber));
                oParams.Add(new SqlParameter("OrderDate", obj.OrderDate));
                oParams.Add(new SqlParameter("TotalAmount", obj.TotalAmount));
                oParams.Add(new SqlParameter("DiscountAmount", obj.DiscountAmount));
                oParams.Add(new SqlParameter("FinalAmount", obj.FinalAmount));
                oParams.Add(new SqlParameter("OrderItems", order_items_table));
                oParams.Add(new SqlParameter("IsActive", obj.IsActive ? 1 : 0));
                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", AppHttpContextAccessor.JwtUserId));

                var result = DataContext.ExecuteStoredProcedure("SP_Orders_Save", oParams, true);


                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving product", ex);
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteOrder(long id)
        {
            try
            {
                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Mode", "DELETE"),
                    new SqlParameter("OperatedBy", Common.LoggedUser_Id())
                };

                var result = DataContext.ExecuteStoredProcedure("SP_Orders_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch
            {
                throw;
            }
        }
    }
}
