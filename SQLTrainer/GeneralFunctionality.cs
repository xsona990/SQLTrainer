using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SQLTrainer
{
    internal class GeneralFunctionality
    {
        #region Private Fields
        private const string DatabaseName = "test.sqlite";
        private const string ConnectionString = "Data Source=" + DatabaseName + ";Version=3;";
        private SQLiteConnection connection = new SQLiteConnection();
        #endregion

        protected SQLiteCommand command = new SQLiteCommand();
        protected SQLiteDataReader reader;
        protected SQLiteDataAdapter adapter;

        internal GeneralFunctionality()
        {
            FirstTimeLaunch();//При каждом создании обьекта класса, посредством метода проверяем есть ли БД.

        }

        /// <summary>
        /// Метод, выполняющий переданную команду
        /// </summary>
        /// <param name="SQLState"></param>
        /// <returns></returns>
        internal DataTable ExecuteStatement(string SQLState)
        {
            DataTable ResultTable = new DataTable();
            try
            {
                if (OpenConnection() == true)
                {
                    command = connection.CreateCommand();
                    command.CommandText = SQLState;
                    if (command.ExecuteScalar() != null)
                    {
                        using (reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable ShemaTable = reader.GetSchemaTable();
                                foreach (DataRow row in ShemaTable.Rows)
                                {
                                    string colName = row.Field<string>("ColumnName");
                                    Type t = row.Field<Type>("DataType");
                                    ResultTable.Columns.Add(colName, t);
                                }
                                while (reader.Read())
                                {
                                    var newRow = ResultTable.Rows.Add();
                                    foreach (DataColumn col in ResultTable.Columns)
                                    {
                                        newRow[col.ColumnName] = reader[col.ColumnName];
                                    }
                                }
                            }
                        }
                    }

                    CloseConnection();
                }
            }
            catch (SQLiteException ex)
            {
                MainWindow.ShowException(ex.Message);
            }
            return ResultTable;
        }
        /// <summary>
        /// Метод возвращающий таблицу из базы данных по ее имени
        /// </summary>
        /// <param name="TableName">Имя таблицы в базе данных которую мы хотим получить</param>
        /// <returns>Обьект DataTable с таблицой</returns>
        internal DataTable GetTable(string TableName)
        {
            DataTable Table = new DataTable();
            try
            {
                if (OpenConnection() == true)
                {
                    command.CommandText = String.Format("SELECT * FROM {0}", TableName);
                    adapter = new SQLiteDataAdapter(command);
                    adapter.AcceptChangesDuringFill = false;
                    adapter.Fill(Table);
                    Table.TableName = TableName;

                    CloseConnection();
                }
            }
            catch (SQLiteException ex)
            {
                MainWindow.ShowException(ex.Message);
            }
            return Table;
        }
        /// <summary>
        /// Метод который пересоздает БД и вызывает метод FirstTimeLaunch()
        /// </summary>
        internal void ReturnToDefault()
        {
            try
            {
                if (OpenConnection() == true)
                {
                    command = connection.CreateCommand();
                    command.CommandText = "DROP TABLE users; DROP TABLE userdata; DROP TABLE userrooms; ";
                    command.ExecuteNonQuery();
                    if (CloseConnection() == true)
                    {
                        FirstTimeLaunch(true);

                    }
                }
            }
            catch (SQLiteException ex)
            {
                MainWindow.ShowException(ex.Message);
            }
        }
        /// <summary>
        /// Метод возвращающий имена всех таблиц которые есть в базе данных
        /// </summary>
        /// <returns>List<string></returns>
        internal List<string> GetAllTableNames()
        {
            List<string> DataBaseNames = new List<string>();
            try
            {
                if (OpenConnection() == true)
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name <> 'sqlite_sequence' ORDER BY name; ";
                    //command.ExecuteNonQuery();
                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataBaseNames.Add(reader.GetString(0));
                        }
                    }
                    CloseConnection();
                }
            }
            catch (SQLiteException ex)
            {
                MainWindow.ShowException(ex.Message);
            }
            return DataBaseNames;
        }

        /// <summary>
        /// Метод, который проверяет наличие созданной базы данных, и таблиц с необходимыми данными в ней.
        /// Если их нет, создает и заполняет необходимыми данными.
        /// </summary>
        /// <returns>Возвращает true только в том случае, если есть база и необходимые таблицы.</returns>
        private void FirstTimeLaunch(bool returnToDefault = false)
        {
            if (!File.Exists(DatabaseName))
            {
                SQLiteConnection.CreateFile(DatabaseName);

                try
                {
                    if (OpenConnection() == true)
                    {
                        command = connection.CreateCommand();
                        command.CommandText = "CREATE TABLE IF NOT EXISTS `users` (`userid`	INTEGER NOT " +
                            "NULL PRIMARY KEY AUTOINCREMENT, `username`	TEXT); " +
                            "CREATE TABLE IF NOT EXISTS`userdata` (`phoneid` INTEGER NOT NULL PRIMARY " +
                            "KEY AUTOINCREMENT,`userid`	INTEGER NOT NULL, `phonenumber`	INTEGER NOT NULL); " +
                            "CREATE TABLE IF NOT EXISTS `userrooms` (`roomid` INTEGER NOT NULL PRIMARY " +
                            "KEY AUTOINCREMENT, `phoneid` INTEGER NOT NULL, `roomnumber` INTEGER NOT NULL);";
                        command.ExecuteNonQuery();
                        command = connection.CreateCommand();
                        command.CommandText = "INSERT INTO users ('username') VALUES ('foo'); " +
                            "INSERT INTO users('username') VALUES('bar'); " +
                            "INSERT INTO users('username') VALUES('baz'); " +
                            "INSERT INTO users('username') VALUES('qux'); " +
                            "INSERT INTO userdata('userid', 'phonenumber') VALUES('2', '200'); " +
                            "INSERT INTO userdata('userid', 'phonenumber') VALUES('4', '201'); " +
                            "INSERT INTO userdata('userid', 'phonenumber') VALUES('3', '202'); " +
                            "INSERT INTO userdata('userid', 'phonenumber') VALUES('1', '203'); " +
                            "INSERT INTO userrooms('phoneid', 'roomnumber') VALUES('4', '30'); " +
                            "INSERT INTO userrooms('phoneid', 'roomnumber') VALUES('1', '32'); " +
                            "INSERT INTO userrooms('phoneid', 'roomnumber') VALUES('2', '35'); " +
                            "INSERT INTO userrooms('phoneid', 'roomnumber') VALUES('3', '50'); ";
                        command.ExecuteNonQuery();

                        CloseConnection();
                    }
                }
                catch (SQLiteException ex)
                {
                    MainWindow.ShowException(ex.Message);
                }
            }
            else if (File.Exists(DatabaseName) && returnToDefault == true)//  Необходимо для того, что бы дублирующиеся данные в таблицу не добавлялись при каждом запуске.
            {
                try
                {
                    if (OpenConnection() == true)
                    {
                        command = connection.CreateCommand();
                        command.CommandText = "CREATE TABLE IF NOT EXISTS `users` (`userid`	INTEGER NOT " +
                            "NULL PRIMARY KEY AUTOINCREMENT, `username`	TEXT); " +
                            "CREATE TABLE IF NOT EXISTS`userdata` (`phoneid` INTEGER NOT NULL PRIMARY " +
                            "KEY AUTOINCREMENT,`userid`	INTEGER NOT NULL, `phonenumber`	INTEGER NOT NULL); " +
                            "CREATE TABLE IF NOT EXISTS `userrooms` (`roomid` INTEGER NOT NULL PRIMARY " +
                            "KEY AUTOINCREMENT, `phoneid` INTEGER NOT NULL, `roomnumber` INTEGER NOT NULL);";
                        command.ExecuteNonQuery();
                        command = connection.CreateCommand();
                        command.CommandText = "INSERT INTO users ('username') VALUES ('foo'); " +
                            "INSERT INTO users('username') VALUES('bar'); " +
                            "INSERT INTO users('username') VALUES('baz'); " +
                            "INSERT INTO users('username') VALUES('qux'); " +
                            "INSERT INTO userdata('userid', 'phonenumber') VALUES('2', '200'); " +
                            "INSERT INTO userdata('userid', 'phonenumber') VALUES('4', '201'); " +
                            "INSERT INTO userdata('userid', 'phonenumber') VALUES('3', '202'); " +
                            "INSERT INTO userdata('userid', 'phonenumber') VALUES('1', '203'); " +
                            "INSERT INTO userrooms('phoneid', 'roomnumber') VALUES('4', '30'); " +
                            "INSERT INTO userrooms('phoneid', 'roomnumber') VALUES('1', '32'); " +
                            "INSERT INTO userrooms('phoneid', 'roomnumber') VALUES('2', '35'); " +
                            "INSERT INTO userrooms('phoneid', 'roomnumber') VALUES('3', '50'); ";
                        command.ExecuteNonQuery();

                        CloseConnection();
                    }
                }
                catch (SQLiteException ex)
                {
                    MainWindow.ShowException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Открывает соединение с базой данных
        /// </summary>
        /// <returns>Возвращает true только в том случае, если соединение было открыто</returns>
        protected bool OpenConnection()
        {
            try
            {
                connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SQLiteException ex)
            {
                MainWindow.ShowException(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// Метод который закрывает соединение с базой данных
        /// </summary>
        /// <returns>Возвращает true только если соединение было закрыто.</returns>
        protected bool CloseConnection()
        {
            try
            {
                this.connection.Close();
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SQLiteException ex)
            {
                MainWindow.ShowException(ex.Message);
                return false;
            }
        }

    }
}
