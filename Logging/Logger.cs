using WhaleExtension.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace WhaleExtension
{
    public class Logger
    {
        #region Variables
        const long lng = 204800;

        #endregion

        public static void WriteLOG(Exception ex, string Method = null)
        {
            try
            {
                var text = string.Empty;
                text = " " + Method + " : ";
                //}
                while (ex != null)
                {
                    text += ex.GetType().FullName;
                    text += Environment.NewLine;
                    text += ex.Source;
                    text += Environment.NewLine;
                    text += ex.ToString();
                    ex = ex.InnerException;
                    text += Environment.NewLine + "**************************************************************************************************************************************** ";
                }

                if (File.Exists(Application.StartupPath + "\\log.um"))
                {
                    long length = new System.IO.FileInfo(Application.StartupPath + "\\log.um").Length;
                    StreamWriter sr;
                    if (length > lng)
                        sr = new StreamWriter(Application.StartupPath + "\\log.um", false);
                    else
                        sr = new StreamWriter(Application.StartupPath + "\\log.um", true);
                    sr.WriteLine(DateTime.Now.Convert2Persian() + " | " + DateTime.Now.ToString("HH:mm") + " | " + text);
                    sr.Close();
                }
                else
                {
                    using (TextWriter sr = new StreamWriter(Application.StartupPath + "\\log.um", true))
                    {
                        sr.WriteLine(DateTime.Now.Convert2Persian() + " | " + DateTime.Now.ToString("HH:mm") + " | " + text);
                        sr.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}