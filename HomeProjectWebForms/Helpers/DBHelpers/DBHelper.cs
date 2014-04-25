using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HomeProjectWebForms.Helpers
{
    /// <summary>
    ///     Класс помощник для работы с БД
    /// </summary>
    public static class DBHelper
    {
        //public static string MainDirectoryName = "/HomeProjectWebForms";
        /// <summary>
        ///     Основная дирректория проекта
        /// </summary>
        /// <summary>
        ///     Имя основной строки соединения с БД
        /// </summary>
        public static string MainConnectionStringName = "CompanyConnectionString";

        //public static Configuration WebConfig;
        /// <summary>
        ///     Ссылка на конфигурацию сайта
        /// </summary>
        /// <summary>
        ///     Основная строка для соединения с БД
        /// </summary>
        public static string MainConnetcionString;

        /// <summary>
        ///     Коснтруктор по умолчанию
        /// </summary>
        static DBHelper()
        {
            //WebConfig =System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(MainDirectoryName);

            //MainConnetcionString = WebConfig.ConnectionStrings.ConnectionStrings["MainConnectionStringName"].ConnectionString;

            MainConnetcionString = ConfigurationManager.ConnectionStrings[MainConnectionStringName].ConnectionString;
        }

        /// <summary>
        ///     Получения соединия с БД
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(MainConnetcionString);
        }

        /// <summary>
        ///     Получение соединения с БД
        /// </summary>
        /// <param name="connectionString">Строка для соединиения</param>
        /// <returns></returns>
        public static SqlConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public static string GetUserFullName(string login, out Guid userGuid)
        {
            if (!string.IsNullOrEmpty(login))
            {
                string commandText = "select Id,Name from Employee where AccountName='" + login + "'";
                string userName = string.Empty;
                userGuid = Guid.Empty;
                SqlConnection connection = GetConnection();
                var cmd = new SqlCommand(commandText, connection);
                SqlDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                        if (reader.Read())
                        {
                            userGuid = reader.GetGuid(0);
                            userName = reader.GetString(1);
                        }
                }
                catch (Exception ex)
                {
                    ShellLogger.WriteLog("DB.log", "Ошибка GetUserFullNameв", ex);
                    //Response.Write("<script>window.alert('Ошибка приложения');</script>");
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    connection.Close();
                }

                return userName;
            }
            userGuid = Guid.Empty;
            return "Пустой Логин";
        }

        public static Guid GetUserDepartment(Guid userGuid)
        {
            Guid userDepartment = Guid.Empty;
            SqlConnection connection = GetConnection();
            var cmd = new SqlCommand("[GetEmployeeDepartment]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = @userGuid;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    if (reader.Read())
                    {
                        userDepartment = reader.GetGuid(0);
                    }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetEmployeeDepartment", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }

            return userDepartment;
        }

        public static bool CheckIfUserIsAdministrator(Guid userGuid)
        {
            bool isAdmin = false;
            SqlConnection connection = GetConnection();
            var cmd = new SqlCommand("[GetIfEmployeeIsAdmin]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = @userGuid;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    if (reader.Read())
                    {
                        isAdmin = reader.GetBoolean(0);
                    }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetEmployeeDivision", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }
            return isAdmin;
        }

        public static Guid GetUserDivision(Guid userGuid, out string divisionName)
        {
            Guid userDivision = Guid.Empty;
            divisionName = string.Empty;
            SqlConnection connection = GetConnection();
            var cmd = new SqlCommand("[GetEmployeeDivision]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = @userGuid;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    if (reader.Read())
                    {
                        userDivision = reader.GetGuid(0);
                        divisionName = reader.GetString(1);
                    }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetEmployeeDivision", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }

            return userDivision;
        }

        public static DataSet GetDepartmentEmployees(Guid userGuid, Guid department)
        {
            var myAdapter = new SqlDataAdapter();
            SqlConnection connection = GetConnection();
            var cmd = new SqlCommand("[GetDepartmentStateEmployees]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = @userGuid;
            cmd.Parameters.Add("@DepartmentStateGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@DepartmentStateGuid"].Value = @department;
            myAdapter.SelectCommand = cmd;
            var myDataSet = new DataSet();
            try
            {
                connection.Open();

                myAdapter.Fill(myDataSet);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetDepartmentEmployees", ex);
                //throw (ex);
            }
            finally
            {
                connection.Close();
            }
            return null;
        }

        public static DataSet GetFilteredEmployees(Guid userGuid, string filteredText)
        {
            var myAdapter = new SqlDataAdapter();
            SqlConnection connection = GetConnection();
            var cmd = new SqlCommand("[GetFilteredDepartmentStateEmployees]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = @userGuid;
            cmd.Parameters.Add("@EmployeeFilterText", SqlDbType.NVarChar);
            cmd.Parameters["@EmployeeFilterText"].Value = @filteredText;
            myAdapter.SelectCommand = cmd;
            var myDataSet = new DataSet();
            try
            {
                connection.Open();

                myAdapter.Fill(myDataSet);
                return myDataSet;
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetFilteredEmployees", ex);
                //throw (ex);
            }
            finally
            {
                connection.Close();
            }
            return null;
        }

        public static string GetUserPhoneNumbers(Guid userGuid)
        {
            string userPhoneNumbers = string.Empty;
            SqlConnection connection = GetConnection();
            var cmd = new SqlCommand("[GetUserPhoneNumbers]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = @userGuid;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    if (i > 0)
                        userPhoneNumbers += "; ";

                    string cityCode = "(Пусто)";
                    if (!reader.IsDBNull(9))
                        cityCode = "(" + reader.GetString(9) + ")";

                    string phoneType = string.Empty;
                    if (!reader.IsDBNull(6))
                        phoneType=reader.GetString(6);

                    string parsedPhoneNumber = GUIHelper.ParsePhone(reader.GetString(0),phoneType);

                    if (phoneType.Trim().ToUpper()=="ГАТС")
                        userPhoneNumbers += cityCode+parsedPhoneNumber;
                    else
                        userPhoneNumbers +=parsedPhoneNumber;
                    i++;
                }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetUserPhoneNumbers", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }

            return userPhoneNumbers;
        }

        public static string GetFullUserPhoneNumbers(Guid userGuid)
        {
            string userPhoneNumbers = string.Empty;
            SqlConnection connection = GetConnection();
            var cmd = new SqlCommand("[GetUserPhoneNumbers]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = @userGuid;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    if (i > 0)
                        userPhoneNumbers += "; ";
                    string cityCode = "(Пусто)";
                    if (!reader.IsDBNull(9))
                        cityCode = "(" + reader.GetString(9) + ")";

                    string phoneType = string.Empty;
                    if (!reader.IsDBNull(6))
                        phoneType = reader.GetString(6);

                    string location = string.Empty;
                    if (!reader.IsDBNull(1))
                        location=reader.GetString(1);

                    string parsedPhoneNumber = GUIHelper.ParsePhone(reader.GetString(0), phoneType);

                    if (phoneType.Trim().ToUpper() == "ГАТС")
                        userPhoneNumbers += location + ": " + cityCode + parsedPhoneNumber + Environment.NewLine;
                    else
                       userPhoneNumbers += location + ": " +parsedPhoneNumber + Environment.NewLine;
                    i++;
                }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetFullUserPhoneNumbers", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }

            return userPhoneNumbers;
        }

        public static string GetUserSpecificPhoneNumbers(Guid userGuid)
        {
            string userPhoneNumbers = string.Empty;
            SqlConnection connection = GetConnection();
            var cmd = new SqlCommand("[GetUserSpecificPhoneNumbers]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = @userGuid;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    if (i > 0)
                        userPhoneNumbers += "; ";
                    string cityCode = "(Пусто)";
                    if (!reader.IsDBNull(3))
                        cityCode = "(" + reader.GetString(3) + ")";

                    string phoneType = string.Empty;
                    if (!reader.IsDBNull(6))
                        phoneType = reader.GetString(6);

                    string parsedPhoneNumber = GUIHelper.ParsePhone(reader.GetString(0), phoneType);
                    if (phoneType.Trim().ToUpper() == "ГАТС")
                        userPhoneNumbers += cityCode + parsedPhoneNumber;
                    else
                        userPhoneNumbers += parsedPhoneNumber;
                    i++;
                }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка [GetUserSpecificPhoneNumbers]", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }

            return userPhoneNumbers;
        }
    }
}