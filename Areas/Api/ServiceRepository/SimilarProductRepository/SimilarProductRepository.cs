using JewelryStore.Areas.Admin.Models;
using JewelryStore.Areas.Api.ServiceRepository.SimilarProductRepository;
using JewelryStore.Infra;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JewelryStore.Areas.Api.ServiceRepository.SimilarProductRepository
{
    public class SimilarProductRepository : ISimilarProductRepository
    {
        public async Task<object> GetSimilarProduct(int ProductId = 0, int TopCount = 0)
        {
            try
            {
                var oParams = new List<SqlParameter>()
                {
                    new SqlParameter("@ProductId",ProductId),
                    new SqlParameter("@TopCount", TopCount),
                 
                };

                DataSet ds = DataContext.ExecuteStoredProcedure_DataSet("SP_SimilarProduct_Get", oParams);

                var table1 = new List<Dictionary<string, object>>();
                

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

               

                var result = new
                {
                    table1 = table1
                };

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
    }
}


