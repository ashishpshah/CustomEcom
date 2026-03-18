using Azure.Core;
using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.DTO;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
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
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> Register(Customer obj)
        {
            try
            {
                List<SqlParameter> oParams = new List<SqlParameter>();
                var Password = Common.Encrypt(obj.Password);

                oParams.Add(new SqlParameter("Id", obj.Id));

                oParams.Add(new SqlParameter("FirstName", obj.FirstName));
                oParams.Add(new SqlParameter("LastName", obj.LastName));

                oParams.Add(new SqlParameter("Email", obj.Email));
                oParams.Add(new SqlParameter("MobileNo", obj.MobileNo));

                oParams.Add(new SqlParameter("Password", string.IsNullOrEmpty(obj.Password) ? null : Common.Encrypt(obj.Password)));

                oParams.Add(new SqlParameter("DateOfBirth", obj.DateOfBirth ?? (object)DBNull.Value));
                oParams.Add(new SqlParameter("Gender", obj.Gender ?? (object)DBNull.Value));

                oParams.Add(new SqlParameter("AddressLine1", obj.AddressLine1));
                oParams.Add(new SqlParameter("AddressLine2", obj.AddressLine2));

                oParams.Add(new SqlParameter("City", obj.City));
                oParams.Add(new SqlParameter("State", obj.State));

                oParams.Add(new SqlParameter("PostalCode", obj.PostalCode));
                oParams.Add(new SqlParameter("Country", obj.Country));

                oParams.Add(new SqlParameter("IsActive", obj.IsActive ? 1 : 0));
                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", Common.LoggedUser_Id()));

                var result = DataContext.ExecuteStoredProcedure("SP_Customer_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving category", ex);
            }
        }
        public async Task<List<CustomerCart?>> ShoppingCartList_Get(int CustomerId = 0)
        {
            try
            {
                List<CustomerCart?> obj = new List<CustomerCart?>();   // FIX


                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", -1),
                    new SqlParameter("@CustomerId", CustomerId)
                };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Customer_Cart_Get", oParams);

                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        obj.Add(new CustomerCart()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            ProductId = Convert.ToInt32(row["ProductId"]),
                            VariantId = Convert.ToInt32(row["VariantId"]),
                            ProductName = row["ProductName"]?.ToString(),
                            SKU = row["SKU"]?.ToString(),
                            AttributeId = row["AttributeId"]?.ToString(),
                            AttributeName = row["AttributeName"]?.ToString(),
                            IsActive = Convert.ToBoolean(row["IsActive"]),
                            //CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                            //CreatedDate_Text = Convert.ToDateTime(row["CreatedDate"]).ToString("dd-MMM-yyyy")
                        });
                    }

                }

                return await Task.FromResult(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
