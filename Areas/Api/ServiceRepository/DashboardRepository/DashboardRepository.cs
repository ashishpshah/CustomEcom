using Azure.Core;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Core.Types;
using System.Data;
namespace JewelryStore.Areas.Api.ServiceRepository.DashboardRepository
{
    public class DashboardRepository : IDashboardRepository
    {
        public async Task<object> GetCustomerDashboard()
        {
            var CustomerId = AppHttpContextAccessor.JwtUserId;
            var oParams = new List<SqlParameter>()
            {
                new SqlParameter("@CustomerId", CustomerId)
            };

            DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_CustomerDashboard_Get", oParams);

            var table1 = new List<Dictionary<string, object>>();
            var table2 = new List<Dictionary<string, object>>();
            var table3 = new List<Dictionary<string, object>>();
            var table4 = new List<Dictionary<string, object>>();

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
            if (ds.Tables.Count > 3)
            {
                foreach (DataRow row in ds.Tables[3].Rows)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in ds.Tables[3].Columns)
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
                table3 = table3,
                table4 = table4
            };

            return await Task.FromResult(result);
        }
        public async Task<ChangePasswordResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                ChangePasswordResult result = new ChangePasswordResult();
                var CustomerId = AppHttpContextAccessor.JwtUserId;
                var OldPassword = Common.Encrypt(request.OldPassword);
                var NewPassword = Common.Encrypt(request.NewPassword);
                var oParams = new List<SqlParameter>()
              
                {
                    new SqlParameter("@UserId", CustomerId),
                    new SqlParameter("@OldPassword", OldPassword),
                    new SqlParameter("@NewPassword", NewPassword)
                };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_ChangePassword", oParams);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    result.Status = Convert.ToInt32(row["Status"]);
                    result.Message = row["Message"].ToString();

                   
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
