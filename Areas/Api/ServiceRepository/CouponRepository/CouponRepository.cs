using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.CouponRepository;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.CouponRepository
{
    public class CouponRepository : ICouponRepository
    {
        public async Task<object> GetAllCoupon(PagingRequest request)
        {
            try
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

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Coupons_Get", oParams);

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

        public async Task<Coupons?> GetCouponById(int id)
        {
            try
            {
                Coupons? obj = null;

                var oParams = new List<SqlParameter>()
        {
            new SqlParameter("@Id", id)
        };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Coupons_Get", oParams);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    obj = new Coupons()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        CouponCode = dr["CouponCode"]?.ToString(),
                        DiscountType = dr["DiscountType"]?.ToString(),
                        DiscountValue = Convert.ToDecimal(dr["DiscountValue"]),
                        MinimumOrderAmount = Convert.ToDecimal(dr["MinimumOrderAmount"]),
                        ExpiryDate = Convert.ToDateTime(dr["ExpiryDate"]),
                        UsageLimit = Convert.ToInt32(dr["UsageLimit"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"])
                    };
                }

                return await Task.FromResult(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveCoupon(Coupons obj)
        {
            try
            {
                List<SqlParameter> oParams = new();
              
                oParams.Add(new SqlParameter("Id", obj.Id));
                oParams.Add(new SqlParameter("CouponCode", obj.CouponCode));
                oParams.Add(new SqlParameter("DiscountType", obj.DiscountType));
                oParams.Add(new SqlParameter("DiscountValue", obj.DiscountValue));
                oParams.Add(new SqlParameter("MinimumOrderAmount", obj.MinimumOrderAmount));
                oParams.Add(new SqlParameter("ExpiryDate", obj.ExpiryDate));
                oParams.Add(new SqlParameter("UsageLimit", obj.UsageLimit));
                oParams.Add(new SqlParameter("IsActive", obj.IsActive ? 1 : 0));

                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", Common.LoggedUser_Id()));

                var result = DataContext.ExecuteStoredProcedure("SP_Coupons_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving category", ex);
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteCoupon(long id)
        {
            try
            {
                List<SqlParameter> oParams = new()
        {
            new SqlParameter("Id", id),
            new SqlParameter("Mode", "DELETE"),
            new SqlParameter("OperatedBy", Common.LoggedUser_Id())
        };

                var result = DataContext.ExecuteStoredProcedure("SP_Coupons_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


