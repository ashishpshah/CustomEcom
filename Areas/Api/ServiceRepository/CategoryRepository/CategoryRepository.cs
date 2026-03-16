using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<object> GetAllCategory(PagingRequest request)
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

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Category_Get", oParams);

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

        public async Task<Category?> GetCategoryById(int id)
        {
            try
            {
                Category? category = null;

                var oParams = new List<SqlParameter>()
        {
            new SqlParameter("@Id", id)
        };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Category_Get", oParams);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    category = new Category()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        CategoryName = dr["CategoryName"]?.ToString(),
                        ParentCategoryId = dr["ParentCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["ParentCategoryId"]) : 0,
                        ParentCategoryName = dr["ParentCategoryName"]?.ToString(),
                        ImagePath = dr["ImagePath"]?.ToString(),
                        IsActive = Convert.ToBoolean(dr["IsActive"])
                    };
                }

                return await Task.FromResult(category);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveCategory(Category category)
        {
            try
            {
                List<SqlParameter> oParams = new()
        {
            new SqlParameter("Id", category.Id),
            new SqlParameter("CategoryName", category.CategoryName),
            new SqlParameter("ParentCategoryId", category.ParentCategoryId),
            new SqlParameter("@ImagePath", category.ImagePath),
            new SqlParameter("@ImagePath_Remove", category.ImagePath_Remove),
            new SqlParameter("IsActive", category.IsActive ? 1 : 0),
            new SqlParameter("Mode", "SAVE"),
            new SqlParameter("OperatedBy", 1)
        };

                var result = DataContext.ExecuteStoredProcedure("SP_Category_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving category", ex);
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteCategory(long id)
        {
            try
            {
                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Mode", "DELETE"),
                     new SqlParameter("OperatedBy", Common.LoggedUser_Id())
                };

                var result = DataContext.ExecuteStoredProcedure("SP_Category_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


