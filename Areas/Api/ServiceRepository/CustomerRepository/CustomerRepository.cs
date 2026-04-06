using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.CustomerRepository;
using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {
        public async Task<object> GetAllCustomer(ReviewPagingRequest request)
        {
            try
            {
                var oParams = new List<SqlParameter>()
        {
           new SqlParameter("@Id", request.Id == 0 ? DBNull.Value : request.Id),
            new SqlParameter("@Search", request.Search),
            new SqlParameter("@Start", request.Start),
            new SqlParameter("@Length", request.Length),
            new SqlParameter("@SortColumnIndex", request.SortColumnIndex),
            new SqlParameter("@SortDirection", request.SortDirection)
        };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Customer_Get", oParams);

                var table1 = new List<Dictionary<string, object>>();
                var table2 = new List<Dictionary<string, object>>();
                var table3 = new List<Dictionary<string, object>>();
                var table4 = new List<Dictionary<string, object>>();
                var table5 = new List<Dictionary<string, object>>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    // Table 0
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                            dict[col.ColumnName] = row[col];

                        table1.Add(dict);
                    }
                }

                if (ds != null && ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[1].Columns)
                            dict[col.ColumnName] = row[col];

                        table2.Add(dict);
                    }
                }

                if (ds != null && ds.Tables.Count > 2)
                {
                    foreach (DataRow row in ds.Tables[2].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[2].Columns)
                            dict[col.ColumnName] = row[col];

                        table3.Add(dict);
                    }
                }

                if (ds != null && ds.Tables.Count > 3)
                {
                    foreach (DataRow row in ds.Tables[3].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[3].Columns)
                            dict[col.ColumnName] = row[col];

                        table4.Add(dict);
                    }
                }

                if (ds != null && ds.Tables.Count > 4)
                {
                    foreach (DataRow row in ds.Tables[4].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[4].Columns)
                            dict[col.ColumnName] = row[col];

                        table5.Add(dict);
                    }
                }

                var result = new
                {
                    table1 = table1,
                    table2 = table2,
                    table3 = table3,
                    table4 = table4,
                    table5 = table5
                };

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Customer?> GetCustomerById(int id)
        {
            try
            {
                Customer? category = null;

                var oParams = new List<SqlParameter>()
        {
            new SqlParameter("@Id", id)
        };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Customer_Get", oParams);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    category = new Customer()
                    {
                        Id = GetValue<int>(dr, "Id"),

                        FirstName = GetValue<string>(dr, "FirstName"),
                        LastName = GetValue<string>(dr, "LastName"),

                        Email = GetValue<string>(dr, "Email"),
                        MobileNo = GetValue<string>(dr, "MobileNo"),

                        DateOfBirth = GetValue<DateTime?>(dr, "DateOfBirth"),
                        Gender = GetValue<string>(dr, "Gender"),

                        AddressLine1 = GetValue<string>(dr, "AddressLine1"),
                        AddressLine2 = GetValue<string>(dr, "AddressLine2"),

                        City = GetValue<string>(dr, "City"),
                        State = GetValue<string>(dr, "State"),
                        PostalCode = GetValue<string>(dr, "PostalCode"),
                        Country = GetValue<string>(dr, "Country"),

                        IsActive = GetValue<bool>(dr, "IsActive")
                    };
                }

                return await Task.FromResult(category);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TModel GetValue<TModel>(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
                return default;

            Type targetType = Nullable.GetUnderlyingType(typeof(TModel)) ?? typeof(TModel);
            return (TModel)Convert.ChangeType(row[columnName], targetType);
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> AddOrUpdateCustomer(Customer customer)
        {
            try
            {
                List<SqlParameter> parameters = new()
        {
            new SqlParameter("@Id", customer.Id),
            new SqlParameter("@CustomerId", customer.CustomerId),
            new SqlParameter("@AddressLine1", customer.AddressLine1 ?? (object)DBNull.Value),
            new SqlParameter("@AddressLine2", customer.AddressLine2 ?? (object)DBNull.Value),
            new SqlParameter("@City", customer.City ?? (object)DBNull.Value),
            new SqlParameter("@State", customer.State ?? (object)DBNull.Value),
            new SqlParameter("@PostalCode", customer.PostalCode ?? (object)DBNull.Value),
            new SqlParameter("@Country", customer.Country ?? (object)DBNull.Value),
            new SqlParameter("@MobileNo", customer.MobileNo ?? (object)DBNull.Value),
            new SqlParameter("@AlternativeMobileNo", customer.AlternativeMobileNo ?? (object)DBNull.Value),
            new SqlParameter("@AddressType", customer.AddressType ?? (object)DBNull.Value),
            new SqlParameter("@CustomerName", customer.CustomerName ?? (object)DBNull.Value),
              new SqlParameter("@Mode", "SAVE"),
            new SqlParameter("@Operated_By", customer.Id == 0 ? customer.CreatedBy : customer.LastModifiedBy)
        };

                var result = DataContext.ExecuteStoredProcedure(
                    "SP_CustomerAddress_Save",
                    parameters,
                    true
                );

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving customer address", ex);
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> RemoveCustomerAddress(long id)
        {
            try
            {
                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Mode", "DELETE")
                };

                var result = DataContext.ExecuteStoredProcedure("SP_CustomerAddress_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}


