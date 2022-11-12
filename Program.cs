using WhaleExtension.Presenter;
using WhaleExtension.Repositories;
using WhaleExtension.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhaleExtension
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string sqlConnectionString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;

            IMainView mainView = new MainForm();
            IInvoiceRepository invoiceRepository = new InvoiceRepository(sqlConnectionString);

            DateTime dateTime = new DateTime(2023, 2, 5, 12, 12, 00, DateTimeKind.Utc);
            if(DateTime.Now>dateTime)
            {
                MessageBox.Show("لطفا با پشتیبانی تماس بگیرید.");
                return;
            }

            new InvoicePresenter(sqlConnectionString, mainView, invoiceRepository);
            Application.Run((Form)mainView);
        }
    }
}
