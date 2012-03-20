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
                sqlCmd.CommandText = "SELECT usertype FROM Credentials WHERE username=\"" + username + "\" AND password=\"" + password + "\""; //Check if any admin credentials exist.
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
                                        "Credentials ( username varchar(255) NOT NULL UNIQUE, password varchar(255) NOT NULL, usertype int NOT NULL , PRIMARY KEY(username) )";
                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "SELECT count(username) FROM Credentials WHERE usertype=1"; //Check if any admin credentials exist.
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                if(rdr.Read())
                {
                    if (rdr.GetInt32(0) == 0)
                    {
                        sqlCmd.CommandText = "INSERT INTO CREDENTIALS (username, password, usertype) VALUES (\"admin\",\"adminpass\",1)";
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

        public bool AddUser(string username, string password, int type)
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {
                sqlConnection.Open();

                sqlCmd.CommandText = "INSERT INTO " +
                                  "CREDENTIALS ( USERNAME, PASSWORD, USERTYPE ) " +
                                  "VALUES ( \"" + username + "\", \"" + password + "\", " + type + ")";
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
            return true;
        }

        public bool ChangePassword(string username,string password)
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {
                sqlConnection.Open();

                sqlCmd.CommandText = "UPDATE CREDENTIALS " +
                        "SET PASSWORD=\"" + password + "\" " +
                        "WHERE USERNAME=\"" + username +"\"";
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

        public List<Sensor> GetSensors(int id, int parentId)
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
                else if (parentId != -1)
                {
                    sqlCmd.CommandText += " WHERE PARENTID=" + parentId;
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

        //*******************************Alarms*******************************//


        public bool CreateAlarmTable()
        {

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {

                sqlConnection.Open();

                sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS " +
                                        "ALARMS ( ID INT AUTO_INCREMENT, ISENABLE BOOL, SENSITIVITY INT, LOCATION VARCHAR(255), ALARMTYPE VARCHAR(255) NOT NULL," +
                                        " PARENTID INT NOT NULL, PRIMARY KEY(ID), FOREIGN KEY (PARENTID) REFERENCES SYSTEMS(systemId) )";
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

        public int AddAlarm(Alarm alarm)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            int id = -1;
            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "INSERT INTO " +
                                          "ALARMS ( ISENABLE, SENSITIVITY, LOCATION, ALARMTYPE, PARENTID ) " +
                                          "VALUES ( " + alarm.IsEnabled + ", " + alarm.Sensitivity + ", \"" + alarm.Location + "\", \"" + alarm.Type + "\", " + alarm.ParentId + ")";

                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "SELECT LAST_INSERT_ID() FROM ALARMS";
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

        public bool EditAlarm(Alarm alarm)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "UPDATE ALARMS " +
                                        "SET ISENABLE=" + alarm.IsEnabled + ", LOCATION=\"" + alarm.Location + "\",  SENSITIVITY=" + alarm.Sensitivity + ", PARENTID=" + alarm.ParentId +
                                        " WHERE ID=" + alarm.Id;
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

        public List<Alarm> GetAlarms(int id, int parentId)
        {
            List<Alarm> alarmList = new List<Alarm>();

            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();

            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "SELECT * FROM ALARMS";
                if (id != -1)
                {
                    sqlCmd.CommandText += " WHERE ID=" + id;
                }
                else if (parentId != -1)
                {
                    sqlCmd.CommandText += " WHERE PARENTID=" + parentId;
                }
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    Alarm alarm = null;
                    switch (rdr.GetString(4))
                    {
                        case "SIREN":
                            alarm = new SirenAlarm();
                            break;
                        case "LIGHT":
                            alarm = new LightAlarm();
                            break;
                    }
                    alarm.Id = rdr.GetInt32(0);
                    alarm.IsEnabled = rdr.GetBoolean(1);
                    alarm.Sensitivity = rdr.GetInt32(2);
                    alarm.Location = rdr.GetString(3);
                    alarm.Type = rdr.GetString(4);
                    alarm.ParentId = rdr.GetInt16(5);
                    alarmList.Add(alarm);
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
            return alarmList;
        }

        public bool RemoveAlarm(int id)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            MySqlCommand sqlCmd = sqlConnection.CreateCommand();
            try
            {
                sqlConnection.Open();
                sqlCmd.CommandText = "DELETE FROM ALARMS " +
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
                        SecuritySystem system = new SecuritySystem(rdr.GetInt32(0));
                        system.Sensors = GetSensors(-1, system.Id);
                        system.Alarms = GetAlarms(-1, system.Id);
                        systemList.Add(system);
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
