using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using MySql.Data;

namespace Mes
{
    class DB_Manager
    {
        string connectionString;

        public void setConnection(string _database,string _server,string _user,string _password)
        {
            connectionString = string.Format("Server={0};Database={1};UID={2};Password={3}",_server,_database,_user,_password);
        }

        public bool createSensorTable()
        {
            return false;
        }

        public bool createAlarmTable()
        {
            return false;
        }

        public bool createMonitorTable()
        {
            return false;
        }
    }
}
