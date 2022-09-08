using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DalApi;
using DO;


namespace DalDB
{
    public class ConnectDB
    {
        ///public static SqlConnection conn;
      public  SqlCommand connectDB ()
        {
            string connectionString; 
                
            SqlConnection connection= new SqlConnection();
            try
            {
                connectionString = @"Data Source=DESKTOP-IJPTHU9; Integrated Security=True; User ID=sa;";
              
                using var conection = new SqlConnection(connectionString);
                conection.Open();
                var command = "SELECT @@VERSION";
                using var comunicator = new SqlCommand(command, conection);
                comunicator.CommandText = "CREATE TABLE [dbo].[test]( [Id] INT NOT NULL PRIMARY KEY, [Model] VARCHAR(50) NULL, [MaxWeight] VARCHAR(50) NULL) ";
                comunicator.ExecuteNonQuery();  
                return comunicator;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There's an error connecting to the database!\n" + ex.Message);
            }
            return null;
        }
        
    }
}
