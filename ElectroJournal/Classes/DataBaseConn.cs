using MySql.Data.MySqlClient;
using System;

namespace ElectroJournal.Classes
{
    class DataBaseConn
    {

        public static MySqlConnection GetDBConnection()
        {
            /*
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load("C:/projects/ElectroJournalNetFramework/ElectroJournal/Settings/Settings.xml");
            string server = xmlDocument.GetElementsByTagName("server")[0].InnerText;
            string username = xmlDocument.GetElementsByTagName("username")[0].InnerText;
            string password = xmlDocument.GetElementsByTagName("password")[0].InnerText;
            string database = xmlDocument.GetElementsByTagName("database")[0].InnerText;
            */

            string server = Properties.Settings.Default.Server;
            string username = Properties.Settings.Default.UserName;
            string password = Properties.Settings.Default.Password;
            string database = Properties.Settings.Default.DataBase;
            // Connection String.
            String connString = "Server=" + server + ";Database=" + database + ";User Id=" + username + ";password=" + password;

            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }


        //MySqlConnection DBConnection = new MySqlConnection("server=localhost;  username=root; password=admin; database=journal_db");//локальная бд mysql
        //MySqlConnection DBConnection = new MySqlConnection("server=80.240.250.128; username=Zhirov; password=64580082; database=zhirov_cw");//база КТС
        //MySqlConnection DBConnection = new MySqlConnection("server=nowgun.beget.tech; username=nowgun_project; password=64580082Now; database=nowgun_project");//бд бегета
        /*
        public void OpenConnection()
        {
            if (DBConnection.State == ConnectionState.Closed)
                DBConnection.Open();
        }

        public void ClosedConnection()
        {
            if (DBConnection.State != ConnectionState.Closed)
                DBConnection.Close();
        }

        public MySqlConnection GetConnection()
        {
            return DBConnection;
        }*/
    }
}
