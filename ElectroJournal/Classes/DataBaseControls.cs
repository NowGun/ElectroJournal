using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography;

namespace ElectroJournal.Classes
{
    class DataBaseControls
    {
        Classes.DataBase DbUser = new Classes.DataBase();
        MySqlConnection conn = DataBase.GetDBConnection();

        public string Hash(string password)
        {
            MD5 md5hasher = MD5.Create();

            var data = md5hasher.ComputeHash(Encoding.Default.GetBytes(password));

            return Convert.ToBase64String(data);
        }

        public void DeleteHousing(string name)
        {
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
            else return false;
        }

        public bool IsCabinetExists(string cabinet)
        {
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
            else return false;
        }

        public void DeleteTeachers(int id)
        {
            MySqlCommand command = new MySqlCommand("DELETE FROM `teachers` WHERE `idteachers` = @id", conn);
            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;

            conn.Open();

            if (command.ExecuteNonQuery() == 1)
            {
                conn.Close();
            }
        }

        public bool IsTeachersLoginExists(string login)
        {
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT count(`teachers_login`) FROM `teachers` WHERE `teachers_login` = @login", conn);

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = login;

            conn.Open();
            int a = command.ExecuteNonQuery();
            conn.Close();
            if (a == 1)
            {
                return true;
            }
            else return false;
        }
    }
}
