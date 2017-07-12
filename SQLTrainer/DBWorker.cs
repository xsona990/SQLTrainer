using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTrainer
{
    class DBWorker
    {
        private String dbFileName;
        private SQLiteConnection dbConn;
        private SQLiteCommand sqlCmd;
        private SQLiteDataReader reader;
        private SQLiteDataAdapter adapter;
        internal DBWorker()
        {
            this.dbFileName = "test.sqlite";
            dbConn = new SQLiteConnection();
            sqlCmd = new SQLiteCommand();
            Initalize();
        }


        internal DataTable GetTable(string TableName)
        {
            DataTable DT = new DataTable();
            dbConn.Open();
            sqlCmd = dbConn.CreateCommand();
            sqlCmd.CommandText = string.Format("SELECT * FROM {0}", TableName);
            adapter = new SQLiteDataAdapter(sqlCmd);
            adapter.AcceptChangesDuringFill = false;
            adapter.Fill(DT);
            dbConn.Close();
            DT.TableName = TableName;
            foreach (DataRow row in DT.Rows)
            {
                row.AcceptChanges();
            }
            return DT;
        }

        internal DataTable Execute(string SQLStatement)
        {
            DataTable DT = new DataTable();
            try
            { 
                dbConn.Open();
                sqlCmd = dbConn.CreateCommand();
                sqlCmd.CommandText = SQLStatement;
                adapter = new SQLiteDataAdapter(sqlCmd);
                adapter.AcceptChangesDuringFill = false;
                adapter.Fill(DT);
                dbConn.Close();
                foreach (DataRow row in DT.Rows)
                {
                    row.AcceptChanges();
                }
            }
            catch (SQLiteException ex)
            {

            }         
            return DT;
        }

        private void Initalize()
        {
           
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);
            try
            {
                dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                dbConn.Open();
                sqlCmd.Connection = dbConn;
                sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS `users` (`user_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,	`username`	TEXT); CREATE TABLE IF NOT EXISTS`userdata` (`phone_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,`user_id`	INTEGER NOT NULL, `phone_number`	INTEGER NOT NULL); CREATE TABLE IF NOT EXISTS `userrooms` (`room_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `phone_id`	INTEGER NOT NULL, `room_number`	INTEGER NOT NULL);";
                sqlCmd.ExecuteNonQuery();
                if (!GetAllTableNames().Contains("users") && !GetAllTableNames().Contains("userdata")&& !GetAllTableNames().Contains("userrooms"))
                {
                    sqlCmd.CommandText = "INSERT INTO users ('username') VALUES ('foo'); INSERT INTO users('username') VALUES('bar'); INSERT INTO users('username') VALUES('baz'); INSERT INTO users('username') VALUES('qux'); INSERT INTO userdata('user_id', 'phone_number') VALUES('2', '200'); INSERT INTO userdata('user_id', 'phone_number') VALUES('4', '201'); INSERT INTO userdata('user_id', 'phone_number') VALUES('3', '202'); INSERT INTO userdata('user_id', 'phone_number') VALUES('1', '203'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('4', '30'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('1', '32'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('2', '35'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('3', '50'); ";
                    sqlCmd.ExecuteNonQuery();
                }           
                dbConn.Close();
            }
            catch (SQLiteException ex)
            {
                throw;
            }
        }

        internal List<string> GetAllTableNames()
        {
            List<string> rezult = new List<string>();
            try
            {
                dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                dbConn.Open();
                sqlCmd.Connection = dbConn;
                sqlCmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name <> 'sqlite_sequence' ORDER BY name; ";
                using (reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rezult.Add(reader.GetString(0));
                    }
                }
                dbConn.Close();
            }
            catch (SQLiteException)
            {
                throw;
            }

            return rezult;
        }

        internal void ReturnToDefault()
        {
           
            try
            {
                dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                dbConn.Open();
                sqlCmd.Connection = dbConn;
                sqlCmd.CommandText = "DROP TABLE users; DROP TABLE userdata; DROP TABLE userrooms; ";
                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS `users` (`user_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,	`username`	TEXT); CREATE TABLE IF NOT EXISTS`userdata` (`phone_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,`user_id`	INTEGER NOT NULL, `phone_number`	INTEGER NOT NULL); CREATE TABLE IF NOT EXISTS `userrooms` (`room_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `phone_id`	INTEGER NOT NULL, `room_number`	INTEGER NOT NULL);";
                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "INSERT INTO users ('username') VALUES ('foo'); INSERT INTO users('username') VALUES('bar'); INSERT INTO users('username') VALUES('baz'); INSERT INTO users('username') VALUES('qux'); INSERT INTO userdata('user_id', 'phone_number') VALUES('2', '200'); INSERT INTO userdata('user_id', 'phone_number') VALUES('4', '201'); INSERT INTO userdata('user_id', 'phone_number') VALUES('3', '202'); INSERT INTO userdata('user_id', 'phone_number') VALUES('1', '203'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('4', '30'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('1', '32'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('2', '35'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('3', '50'); ";
                sqlCmd.ExecuteNonQuery();
                dbConn.Close();
            }
            catch (SQLiteException)
            {
                throw;
            }
        }

    }
}
