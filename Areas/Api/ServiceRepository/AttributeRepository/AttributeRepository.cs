using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.AttributeRepository;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;
using Attribute = JewelryStore.Areas.Admin.Models.Attribute;
namespace JewelryStore.Areas.Api.ServiceRepository.AttibuteRepository
{
    public class AttibuteRepository : IAttibuteRepository
    {
        public async Task<object> GetAllAttribute(PagingRequest request)
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

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Attribute_Get", oParams);

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

        public async Task<Attribute?> GetAttributeById(int id)
        {
            try
            {
                Attribute? attribute = null;

                var oParams = new List<SqlParameter>()
        {
            new SqlParameter("@Id", id)
        };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Attribute_Get", oParams);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    attribute = new Attribute()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Name = dr["Name"]?.ToString(),
                        DisplayName = dr["DisplayName"]?.ToString(),
                        Values = dr["AttributeValues"].ToString(),
                        Values_Ids = dr["AttributeValues_Ids"].ToString(),
                        IsActive = Convert.ToBoolean(dr["IsActive"])
                    };
                }

                return await Task.FromResult(attribute);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveAttribute(Attribute obj)
        {
            try
            {
                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", obj.Id),
                    new SqlParameter("Name", obj.Name),
                    new SqlParameter("DisplayName", obj.DisplayName),
                    new SqlParameter("@AttributeValues", obj.Values),
                    new SqlParameter("IsActive", obj.IsActive ? 1 : 0),
                    new SqlParameter("Mode", "SAVE"),
                    new SqlParameter("OperatedBy", 1)
                };

                var result = DataContext.ExecuteStoredProcedure("SP_Attribute_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving category", ex);
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteAttribute(long id, long operatedBy)
        {
            try
            {
                List<SqlParameter> oParams = new()
        {
            new SqlParameter("Id", id),
            new SqlParameter("Mode", "DELETE"),
            new SqlParameter("OperatedBy", operatedBy)
        };

                var result = DataContext.ExecuteStoredProcedure("SP_Attribute_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


