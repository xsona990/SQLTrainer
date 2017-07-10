using System;
using System.Collections.Generic;
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

        internal DBWorker()
        {
            this.dbFileName = "test.sqlite";
            dbConn = new SQLiteConnection();
            sqlCmd = new SQLiteCommand();
            Initalize();
        }


        public string Initalize()
        {
            string error = "0";
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);
            try
            {
                dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                dbConn.Open();
                sqlCmd.Connection = dbConn;
                sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS `users` (`user_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,	`username`	TEXT); CREATE TABLE IF NOT EXISTS`userdata` (`phone_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,`user_id`	INTEGER NOT NULL, `phone_number`	INTEGER NOT NULL); CREATE TABLE IF NOT EXISTS `userrooms` (`room_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `phone_id`	INTEGER NOT NULL, `room_number`	INTEGER NOT NULL);";
                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "INSERT INTO users ('username') VALUES ('foo'); INSERT INTO users('username') VALUES('bar'); INSERT INTO users('username') VALUES('baz'); INSERT INTO users('username') VALUES('qux'); INSERT INTO userdata('user_id', 'phone_number') VALUES('2', '200'); INSERT INTO userdata('user_id', 'phone_number') VALUES('4', '201'); INSERT INTO userdata('user_id', 'phone_number') VALUES('3', '202'); INSERT INTO userdata('user_id', 'phone_number') VALUES('1', '203'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('4', '30'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('1', '32'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('2', '35'); INSERT INTO userrooms('phone_id', 'room_number') VALUES('3', '50'); ";
                sqlCmd.ExecuteNonQuery();
                dbConn.Close();
            }
            catch (SQLiteException ex)
            {
                error = ex.ToString();
                // MessageBox.Show("Error: " + ex.Message);
            }
            return error;
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
                using (SQLiteDataReader reader = sqlCmd.ExecuteReader())
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
                sqlCmd.CommandText = "CREATE DATABASE IF NOT EXISTS test; CREATE TABLE `users` (`user_id` INT(5) NOT NULL AUTO_INCREMENT, `username` VARCHAR(50), PRIMARY KEY(`user_id`), INDEX(`username`)); CREATE TABLE `userdata` (`phone_id` INT(5) NOT NULL AUTO_INCREMENT, `user_id` INT(5) NOT NULL, phone_number INT(10) NOT NULL, PRIMARY KEY(`phone_id`), INDEX(`user_id`, `phone_number`)); CREATE TABLE `userrooms` (`room_id` INT(5) NOT NULL AUTO_INCREMENT, `phone_id` INT(5) NOT NULL, `room_number` INT(4) NOT NULL, PRIMARY KEY(`room_id`), INDEX(`phone_id`, `room_number`)); ";
                sqlCmd.ExecuteNonQuery();
                sqlCmd.CommandText = "INSERT INTO users (username) VALUE ('foo'); INSERT INTO users(username) VALUE('bar'); INSERT INTO users(username) VALUE('baz'); INSERT INTO users(username) VALUE('qux'); INSERT INTO userdata(user_id, phone_number) VALUE('2', '200'); INSERT INTO userdata(user_id, phone_number) VALUE('4', '201'); INSERT INTO userdata(user_id, phone_number) VALUE('3', '202'); INSERT INTO userdata(user_id, phone_number) VALUE('1', '203'); INSERT INTO userrooms(phone_id, room_number) VALUE('4', '30'); INSERT INTO userrooms(phone_id, room_number) VALUE('1', '32'); INSERT INTO userrooms(phone_id, room_number) VALUE('2', '35'); INSERT INTO userrooms(phone_id, room_number) VALUE('3', '50'); ";
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
