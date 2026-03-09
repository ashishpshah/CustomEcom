using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace JewelryStore.Infra
{
	public static class DataContext
	{
		public static string _connectionString = AppHttpContextAccessor.AppConfiguration.GetSection("ConnectionStrings").Value;

		public static string Get_DbSchemaName()
		{
			string keyValue = "database=";
			int startIndex = _connectionString.IndexOf(keyValue) + keyValue.Length;
			int endIndex = _connectionString.IndexOf(';', startIndex);
			return _connectionString.Substring(startIndex, endIndex - startIndex);
		}

		public static DataTable ExecuteQuery(string query)
		{
			try
			{
				DataTable dt = new DataTable();

				SqlConnection connection = new SqlConnection(_connectionString);

				SqlDataAdapter oraAdapter = new SqlDataAdapter(query, connection);

				oraAdapter.Fill(dt);

				return dt;
			}
			catch (Exception ex)
			{
				LogService.LogInsert("ExecuteQuery_DataTable - DataContext", "", ex);
				return null;
			}

		}

		public static DataSet ExecuteQuery_DataSet(string sqlquerys)
		{
			DataSet ds = new DataSet();

			try
			{
				DataTable dt = new DataTable();

				SqlConnection connection = new SqlConnection(_connectionString);

				foreach (var sqlquery in sqlquerys.Split(";"))
				{
					dt = new DataTable();

					SqlDataAdapter oraAdapter = new SqlDataAdapter(sqlquery, connection);

					SqlCommandBuilder oraBuilder = new SqlCommandBuilder(oraAdapter);

					oraAdapter.Fill(dt);

					if (dt != null)
						ds.Tables.Add(dt);
				}

			}
			catch (Exception ex)
			{
				LogService.LogInsert("ExecuteQuery_DataSet - DataContext", "", ex);
				return null;
			}

			return ds;
		}

		public static DataTable ExecuteStoredProcedure_DataTable(string query, List<SqlParameter> parameters = null, bool returnParameter = false)
		{
			DataTable dt = new DataTable();

			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						if (parameters != null)
							foreach (SqlParameter param in parameters)
							{
								if (param.Direction == ParameterDirection.Input) param.IsNullable = true;
								cmd.Parameters.Add(param);
							}

						SqlDataAdapter da = new SqlDataAdapter(cmd);

						da.Fill(dt);

						parameters = null;
					}
					conn.Close();
				}
			}
			catch (Exception ex)
			{
				LogService.LogInsert("ExecuteStoredProcedure_DataTable - DataContext", "", ex);
				return null;
			}

			return dt;
		}

		public static DataSet ExecuteStoredProcedure_DataSet(string sp, List<SqlParameter> spCol = null)
		{
			DataSet ds = new DataSet();

			try
			{
				using (SqlConnection con = new SqlConnection(_connectionString))
				{
					con.Open();

					using (SqlCommand cmd = new SqlCommand(sp, con))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						if (spCol != null && spCol.Count > 0)
							cmd.Parameters.AddRange(spCol.ToArray());

						using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
						{
							adp.Fill(ds);
						}
					}

					con.Close();
				}
			}
			catch (Exception ex) { LogService.LogInsert("ExecuteStoredProcedure_DataSet - DataContext", "", ex); }

			return ds;
		}

		public static bool ExecuteNonQuery(string query, List<SqlParameter> parameters = null)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(_connectionString))
				{
					con.Open();

					SqlCommand cmd = con.CreateCommand();

					cmd.CommandType = CommandType.Text;
					cmd.CommandText = query;

					if (parameters != null)
						foreach (SqlParameter param in parameters)
							cmd.Parameters.Add(param);

					cmd.ExecuteNonQuery();
				}

				return true;
			}
			catch (Exception ex)
			{
				LogService.LogInsert("ExecuteNonQuery - DataContext", "", ex);
				return false;
			}
		}

		public static (bool IsSuccess, string Message, long Id, List<string> Extra) ExecuteStoredProcedure(string query, List<SqlParameter> parameters, bool returnParameter = false)
		{
			var response = string.Empty;

			using (SqlConnection con = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = con.CreateCommand())
				{
					try
					{
						con.Open();

						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = query;
						//cmd.DeriveParameters();

						if (parameters != null && parameters.Count > 0)
							cmd.Parameters.AddRange(parameters.ToArray());

						if (returnParameter)
							cmd.Parameters.Add(new SqlParameter("@response", SqlDbType.VarChar, 2000) { Direction = ParameterDirection.Output });

						cmd.CommandTimeout = 86400;
						cmd.ExecuteNonQuery();

						//RETURN VALUE
						//response = cmd.Parameters["P_Response"].Value.ToString();

						response = "S|Success";

						if (cmd.Parameters.Contains("@response"))
						{
							response = cmd.Parameters["@response"].Value.ToString();
						}

						con.Close();
						cmd.Parameters.Clear();
						cmd.Dispose();

					}
					catch (Exception ex)
					{
						con.Close();
						cmd.Parameters.Clear();
						cmd.Dispose();

						response = "E|Opps!... Something went wrong. " + JsonConvert.SerializeObject(ex) + "|0";
					}
				}
			}

			if (!string.IsNullOrEmpty(response) && response.Contains("|"))
			{
				var parts = response.Split('|');

				string msgType = parts.Length > 0 ? parts[0] : "";
				string message = parts.Length > 1 ? parts[1] : "";

				long id = 0;
				if (parts.Length > 2) long.TryParse(parts[2], out id);

				List<string> extra = new List<string>();

				if (parts.Length > 3) for (int i = 3; i < parts.Length; i++) extra.Add(parts[i]);

				return (msgType == "S", message, id, extra);
			}

			return (false, "Invalid response format.", 0, new List<string>());
		}


		public static (bool, string, long, string) ExecuteStoredProcedure_SQLwithpath(string query, List<SqlParameter> parameters, bool returnParameter = false)
		{
			var response = string.Empty;

			using (SqlConnection con = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = con.CreateCommand())
				{
					try
					{
						con.Open();

						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = query;
						//cmd.DeriveParameters();

						if (parameters != null && parameters.Count > 0)
							cmd.Parameters.AddRange(parameters.ToArray());

						if (returnParameter)
							cmd.Parameters.Add(new SqlParameter("@response", SqlDbType.VarChar, 2000) { Direction = ParameterDirection.Output });

						cmd.CommandTimeout = 86400;
						cmd.ExecuteNonQuery();

						//RETURN VALUE
						//response = cmd.Parameters["P_Response"].Value.ToString();

						response = "S|Success";

						if (cmd.Parameters.Contains("@response"))
						{
							response = cmd.Parameters["@response"].Value.ToString();
						}

						con.Close();
						cmd.Parameters.Clear();
						cmd.Dispose();

					}
					catch (Exception ex)
					{
						con.Close();
						cmd.Parameters.Clear();
						cmd.Dispose();

						response = "E|Opps!... Something went wrong. " + JsonConvert.SerializeObject(ex) + "|0";
					}
				}
			}

			if (!string.IsNullOrEmpty(response) && response.Contains("|"))
			{
				var msgtype = response.Split('|').Length > 0 ? Convert.ToString(response.Split('|')[0]) : "";
				var message = response.Split('|').Length > 1 ? Convert.ToString(response.Split('|')[1]).Replace("\"", "") : "";

				Int64 strid = 0;
				if (Int64.TryParse(response.Split('|').Length > 2 ? Convert.ToString(response.Split('|')[2]).Replace("\"", "") : "0", out strid)) { }
				string paths = response.Split('|').Length > 3 ? response.Split('|')[3].Replace("\"", "") : "0";


				return (msgtype.Contains("S"), message, strid, paths);
			}
			else
				return (false, ResponseStatusMessage.Error, 0, "0");
		}

		public static string ExecuteStoredProcedure(string sp, SqlParameter[] spCol)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					using (SqlCommand cmd = new SqlCommand(sp, conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						SqlParameter returnParameter = new SqlParameter("@response", SqlDbType.NVarChar, 2000);

						returnParameter.Direction = ParameterDirection.Output;

						if (spCol != null && spCol.Length > 0)
							cmd.Parameters.AddRange(spCol);


						cmd.Parameters.Add(returnParameter);

						conn.Open();
						cmd.ExecuteNonQuery();
						conn.Close();

						return returnParameter.Value.ToString();
					}
				}

			}
			catch (SqlException ex)
			{
				StringBuilder errorMessages = new StringBuilder();
				for (int i = 0; i < ex.Errors.Count; i++)
				{
					errorMessages.Append("Index #......" + i.ToString() + Environment.NewLine +
										 "Message:....." + ex.Errors[i].Message + Environment.NewLine +
										 "LineNumber:.." + ex.Errors[i].LineNumber + Environment.NewLine);
				}
				//Activity_Log.SendToDB("Database Oparation", "Error: " + "StoredProcedure: " + sp, ex);
				return "E|" + errorMessages.ToString();
			}
			catch (Exception ex)
			{
				//Activity_Log.SendToDB("Database Oparation", "Error: " + "StoredProcedure: " + sp, ex);
				return "E|" + ex.Message.ToString();
			}
		}

		public static bool ExecuteNonQuery_Delete(string query, List<SqlParameter> parameters = null)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(_connectionString))
				{
					con.Open();

					SqlCommand cmd = con.CreateCommand();
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = query;

					if (parameters != null)
						foreach (SqlParameter param in parameters)
							cmd.Parameters.Add(param);

					cmd.ExecuteNonQuery();
				}

				return true;
			}
			catch (Exception ex)
			{
				LogService.LogInsert("ExecuteNonQuery_Delete - DataContext", "", ex);
				return false;
			}
		}

	}

}
