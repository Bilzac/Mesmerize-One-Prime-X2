using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Mes
{
    class DB_Manager
    {
        string connectionString;
        string database = "mes";
        string server = "localhost";
        string user = "root";
        string password = null;

        public void setConnection()
        {
            connectionString = string.Format("Server={0};Database={1};UID={2};Password={3}",server,database,user,password);
        }

        public bool createSensorTable()
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {
                
                sqlConnection.Open();

                sqlCmd.CommandText = "Create Table IF NOT EXISTS " +
                                        "Sensors (monitorId int, isEnable bit, isTriggered bit, location varchar(255), sensorType varchar(255), canTrigger bit)";
                sqlCmd.ExecuteNonQuery();

                return true;
            }
            catch
            {
                Console.WriteLine("Could not connect to database!");
            }
            finally
            {
                sqlConnection.Close();
            }
            return false;
        }

        public bool createAlarmTable()
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {

                sqlConnection.Open();

                sqlCmd.CommandText = "Create Table IF NOT EXISTS " +
                                        "Alarms (monitorId int, isEnable bit, isTriggered bit, location varchar(255), sensorType varchar(255), canTrigger bit)";
                sqlCmd.ExecuteNonQuery();

                return true;
            }
            catch
            {
                Console.WriteLine("Could not connect to database!");
            }
            finally
            {
                sqlConnection.Close();
            }
            return false;
        }

        public bool createMonitorTable()
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {

                sqlConnection.Open();

                sqlCmd.CommandText = "Create Table IF NOT EXISTS " +
                                        "Monitors (monitorId int, isEnable bit, isTriggered bit, location varchar(255), sensorType varchar(255), canTrigger bit)";
                sqlCmd.ExecuteNonQuery();

                return true;
            }
            catch
            {
                Console.WriteLine("Could not connect to database!");
            }
            finally
            {
                sqlConnection.Close();
            }
            return false;
        }

        public string viewSensorsTable()
        {
            return null;
        }

        public string viewAlarmsTable()
        {
            return null;
        }

        public string viewMonitorsTable()
        {
            return null;
        }
    }
}
