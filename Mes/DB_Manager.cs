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

        public void SetConnection()
        {
            connectionString = string.Format("Server={0};Database={1};UID={2};Password={3}", server, database, user, password);
        }

        //*******************************Authentication*******************************//
        public int CheckAuthentication(String username, String password)
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

        public bool CreateCredentialsTable()
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

        //*******************************Sensors*******************************//
        

        public bool CreateSensorTable()
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS " +
                                        "SENSORS ( ID INT AUTO_INCREMENT, ISENABLE BOOL, THRESHOLD INT, LOCATION VARCHAR(255), SENSORTYPE VARCHAR(255) NOT NULL," +
                                        " PARENTID INT NOT NULL, PRIMARY KEY(ID), FOREIGN KEY (PARENTID) REFERENCES SYSTEMS(systemId) )";
                sqlCmd.ExecuteNonQuery();
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

        public int AddSensor(Sensor sensor)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            int id = -1;
            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "INSERT INTO " +
                                          "SENSORS ( ISENABLE, THRESHOLD, LOCATION, SENSORTYPE, PARENTID ) " +
                                          "VALUES ( " + sensor.IsEnabled + ", " + sensor.Threshold + ", \"" + sensor.Location + "\", \"" + sensor.Type + "\", " + sensor.ParentId + ")";

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

        public bool EditSensor(Sensor sensor)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "UPDATE SENSORS " + 
                                        "SET ISENABLE=" + sensor.IsEnabled + ", LOCATION=\"" + sensor.Location + "\",  THRESHOLD=" + sensor.Threshold + ", PARENTID=" + sensor.ParentId +
                                        " WHERE ID=" + sensor.Id;
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
            return true;
        }

        public List<Sensor> GetSensors(int id)
        {
            List<Sensor> sensorsList = new List<Sensor>();
            
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "SELECT * FROM SENSORS";
                if ( id != -1)
                {
                    sqlCmd.CommandText += " WHERE ID=" + id;
                }
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    Sensor sensor = null;
                    switch(rdr.GetString(4))
                    {
                        case "FLOOD":
                            sensor = new FloodSensor();
                            break;
                        case "MAGNETIC":
                            sensor = new MagneticSensor();
                            break;
                        case "SMOKE":
                            sensor = new SmokeSensor();
                            break;
                        case "MOTION":
                            sensor = new MotionSensor();
                            break;
                    }
                        sensor.Id = rdr.GetInt32(0);
                        sensor.IsEnabled = rdr.GetBoolean(1);
                        sensor.Threshold = rdr.GetInt32(2);
                        sensor.Location = rdr.GetString(3);
                        sensor.Type = rdr.GetString(4);
                        sensor.ParentId = rdr.GetInt16(5);
                        sensorsList.Add(sensor);
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
            return sensorsList;
        }

        public bool RemoveSensor(int id)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "DELETE FROM SENSORS " +
                                        "WHERE ID=" + id;
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to database!");
                Console.WriteLine("{0} Exception caught.", e);
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
            return true;
        }

        public bool CreateAlarmTable()
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

        public bool CreateMonitorTable()
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

        public bool CreateSystemTable()
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

        public int AddSystem(int type)
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

        public List<GenericSystem> GetSystems()
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

        public string ViewSensorsTable()
        {
            return null;
        }

        public string ViewAlarmsTable()
        {
            return null;
        }

        public string ViewMonitorsTable()
        {
            return null;
        }
    }
}
