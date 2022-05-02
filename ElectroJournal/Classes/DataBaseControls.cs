using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ElectroJournal.Classes
{
    class DataBaseControls
    {
        public string Hash(string password)
        {
            MD5 md5hasher = MD5.Create();

            var data = md5hasher.ComputeHash(Encoding.Default.GetBytes(password));

            return Convert.ToBase64String(data);
        }

        public void DeleteHousing(string name)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            MySqlCommand command = new MySqlCommand("DELETE FROM `housing` WHERE `housing_name` = @name", conn);
            command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;

            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
            }
        }

        public void DeleteCabinet(string name)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            MySqlCommand command = new MySqlCommand("DELETE FROM `cabinet` WHERE `cabinet_number` = @name", conn);
            command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;

            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
            }
        }

        public bool IsHousingExists(string name)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT `housing_name` FROM `housing` WHERE `housing_name` = @name", conn);

            command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsCabinetExists(string cabinet)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `cabinet` WHERE `cabinet_number` = @NumberCabinet", conn);

            command.Parameters.Add("@NumberCabinet", MySqlDbType.VarChar).Value = cabinet;
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DeleteTeachers(int id)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            MySqlCommand command = new MySqlCommand("DELETE FROM `teachers` WHERE `idteachers` = @id", conn);
            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;

            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
            }
        }

        public void DeleteStudent(int id)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            MySqlCommand command = new MySqlCommand("DELETE FROM `students` WHERE `idstudents` = @id", conn);
            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;

            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
            }
        }

        public void DeleteGroup(int id)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            MySqlCommand command = new MySqlCommand("DELETE FROM `groups` WHERE `idgroups` = @id", conn);
            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;

            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
            }
        }

        public bool IsTeachersLoginExists(string login)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            MySqlCommand command = new MySqlCommand("SELECT count(`teachers_login`) FROM `teachers` WHERE `teachers_login` = @login", conn);

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = login;

            conn.Open();
            MySqlDataReader read = command.ExecuteReader();

            if (read.Read())
            {
                int a = read.GetInt32(0);
                conn.Close();
                if (a >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public bool IsUserExists(string login)
        {
            Classes.DataBaseConn DbUser = new Classes.DataBaseConn();
            MySqlConnection conn = DataBaseConn.GetDBConnection();
            MySqlCommand command = new MySqlCommand("SELECT count(User) FROM mysql.user where User = @login", conn);

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = login;

            conn.Open();
            MySqlDataReader read = command.ExecuteReader();

            if (read.Read())
            {
                int a = read.GetInt32(0);
                conn.Close();
                if (a >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
