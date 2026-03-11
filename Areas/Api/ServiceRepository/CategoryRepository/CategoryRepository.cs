using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<List<Category>> GetAllCategory(PagingRequest request)
        {
            try
            {
                List<Category> list = new();

                var oParams = new List<SqlParameter>()
        {
            new SqlParameter("@Id", null),
            new SqlParameter("@Search", request.Search),
            new SqlParameter("@Start", request.Start),
            new SqlParameter("@Length", request.Length),
            new SqlParameter("@SortColumnIndex", request.SortColumnIndex),
            new SqlParameter("@SortDirection", request.SortDirection)
        };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Category_Get", oParams);

                if (ds != null && ds.Tables.Count > 1)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        list.Add(new Category()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            CategoryName = dr["CategoryName"]?.ToString(),
                            ParentCategoryId = dr["ParentCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["ParentCategoryId"]) : 0,
                            ParentCategoryName = dr["ParentCategoryName"]?.ToString(),
                            ImagePath = dr["ImagePath"]?.ToString(),
                            IsActive = Convert.ToBoolean(dr["IsActive"])
                        });
                    }
                }

                return await Task.FromResult(list);
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteCategory(long id, long operatedBy)
        {
            try
            {
                List<SqlParameter> oParams = new()
        {
            new SqlParameter("Id", id),
            new SqlParameter("Mode", "DELETE"),
            new SqlParameter("OperatedBy", operatedBy)
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


