using JewelryStore.Areas.Admin.Models;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.InquiryRepository
{
    public class InquiryRepository : IInquiryRepository
    {
        public async Task<object> GetAllInquires(PagingRequest request)
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

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_Inquiry_Get", oParams);

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
                            if (col.ColumnName == "Inquiry_Date" && row[col] != DBNull.Value)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Inquiries?> GetInquiryById(int id)
        {
            try
            {
                Inquiries? obj = null;

                var oParams = new List<SqlParameter>()
        {
            new SqlParameter("@Id", id)
        };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Inquiry_Get", oParams);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    obj = new Inquiries()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        ProductId = Convert.ToInt32(dr["ProductId"]),
                        ProductName = dr["ProductName"]?.ToString(),
                        Inquiry_Date = Convert.ToDateTime(dr["Inquiry_Date"]),
                        Subject = dr["Subject"]?.ToString(),
                        Message = dr["Message"]?.ToString(),
                        Status = dr["Status"]?.ToString(),
                        Status_Desc = dr["Status_Desc"]?.ToString(),
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


        public async Task<List<Inquiries?>> GetInquiryStatusHistory(int id)
        {
            try
            {
                List<Inquiries?> obj = new List<Inquiries?>();   // FIX


                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Flag", "S")
                };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Inquiry_History_Get", oParams);

                if (dt.Rows.Count > 0)
                {
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        obj.Add(new Inquiries()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            InquiryId = Convert.ToInt32(row["InquiryId"]),
                            OldStatus = row["OldStatus"]?.ToString(),
                            NewStatus = row["NewStatus"]?.ToString(),
                            OldStatus_Desc = row["OldStatus_Desc"]?.ToString(),
                            NewStatus_Desc = row["NewStatus_Desc"]?.ToString(),
                            Remarks = row["Remarks"]?.ToString(),
                            IsActive = Convert.ToBoolean(row["IsActive"]),
                            CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                            CreatedDate_Text = Convert.ToDateTime(row["CreatedDate"]).ToString("dd-MMM-yyyy")
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

        public async Task<List<Inquiries?>> GetInquiryRepliedHistory(int id)
        {
            try
            {
                List<Inquiries?> obj = new List<Inquiries?>();   // FIX

                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Flag", "R")
                };

                DataTable dt = DataContext.ExecuteStoredProcedure_DataTable("SP_Inquiry_History_Get", oParams);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        obj.Add(new Inquiries()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            InquiryId = Convert.ToInt32(dr["InquiryId"]),
                            ReplyBy = dr["ReplyBy"]?.ToString(),
                            ReplyBy_Text = dr["ReplyBy_Text"]?.ToString(),
                            Remarks = dr["ReplyMessage"]?.ToString(),
                            IsActive = Convert.ToBoolean(dr["IsActive"]),
                            ReplyDate = Convert.ToDateTime(dr["ReplyDate"]),
                            ReplyDate_Text = Convert.ToDateTime(dr["ReplyDate"]).ToString("dd-MMM-yyyy")
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

        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> SaveInquiry(Inquiries obj)
        {
            try
            {
                List<SqlParameter> oParams = new List<SqlParameter>();
                var UserId = Common.LoggedUser_Id();
                oParams.Add(new SqlParameter("Id", obj.Id));
                oParams.Add(new SqlParameter("ProductId", obj.ProductId));
                oParams.Add(new SqlParameter("Subject", obj.Subject));
                oParams.Add(new SqlParameter("Message", obj.Message));
                oParams.Add(new SqlParameter("Inquiry_Date", obj.Inquiry_Date));
                oParams.Add(new SqlParameter("IsActive", obj.IsActive ? 1 : 0));
                oParams.Add(new SqlParameter("Mode", "SAVE"));
                oParams.Add(new SqlParameter("OperatedBy", Common.LoggedUser_Id()));

                var result = DataContext.ExecuteStoredProcedure("SP_Inquiry_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving category", ex);
            }
        }
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> ChangeStatus(int InquiryId, string Status = "", string Remarks = "")
        {
            try
            {
                List<SqlParameter> oParams = new List<SqlParameter>();

                oParams.Add(new SqlParameter("Id", InquiryId));
                oParams.Add(new SqlParameter("@Status", Status));
                oParams.Add(new SqlParameter("Remarks", Remarks));
                oParams.Add(new SqlParameter("OperatedBy", Common.LoggedUser_Id()));
                var result = DataContext.ExecuteStoredProcedure("SP_Inquiry_Status_Change", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving category", ex);
            }
        }
        public async Task<(bool IsSuccess, string Message, long Id, List<string> Extra)> DeleteInquiries(long id)
        {
            try
            {
                List<SqlParameter> oParams = new()
                {
                    new SqlParameter("Id", id),
                    new SqlParameter("Mode", "DELETE"),
                    new SqlParameter("OperatedBy", Common.LoggedUser_Id())
                };

                var result = DataContext.ExecuteStoredProcedure("SP_Inquiry_Save", oParams, true);

                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


