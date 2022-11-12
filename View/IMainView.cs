using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WhaleExtension.View
{
    public interface IMainView
    {
        string Number { get; set; }
        /// <summary>
        /// رخداد انتخاب فایل
        /// </summary>
        event EventHandler SelectFileEvent;

        /// <summary>
        /// رخداد ثبت فاکتور
        /// </summary>
        event EventHandler InsertInvoiceEvent;
    }
}
