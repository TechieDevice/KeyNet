using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace KeyNetServer
{
    class DB
    {
        private static SQLiteConnection DBConn;

        public static void Connect()
        {
            DBConn = new SQLiteConnection(@"Data Sourse=D:\KeyNet\KeyNetServer\KeyNetServer\db\DB.db; Version=3");
            DBConn.Open();
        }

        public static void Close()
        {
            DBConn.Close();
        }

        public static void Add(string Name, string Key, string Mark)
        {
            SQLiteCommand CMD = DBConn.CreateCommand();
            CMD.CommandText = "insert into signatures(name, key, mark) values('"+Name+"','"+Key+"','"+Mark+"')";
        }

        public static string[] Search(string param, string value)
        {
            string[] data = new string[3];
            SQLiteCommand CMD = DBConn.CreateCommand();
            CMD.CommandText = "select * from signatures where "+param+" like "+value;
            SQLiteDataReader reader = CMD.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                string name = reader["name"].ToString();
                string Key = reader["key"].ToString();
                string Mark = reader["mark"].ToString();
            }
            return data;
        }
    }
}
