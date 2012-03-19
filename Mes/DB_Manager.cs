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
            connectionString = string.Format("Server={0};Database={1};UID={2};Password={3}", server, database, user, password);
        }

        public int checkAuthentication(String username, String password)
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            int type = -1;

            try
            { 
                sqlConnection.Open();
                sqlCmd.CommandText = "SELECT type FROM Credentials WHERE username=\"" + username + "\" AND password=\"" + password + "\""; //Check if any admin credentials exist.
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                if (rdr.Read())
                {
                    type = rdr.GetInt32(0);  
                }
                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return type;
        }

        public bool createCredentialsTable()
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {
                sqlConnection.Open();

                sqlCmd.CommandText = "Create Table IF NOT EXISTS " +
                                        "Credentials ( username varchar(255) NOT NULL, password varchar(255) NOT NULL, type int NOT NULL , PRIMARY KEY(username) )";
                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "SELECT count(username) FROM Credentials WHERE type=1"; //Check if any admin credentials exist.
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                if(rdr.Read())
                {
                    if (rdr.GetInt32(0) == 0)
                    {
                        sqlCmd.CommandText = "INSERT INTO CREDENTIALS (username, password, type) VALUES (\"admin\",\"adminpass\",1)";
                    }
                }
                rdr.Close();
                sqlCmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return false;
        }


        public bool createSensorTable()
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {

                sqlConnection.Open();

                sqlCmd.CommandText = "Create Table IF NOT EXISTS " +
                                        "Sensors ( id int AUTO_INCREMENT, isEnable bit, isTriggered bit, location varchar(255), sensorType varchar(255)," + 
                                        "canTrigger bit, parentId int NOT NULL, PRIMARY KEY(id), FOREIGN KEY (parentId) REFERENCES SYSTEMS(systemId) )";
                sqlCmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return false;
        }

        public int addNewSensor(int type ,int parentId)
        {
            return updateSensorTableRow(type, parentId, null);
        }

        public int updateSensor(Sensor sensor)
        {
            return updateSensorTableRow(-1, -1, sensor);
        }

        private int updateSensorTableRow(int type, int parentId, Sensor sensor)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            int id = -1;
            try
            {

                sqlConnection.Open();

                if (sensor != null)
                {
                    sqlCmd.CommandText = "UPDATE SENSORS " +
                                            "SET isEnable=" + sensor.IsEnabled + ", isTriggered=" + sensor.IsTriggered + "  , location= \"" + sensor.Location + "\" " +
                                            "WHERE id=" + sensor.Id + ";";
                }
                else
                {
                    sqlCmd.CommandText = "INSERT INTO " +
                                            "SENSORS ( isEnable, isTriggered, sensorType, parentId)" +
                                            "VALUES ( false, false," + type + ","+ parentId +")";
                }

                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "SELECT LAST_INSERT_ID() FROM SENSORS";
                id = Convert.ToInt32(sqlCmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return id;
        }

        public List<GenericSystem> getSensors()
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            List<GenericSystem> systemList = new List<GenericSystem>();

            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "SELECT * FROM SYSTEMS";
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr.GetInt32(1) == 1)
                    {
                        systemList.Add(new SecuritySystem(rdr.GetInt32(0)));
                    }
                }
                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return systemList;
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
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
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
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return false;
        }

        public bool createSystemTable()
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {

                sqlConnection.Open();

                sqlCmd.CommandText = "Create Table IF NOT EXISTS " +
                                        "Systems (systemId int NOT NULL AUTO_INCREMENT, systemType int NOT NULL, PRIMARY KEY(systemId) )";
                sqlCmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return false;
        }

        public int addSystem(int type)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            int id = -1;
            try
            {
                sqlConnection.Open();

                sqlCmd.CommandText = "INSERT INTO " +
                                        "Systems (systemType) VALUES (" + type + ")";

                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "SELECT LAST_INSERT_ID() FROM Systems";
                id = Convert.ToInt32(sqlCmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return id;
        }

        public List<GenericSystem> getSystems()
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            List<GenericSystem> systemList = new List<GenericSystem>();

            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "SELECT * FROM SYSTEMS";
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr.GetInt32(1) == 1)
                    {
                        systemList.Add(new SecuritySystem(rdr.GetInt32(0)));
                    }
                }
                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
            }
            finally
            {
                sqlConnection.Close();
            }
            return systemList;
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
