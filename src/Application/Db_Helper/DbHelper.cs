using Domain.OtherModels.DataService;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Db_Helper
{
    public class DbHelper
    {
        private readonly IDbConnection _dbConnection;
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbConnection = new SqlConnection(_connectionString);
        }

        public SqlConnection Con
        {
            get
            {
                return (SqlConnection)_dbConnection;
            }
        }

        public async Task<int> GetLastValueAsync(string fieldName, string tableName, string condition, SqlTransaction trans)
        {
            string lcsql = $@"SELECT Isnull(MAX({fieldName}),0) AS TrNo
             FROM {tableName} WHERE 1 = 1  {condition}";

            var aCommand = new SqlCommand(lcsql, trans.Connection, trans);

            try
            {
                using (SqlDataReader aReader = await aCommand.ExecuteReaderAsync())
                {
                    var result = 0;

                    while (await aReader.ReadAsync())
                    {
                        result = (int)aReader["TrNo"];
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching product code.", ex);
            }
        }

        public async Task<string> GetProductCodeAsync(string fieldName, string tableName, SqlTransaction trans)
        {
            string lcsql = $@"SELECT RIGHT('0000' + Convert(varchar, ISNULL(Max(CASE 
         WHEN RIGHT({fieldName}, 4) LIKE '%[^0-9]%' THEN 0 ELSE Convert(integer, RIGHT({fieldName}, 4)) END), 0) + 1), 4) AS TrNo 
         FROM {tableName}";

            var aCommand = new SqlCommand(lcsql, trans.Connection, trans);

            try
            {
                using (SqlDataReader aReader = await aCommand.ExecuteReaderAsync())
                {
                    string result = string.Empty;

                    while (await aReader.ReadAsync())
                    {
                        result = aReader["TrNo"].ToString();
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching product code.", ex);
            }
        }
        public async Task<string> GetProductTenDigitCodeAsync(string fieldName, string tableName, SqlTransaction trans)
        {
            string lcsql = $@"SELECT RIGHT('0000000000' + Convert(varchar, ISNULL(Max(CASE 
            WHEN RIGHT({fieldName}, 4) LIKE '%[^0-9]%' THEN 0 ELSE Convert(integer, RIGHT({fieldName}, 4)) END), 0) + 1), 10) AS TrNo 
            FROM {tableName}";

            var aCommand = new SqlCommand(lcsql, trans.Connection, trans);

            try
            {
                using (SqlDataReader aReader = await aCommand.ExecuteReaderAsync())
                {
                    string result = string.Empty;

                    while (await aReader.ReadAsync())
                    {
                        result = aReader["TrNo"].ToString();
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching product code.", ex);
            }
        }
        public bool FncSeekRecordNew(string lcTableName, string lcCondition, SqlTransaction trans)
        {
            string query = "";
            if (lcCondition != "")
            {
                query = $@"Select * from " + lcTableName + " where 1=1 AND " + lcCondition + "";
            }
            else
            {
                query = $@"Select * from " + lcTableName + "";
            }

            var aCommand = new SqlCommand(query, trans.Connection, trans);

            try
            {
                bool lnTrueFlase = false;
                using (SqlDataReader aReader = aCommand.ExecuteReader())
                {
                    while (aReader.Read())
                    {
                        lnTrueFlase = aReader.HasRows;
                    }

                    return lnTrueFlase;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching product code.", ex);
            }
        }
        public List<IdNameForDropdown> GetIdNameForDropdownBox(string query)
        {
            var lists = new List<IdNameForDropdown>();
            try
            {
                var cmd = new SqlCommand(query, Con);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lists.Add(new IdNameForDropdown()
                    {
                        Id = Convert.ToInt32(rdr["Id"]),
                        Name = rdr["Name"].ToString(),
                    });
                }
                return lists;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<IdNameForDropdown> GetCodeDescriptionForDropdown(string query)
        {
            var lists = new List<IdNameForDropdown>();
            try
            {
                var cmd = new SqlCommand(query, Con);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lists.Add(new IdNameForDropdown()
                    {
                        Code = rdr["Code"].ToString(),
                        Description = rdr["Description"].ToString(),
                    });
                }
                return lists;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DeleteInsert(string id)
        {
            try
            {
                Con.Open();
                string autoId = id;
                var command = new SqlCommand(autoId, Con);
                return command.ExecuteNonQuery().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }
        public string DeleteInsertWithTransaction(string id, SqlTransaction trans)
        {
            try
            {
                // Con.Open();
                string autoId = id;
                var command = new SqlCommand(autoId, Con, trans);
                return command.ExecuteNonQuery().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    //  Con.Close();
                }
            }
        }

        public int GetPnoByUserNameCloseCon(string userName)
        {
            try
            {
                return Convert.ToInt32(ReturnFieldValue("tbl_PASSWORD_MASTER", "UserName='" + userName + "'", "Pno"));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public string ReturnFieldValue(string lcTableName, string lcCondition, string lcFieldName)
        {
            string query = "", result = "";
            if (lcCondition != "")
            {
                query = "Select " + lcFieldName + " as Description from " + lcTableName + " where " + lcCondition + "";
            }
            else
            {
                query = "Select " + lcFieldName + " as Description from " + lcTableName + "";
            }
            Con.Open();
            var aCommand = new SqlCommand(query, Con);
            SqlDataReader aReader = aCommand.ExecuteReader();

            while (aReader.Read())
            {
                result = aReader["Description"].ToString();
            }
            aReader.Close();
            Con.Close();
            return result;
        }

        public string ReturnFieldValue(string tableName, string fieldName, string? condition, IDbConnection connection, SqlTransaction trans)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException(nameof(tableName));
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException(nameof(fieldName));
            string query = $"SELECT {fieldName} FROM {tableName}";
            if (!string.IsNullOrWhiteSpace(condition))
                query += $" WHERE {condition}";

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Transaction = trans;

            object? result = command.ExecuteScalar();
            return result?.ToString() ?? string.Empty;
        }


        public DataTable ConvertListDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public int GetMaxId(string tableName, string fieldName)
        {
            int slNo = 0;
            string sql = @"SELECT  Isnull(MAX(" + fieldName + "),0)+1 AS TrNo  FROM " + tableName + " ";
            //WHERE Pno='" + CurrentPno + "'
            Con.Open();
            var aCommand = new SqlCommand(sql, Con);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                slNo = Convert.ToInt32(aReader["TrNo"].ToString());
            }
            Con.Close();
            return slNo;
        }
        public int GetMaxId(string tableName, string fieldName, SqlTransaction transaction)
        {
            int slNo = 0;
            string sql = @"SELECT  Isnull(MAX(" + fieldName + "),0)+1 AS TrNo  FROM " + tableName + " ";
            //WHERE CostCenterName='" + CurrentPno + "'
            //Con.Open();
            var aCommand = new SqlCommand(sql, Con, transaction);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                slNo = Convert.ToInt32(aReader["TrNo"].ToString());
            }
            //Con.Close();
            aReader.Close();
            return slNo;
        }

        public string GetMaxCode(string fieldName, string tableName, string condition, SqlTransaction trans)
        {
            string lcsql = @"SELECT RIGHT('0000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + fieldName + ", 4))),0)+ 1), 4) AS SlNo FROM " + tableName + " where " + condition + " ";
            // Con.Open();
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["SlNo"].ToString();
            }

            // Con.Close();
            aReader.Close();
            return lcsql;
        }

        public string GetMaxCode(string fieldName, string tableName, string condition)
        {
            string lcsql = @"SELECT RIGHT('0000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + fieldName + ", 4))),0)+ 1), 4) AS SlNo FROM " + tableName + " where " + condition + " ";
            Con.Open();
            var aCommand = new SqlCommand(lcsql, Con);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["SlNo"].ToString();
            }

            Con.Close();
            aReader.Close();
            return lcsql;
        }

        public string GetMaxCode5(string fieldName, string tableName, SqlTransaction trans)
        {
            string lcsql = @"SELECT RIGHT('00000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + fieldName + ", 5))),0)+ 1), 5) AS SlNo FROM " + tableName + " ";
            //Con.Open();
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["SlNo"].ToString();
            }

            //Con.Close();
            aReader.Close();
            return lcsql;
        }

        public string GetInvoiceNo(int stockType, int pno, SqlTransaction trans)
        {
            string lcsql = @"Exec SP_GET_INVOICENO " + stockType + "," + pno + " ";
            // Con.Open();
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["InvNo"].ToString();
            }
            // Con.Close();
            aReader.Close();
            return lcsql;
        }
        public string GetInvoiceNo(int stockType, int pno)
        {
            string lcsql = @"Exec SP_GET_INVOICENO " + stockType + "," + pno + "";
            Con.Open();
            var aCommand = new SqlCommand(lcsql, Con);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["InvNo"].ToString();
            }
            Con.Close();
            aReader.Close();
            return lcsql;
        }

        public string GetMonthlyTrNo(string fieldName, string tableName, string startWith, SqlTransaction trans)
        {
            string lcsql = @"SELECT ISNULL( (SELECT TOP 1 " + fieldName + " FROM " + tableName + " ORDER by id DESC), '0000000001') as id";
            //Con.Open();
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["id"].ToString();
            }
            string lid = lcsql.Substring(lcsql.Length - 10);
            string nid = lid.Substring(6, 4);

            string mn = lid.Substring(2, 2);
            var month = DateTime.Today.ToString("MM");
            var currdate = DateTime.Today.ToString("yyMMdd");
            var strpd = ""; long RN = 0;
            if (mn != month)
            {
                var i = 1;
                strpd = currdate.PadRight(10, '0');
                RN = Convert.ToInt64(strpd) + i;
            }
            else
            {
                strpd = currdate.PadRight(10, '0');
                RN = Convert.ToInt64(strpd) + Convert.ToInt64(nid) + 1;
            }
            var refno = startWith + RN;
            //Con.Close();
            aReader.Close();
            return refno;
        }

        public string GetMonthlyTrNo(string fieldName, string tableName, string startWith, string cond, SqlTransaction trans)
        {
            string lcsql = @"SELECT  RIGHT('00' + Convert(varchar,YEAR(GETDATE())), 2) + RIGHT('00' + Convert(varchar,MONTH(Getdate())), 2) + RIGHT('0000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + fieldName + ", 4))),0)+ 1), 5) AS RefNo FROM " + tableName + " Where " + cond + " ";
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["RefNo"].ToString();
            }
            var refno = startWith + lcsql;
            aReader.Close();
            return refno;
        }

        public string GetRefNo(string fieldName, string tableName, string startWith, string cond, SqlTransaction trans)
        {
            string lcsql = @"SELECT  RIGHT('00' + Convert(varchar,YEAR(GETDATE())), 2) + RIGHT('00' + Convert(varchar,MONTH(Getdate())), 2) + RIGHT('0000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + fieldName + ", 4))),0)+ 1), 5) AS RefNo FROM " + tableName + " Where " + cond + " ";
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["RefNo"].ToString();
            }
            var refno = startWith + lcsql;
            aReader.Close();
            return refno;
        }

        public string GetAppointmentNo(string fieldName, string tableName, string condition, SqlTransaction trans)
        {
            string lcsql = @"SELECT RIGHT('000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + fieldName + ", 3))),3)+ 1), 3) AS SlNo FROM " + tableName + " where " + condition + " ";
            // Con.Open();
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["SlNo"].ToString();
            }
            // Con.Close();
            aReader.Close();
            return lcsql;
        }

        public DateTime GetBdTime()
        {
            var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.Now, timezoneInfo);
        }

        public string ReturnLastInv(string lcTableName)
        {
            string query = "", result = "";
            query = "SELECT TOP 1 InvoiceNo  as Description FROM " + lcTableName + " ORDER BY Id DESC ";
            Con.Open();
            var aCommand = new SqlCommand(query, Con);
            SqlDataReader aReader = aCommand.ExecuteReader();

            while (aReader.Read())
            {
                result = aReader["Description"].ToString();
            }
            aReader.Close();
            Con.Close();
            return result;
        }



        public bool FncSeekRecordNewOpenCon(string lcTableName, string lcCondition)
        {
            string query = "";
            if (lcCondition != "")
            {
                query = "Select * from " + lcTableName + " where " + lcCondition + "";
            }
            else
            {
                query = "Select * from " + lcTableName + "";
            }

            var cmd = new SqlCommand(query, Con);
            var aReader = cmd.ExecuteReader();
            bool lnTrueFlase = aReader.HasRows;
            aReader.Close();
            //   Con.Close();
            return lnTrueFlase;
        }

        public string DeleteTableOpenCon(string lcTableName, string lcCondition, SqlTransaction trans, SqlConnection Con)
        {
            string query = "";
            if (lcCondition != "")
            {
                query = "DELETE  FROM " + lcTableName + " WHERE " + lcCondition + "";
            }
            try
            {
                var cmd = new SqlCommand(query, Con, trans);
                return cmd.ExecuteNonQuery().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string getCompanyId(string Name)
        {
            string query = ""; int result = 0;
            string cName = Name;
            string fName = cName.Substring(0, 1).ToUpper();
            query = "Select COUNT(Name) as Name from PharCompanyInfo where Name like '" + fName + "%' ";
            string padleft = fName.PadRight(3, '0');
            Con.Open();
            //string Company_Id = padleft + query;
            var cmd = new SqlCommand(query, Con);
            var aReader = cmd.ExecuteReader();
            while (aReader.Read())
            {
                result = int.Parse(aReader["Name"].ToString());
            }
            string Company_Id = padleft + (result + 1);
            aReader.Close();
            Con.Close();
            return Company_Id;
        }

        public string getProductId(string Name)
        {
            string query = ""; int result = 0;
            string cName = Name;
            string fName = cName.Substring(0, 1).ToUpper();
            query = "Select COUNT(ProductName) as ProductName from PharProductInfo where ProductName like '%" + fName + "%' ";
            string padleft = fName.PadRight(3, '0');
            Con.Open();
            //string Company_Id = padleft + query;
            var cmd = new SqlCommand(query, Con);
            var aReader = cmd.ExecuteReader();
            while (aReader.Read())
            {
                result = int.Parse(aReader["ProductName"].ToString());
            }
            string Company_Id = padleft + (result + 1);
            aReader.Close();
            Con.Close();
            return Company_Id;
        }

        public string GetDailySlNo(string fieldName, string tableName, string cond, SqlTransaction trans)
        {
            string lcsql = @"SELECT  RIGHT('000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + fieldName + ", 3))),0)+ 1), 3) AS Code FROM " + tableName + "  WHERE valid = 1 " + cond + " ";
            var aCommand = new SqlCommand(lcsql, Con, trans);
            Con.Open();
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["Code"].ToString();
            }
            Con.Close();
            aReader.Close();
            return lcsql;
        }

        public string GetMonthlyRefNo(string fieldName, string tableName, string startWith, string RefDate, SqlTransaction trans)
        {
            string lcsql = @"SELECT  RIGHT('00' + Convert(varchar,YEAR(GETDATE())), 2) + RIGHT('00' + Convert(varchar,MONTH(Getdate())), 2) + RIGHT('0000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + fieldName + ", 4))),0)+ 1), 5) AS RefNo FROM " + tableName + " WHERE MONTH(" + RefDate + ")=MONTH(GETDATE())";
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["RefNo"].ToString();
            }
            var refno = startWith + lcsql;
            aReader.Close();
            return refno;
        }

        public bool CheckRecord(string lcTableName, string condition)
        {
            string query = "";
            if (lcTableName != "")
            {
                query = "Select * from " + lcTableName + " where " + condition + " ";
            }
            else
            {
                query = "Select * from " + lcTableName + "";
            }
            Con.Open();
            var cmd = new SqlCommand(query, Con);
            var aReader = cmd.ExecuteReader();
            bool lnTrueFlase = aReader.HasRows;
            Con.Close();
            return lnTrueFlase;
        }

        public string Ip()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            var ip = addr[1].ToString();
            return ip;
        }

        public string UpdateTableOpenCon(string lcTableName, string lcCondition, SqlTransaction trans, SqlConnection Con)
        {
            string query = "";
            if (lcCondition != "")
            {
                query = "UPDATE " + lcTableName + " SET Valid=0 WHERE " + lcCondition + "";
            }
            try
            {
                var cmd = new SqlCommand(query, Con, trans);
                return cmd.ExecuteNonQuery().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // End of Monjur Bhai


        public string GetAutoIncrementNumberFromStoreProcedure(int tableId, SqlTransaction trans)
        {
            string lcsql = @"Exec SP_GET_AUTO_GENERATED_NUMBER " + tableId + "";
            //Con.Open();
            var aCommand = new SqlCommand(lcsql, Con, trans);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["InvNo"].ToString();
            }
            //Con.Close();
            aReader.Close();
            return lcsql;
        }
        public string GetAutoIncrementNumberFromStoreProcedure(int tableId)
        {
            string lcsql = @"Exec SP_GET_AUTO_GENERATED_NUMBER " + tableId + "";
            Con.Open();
            var aCommand = new SqlCommand(lcsql, Con);
            SqlDataReader aReader = aCommand.ExecuteReader();
            while (aReader.Read())
            {
                lcsql = aReader["InvNo"].ToString();
            }
            Con.Close();
            aReader.Close();
            return lcsql;
        }

        public async Task<string> GenerateRandomTicketNo(string fieldName, string tableName, SqlTransaction trans)
        {
            string lcsql = $@"SELECT RIGHT('00' + Convert(varchar, DAY(Getdate())), 2) + RIGHT('00' + Convert(varchar,MONTH(Getdate())), 2) 
             + RIGHT('00' + Convert(varchar,YEAR(GETDATE())), 2)
             + RIGHT('0000'+ Convert(varchar,ISNULL(Max(Convert(integer, RIGHT({fieldName}, 4))),0)+ 1), 4)
            AS TicketNo 
            FROM {tableName}";
            var aCommand = new SqlCommand(lcsql, trans.Connection, trans);

            try
            {
                using (SqlDataReader aReader = await aCommand.ExecuteReaderAsync())
                {
                    string result = string.Empty;

                    while (await aReader.ReadAsync())
                    {
                        result = aReader["TicketNo"].ToString();
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching generate random ticket number.", ex);
            }
        }

    }

}
