using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.ReviewsRepository;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.ReviewsRepository
{
    public class ReviewsRepository : IReviewsRepository
    {
        public async Task<object> GetAllReviews(ReviewPagingRequest request)
        {
            try
            {
                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", request.Id == 0?DBNull.Value:request.Id),
                    new SqlParameter("@ProductId", request.ProductId == 0?DBNull.Value:request.ProductId),
                    new SqlParameter("@Search", request.Search),
                    new SqlParameter("@Start", request.Start),
                    new SqlParameter("@Length", request.Length),
                    new SqlParameter("@SortColumnIndex", request.SortColumnIndex),
                    new SqlParameter("@SortDirection", request.SortDirection)
                };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Reviews_Get", oParams);

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

        public async Task<Reviews?> GetReviewsById(int id)
        {
            try
            {
                Reviews? obj = null;

                var oParams = new List<SqlParameter>()
        {
            new SqlParameter("@Id", id)
        };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Reviews_Get", oParams);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    obj = new Reviews()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        ProductId = Convert.ToInt32(dr["ProductId"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        ProductName = dr["ProductName"].ToString(),
                        UserName = dr["UserName"].ToString(),
                        Rating = Convert.ToInt32(dr["Rating"]),
                        ReviewText = dr["ReviewText"].ToString(),
                        IsApproved = Convert.ToBoolean(dr["IsApproved"]),
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveReview(Reviews obj)
        {
            try
            {
                List<SqlParameter> oParams = new();
              
                oParams.Add(new SqlParameter("Id", obj.Id));
                oParams.Add(new SqlParameter("ProductId", obj.ProductId));
                oParams.Add(new SqlParameter("UserId", obj.UserId));
                oParams.Add(new SqlParameter("Rating", obj.Rating));
                oParams.Add(new SqlParameter("ReviewText", obj.ReviewText));
                oParams.Add(new SqlParameter("IsApproved", (obj.IsApproved ?? false) ? 1 : 0));
                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", obj.Id == 0 ? obj.CreatedBy : obj.LastModifiedBy));
                var result = DataContext.ExecuteStoredProcedure("SP_Reviews_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving category", ex);
            }
        }

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteReview(long id)
        {
            try
            {
                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Mode", "DELETE"),
                    new SqlParameter("OperatedBy", Common.LoggedUser_Id())
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


