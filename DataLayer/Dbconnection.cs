﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Dbconnection
    {
        public SqlConnection con = new SqlConnection("Data Source=10.10.10.46;Initial Catalog=DonBest;User ID=luisma;Password=12345678");

        public SqlConnection getConn()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            return con;

        }

        public int ExeNonQuery(SqlCommand cmd)
        {
            cmd.Connection = getConn();
            int rowsaffected = -1;

            rowsaffected = cmd.ExecuteNonQuery();
            con.Close();
            return rowsaffected;

        }

     

        public object ExeStoredProcedure(string storedProcedureName)
        {
            using (con)
            {
                con.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, con) { CommandType = System.Data.CommandType.StoredProcedure })
                {

                    command.Parameters.AddWithValue("@YourSpParameter", "ParameterValue");
                    command.Parameters.AddWithValue("@YourSpParameter", "ParameterValue");

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //read the data
                        while (reader.Read())
                        {
                            //label1.Text = reader["yourcolumn"].ToString();
                        }

                        return reader;
                    }


                }
            }
          
        }


        public object ExeScalar(string insertString)
        {

            SqlCommand cmd = new SqlCommand(insertString, con);
            cmd.Connection = getConn();
            object obj = -1;
            try
            { 
            obj = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error:" + ex.Message);
            }
            con.Close();
            return obj;


        }

        public DataTable ExeReader(SqlCommand cmd)
        {
            cmd.Connection = getConn();
            SqlDataReader sdr;
            DataTable dt = new DataTable();

            sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            return dt;



        }

    }
}
