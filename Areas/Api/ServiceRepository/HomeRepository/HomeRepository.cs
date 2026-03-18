using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using JewelryStore.Areas.Api.DTO;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.HomeRepository
{
    public class HomeRepository : IHomeRepository
    {
        public async Task<List<DropdownModel?>> GetCategory_SubCategory_Dropdown(int ParentId = 0)
        {
            try
            {
                List<DropdownModel?> obj = new List<DropdownModel?>();

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@ParentId", ParentId)
                };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_DD_Category_Get", oParams);

                if (dt != null && dt.Rows.Count > 0)
                {


                    foreach (DataRow row in dt.Rows)
                    {
                        obj.Add(new DropdownModel()
                        {
                            Value = row["Id"].ToString(),
                            Text = row["CategoryName"].ToString(),
                            Group = row["Groupby"].ToString()
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
        public async Task<LoginResult> CustomerLogin(LoginRequest request)
        {
            try
            {
                LoginResult result = new LoginResult();
                var Password = Common.Encrypt(request.Password);
                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Username", request.Username),
                    new SqlParameter("@Password", Password)
                };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_CustomerLogin", oParams);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    result.Status = Convert.ToInt32(row["Status"]);
                    result.Message = row["Message"].ToString();

                    if (result.Status == 1)
                    {
                        var dict = new Dictionary<string, object>();

                        foreach (DataColumn col in row.Table.Columns)
                        {
                            dict[col.ColumnName] = row[col];
                        }

                        result.Data = dict;
                    }
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
