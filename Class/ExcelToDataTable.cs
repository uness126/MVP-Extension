﻿using System;
using System.Data;
using System.Globalization;
using System.Data.OleDb;

namespace WhaleExtension.Class
{
    public class ExcelToDataTable
    {
        public static DataTable Convert(string filePath)
        {
            DataTable dtexcel = new DataTable();
            try
            {
                bool hasHeaders = true;
                string HDR = hasHeaders ? "Yes" : "No";
                string strConn;
                if (filePath.Substring(filePath.LastIndexOf('.')).ToLower() == ".xlsx")
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\"";
                else
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=0\"";
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                //Looping Total Sheet of Xl File
                /*foreach (DataRow schemaRow in schemaTable.Rows)
                {
                }*/
                //Looping a first Sheet of Xl File
                DataRow schemaRow = schemaTable.Rows[0];
                string sheet = schemaRow["TABLE_NAME"].ToString();
                if (!sheet.EndsWith("_"))
                {
                    string query = "SELECT  * FROM [" + sheet + "]";
                    OleDbDataAdapter daexcel = new OleDbDataAdapter(query, conn);
                    dtexcel.Locale = CultureInfo.CurrentCulture;
                    daexcel.Fill(dtexcel);
                }

                conn.Close();
                return dtexcel;
            }
            catch (Exception exc)
            {
                Logger.WriteLOG(exc, "ExcelToDataTable.Convert");
                throw new Exception("پیش نیازهای برنامه نصب نشده اند!!!");
            }

        }
    }
}
