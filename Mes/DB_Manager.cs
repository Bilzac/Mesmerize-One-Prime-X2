using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Mes
{
    class DB_Manager
    {
        SqlConnection sql_Connection = new SqlConnection(Mes.Properties.Settings.Default.MesConnectionString);

    }
}
