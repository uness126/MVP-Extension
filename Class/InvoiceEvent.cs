using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhaleExtension.Class
{
    /// <summary>
    /// کلاس ذخیره رخداد ثبت فاکتور
    /// </summary>
    public class InvoiceEventArgs : EventArgs
    {
        /// <summary>
        /// شماره فاکتور ثبت شده
        /// </summary>
        public string InvoiceNumber { get; private set; }

        /// <summary>
        /// شماره ردیف
        /// </summary>
        public int InvoiceRow { get; private set; }

        /// <summary>
        /// کل ردیف
        /// </summary>
        public int TotalRow { get; private set; }

        public InvoiceEventArgs(string incNumber, int invoiceRow,int totalRow)
        {
            InvoiceNumber = incNumber;
            InvoiceRow = invoiceRow;
            TotalRow = totalRow;
        }
    }
}
