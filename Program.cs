using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Diagnostics;


namespace ADOdotNET_First_Project
{
    internal class Program
    {

        //static string connStr = @"Data Source=DESKTOP-ET8RJ3S;Initial Catalog=Library;Integrated Security=True;Connect Timeout=30";
        //static SqlConnection conn = new SqlConnection(connStr);
        static DbConnection conn = null;
        static DbProviderFactory fact = null;

        private enum Tasks
        {
            ALL = 1,//вся инфоромация
            ALL_FULL_NAMES,//все имена
            ALL_AVG_GRADES,//все средние оценки
            MIN_GRADES_MORE_THAN_TARGET,// все минимальные оценки больше чем заданная
            ALL_UNIQUE_MIN_GRADES = 5,//минимамльная оценка по каждому предмету
            MIN_AVG_GRADE,//минимальная средняя оценка
            MAX_AVG_GRADE,//максимальная средняя оценка
            STUDS_WITH_MIN_MATH,//студенты с минимальной оценкой по математике
            STUDS_WITH_MAX_MATH = 9,//студенты с максимальной оценкой по математике
            COUNT_IN_GROUPS,//количество студентов в каждой группе
            AVG_GRADE_IN_GROUP,//средняя оценка в каждой группе
            EXT//выход

        }

        public Program()
        {
            //conn = new SqlConnection(); 
            //conn = new SqlConnection(connStr);
             //conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
        }
        private static string GetConnectionStringVByProvider(string providerName)
        {
            ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;
            string returnValue = "";
            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    //if (cs.ProviderName == providerName)
                    //{
                    //    returnValue = cs.ConnectionString;
                    //    break;
                    //}
                    //Костыль, проект не видит содержимое App.config
                    if (cs.ProviderName == "System.Data.SqlClient")
                    {
                        returnValue = "Data Source=DESKTOP-ET8RJ3S;Initial Catalog=Library;Integrated Security=True;Connect Timeout=3";
                            break;
                    }

                }
            }
            return returnValue;
        }

        public static async void Insert()
        {
            try
            {
                conn.Open();

                string insertStr = "INSERT INTO Authors (FirstName, LastName) VALUES ('Roger', 'Federer')";
                SqlCommand insertCmd = new SqlCommand(insertStr, conn as SqlConnection);

                await insertCmd.ExecuteNonQueryAsync();

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }
        public static async void SelectAuthors()
        {
            string sqlSelect = "SELECT * FROM Authors";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlSelect, conn as SqlConnection);

                var reader = cmd.ExecuteReader();
                int lineCount = 0;
                while (await reader.ReadAsync())
                {
                    if (lineCount ==0)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader.GetName(i) + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine($"Line {lineCount}: {reader["FirstName"]} {reader["LastName"]}");
                    lineCount++;
                }
                


            }
            catch (Exception ex)
            {            
                Console.WriteLine(ex.Message); 
            }
            finally
            {
                conn.Close();
            }
        }


        public async static void selectId()
        {
            using (conn = new SqlConnection(conn.ConnectionString))
            {
                string selectId = "Select * from Authors where id = @p1";
                SqlCommand cms = new SqlCommand();
                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = "@p1";
                param1.SqlDbType = System.Data.SqlDbType.Int;
                Int32.TryParse(Console.ReadLine(), out int id);
                param1.Value = id;

                cms.Parameters.Add(param1);
                await cms.ExecuteNonQueryAsync();
            }
        }


        static void Main(string[] args)
        {
            try
            {

                Console.WriteLine("Выберите провайдера: ");
                DataTable table = DbProviderFactories.GetFactoryClasses();
                short i = 0;
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine(i.ToString() + "- " + row["InvariantName"]);
                    i++;
                }
                byte providerNumber = Convert.ToByte(Console.ReadLine());

                string providerName = table.Rows[providerNumber]["InvariantName"].ToString();
                fact = DbProviderFactories.GetFactory(providerName);
                conn = fact.CreateConnection();
                string connectionString = GetConnectionStringVByProvider(providerName);
                conn.ConnectionString = connectionString;

                Console.WriteLine("Строка подключения: " + providerName);

                conn.Open();
                Console.WriteLine("Подключение к БД успешно установлено!");
                conn.Close();

            }
            catch (SqlException ex)
            {                
                Console.WriteLine("Ошибка подключения к БД: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Нажмите 0 - закрыть соединение, любую другую клавишу - продолжить");

                if (Console.ReadKey().KeyChar == '0')
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                        Console.WriteLine("Соединение с БД закрыто.");
                    }
                }
                
            }
            Console.Clear();
            int funcNum = -1;
            while (funcNum != 12)
            {
                Console.WriteLine
                (
                    @"Введите номер необходимого действия:
                    1. Отображение всей информации из таблицы со студентами и оценками
                    2. Отображение ФИО всех студентов
                    3. Отображение всех средних оценок
                    4. Показать ФИО всех студентов с минимальной оценкой, больше, чем указанная
                    5. Показать название всех предметов с минимальными средними оценками. Названия предметов должны быть уникальными
                    6. Показать минимальную среднюю оценку
                    7. Показать максимальную среднюю оценку
                    8. Показать количество студентов, у которых минимальная средняя оценка по математике
                    9. Показать количество студентов, у которых максимальная средняя оценка по математике
                    10. Показать количество студентов в каждой группе
                    11. Показать среднюю оценку по группе
                    12. Выход"
                );
                funcNum = int.Parse(Console.ReadLine());
                Console.Clear();
                Tasks taskNum = (Tasks)funcNum;
                
                switch (taskNum)
                {
                    case Tasks.ALL:
                    {                        
                        string sqlInput = @"SELECT *
                                            FROM Students";
                        string[] sqlOutput = { "Id", "FullName", "AverageGrade", "GroupName", "MinSubjectName", "MaxSubjectName" };
                        GetInfo (sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.ALL_FULL_NAMES:
                    {
                        string sqlInput = @"SELECT Id, FullName
                                            FROM Students";
                        string[] sqlOutput = { "Id", "FullName" };
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.ALL_AVG_GRADES:
                    {
                        string sqlInput = @"SELECT Id, AverageGrade
                                            FROM Students";
                        string[] sqlOutput = { "Id", "AverageGrade" };
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.MIN_GRADES_MORE_THAN_TARGET:
                    {
                        showMinGradesMoreThanTarget();
                        break;
                    }
                    case Tasks.ALL_UNIQUE_MIN_GRADES:
                    {
                        string sqlInput = @"SELECT DISTINCT 
                                            Id, MinSubjectName
                                            FROM Students
                                            WHERE AverageGrade = (SELECT MIN(AverageGrade) 
                                            FROM Students);";

                        string[] sqlOutput = { "Id", "MinSubjectName" };
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.MIN_AVG_GRADE:
                    {
                        string sqlInput = @"SELECT MIN(AverageGrade) AS MinimumAverageGrade
                                            FROM Students;";

                        string[] sqlOutput = { "MinimumAverageGrade" };
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.MAX_AVG_GRADE:
                    {
                        string sqlInput = @"SELECT MAX(AverageGrade) AS MaximumAverageGrade
                                            FROM Students;";

                        string[] sqlOutput = { "MaximumAverageGrade" };
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.STUDS_WITH_MIN_MATH:
                    {
                        string sqlInput = @"SELECT COUNT(*) AS StudentsWithMinMathGrade
                                            FROM Students
                                            WHERE MinSubjectName = 'Математика' AND AverageGrade = 
                                            (SELECT MIN(AverageGrade) FROM Students WHERE MinSubjectName = 'Математика')" ;

                        string[] sqlOutput = { "StudentsWithMinMathGrade"};
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.STUDS_WITH_MAX_MATH:
                    {
                        string sqlInput = @"SELECT COUNT(*) AS StudentsWithMaxMathGrade
                        FROM Students
                        WHERE MinSubjectName = 'Математика' AND AverageGrade = 
                        (SELECT MAX(AverageGrade) FROM Students WHERE MinSubjectName = 'Математика')";

                        string[] sqlOutput = { "StudentsWithMaxMathGrade" };
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.COUNT_IN_GROUPS:
                    {
                        string sqlInput = @"SELECT GroupName, COUNT(*) AS StudentCount
                        FROM Students
                        GROUP BY GroupName;";

                        string[] sqlOutput = { "GroupName, StudentCount" };
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.AVG_GRADE_IN_GROUP:
                    {
                        
                        string sqlInput = @"SELECT GroupName, AVG(AverageGrade) AS AverageGrade
                        FROM Students
                        GROUP BY GroupName;";

                        string[] sqlOutput = { "GroupName, AverageGrade" };
                        GetInfo(sqlInput, sqlOutput);
                        break;
                    }
                    case Tasks.EXT:
                    {
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Такого действия не существует");
                        break;
                    }
                    
                }           

            }

        }
        private static string sqlStrCommand = "";

        private static async void GetInfo(string sqlInput, string[] mSqlOutput)
        {
            sqlStrCommand = sqlInput;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlStrCommand, conn as SqlConnection);
                var reader = await cmd.ExecuteReaderAsync();
                int lineCount = 0;
                while (await reader.ReadAsync())
                {
                    if (lineCount == 0)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader.GetName(i) + " ");
                        }
                        Console.WriteLine();
                    }
                    string sqlOutput = "";
                    
                    foreach (var item in mSqlOutput)
                    {
                        sqlOutput += $@"{reader[item]} ";
                    }
                    Console.WriteLine(sqlOutput);
                    lineCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
                stopwatch.Stop();
                Console.WriteLine($"Время выполнения запроса: {stopwatch.ElapsedMilliseconds} ms");
            }
        } 

        private static async void showMinGradesMoreThanTarget()
        {
            sqlStrCommand = @"SELECT Id, FullName
                                FROM Students
                                WHERE AverageGrade > @parMinGrade";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlStrCommand, conn as SqlConnection);
                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = "@parMinGrade";
                Console.WriteLine("Введите минимальную оценку: ");
                float.TryParse(Console.ReadLine(), out float grade);
                param1.Value = grade;

                cmd.Parameters.Add(param1);
                var reader = await cmd.ExecuteReaderAsync();
                int lineCount = 0;
                while (await reader.ReadAsync())
                {
                    if (lineCount == 0)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader.GetName(i) + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine($@"{reader["Id"]} {reader["FullName"]}");
                    lineCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }        
    }
}