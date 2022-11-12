using WhaleExtension.Class;
using WhaleExtension.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace WhaleExtension.Repositories
{
    public interface IInvoiceRepository
    {
        /// <summary>
        /// ثبت فاکتورها در مالی
        /// </summary>
        /// <param name="invoices">لیست فاکتورها</param>
        /// <param name="cToken">کنسل کردن ثبت فاکتور</param>
        /// <returns>تعداد فاکتور ثبت شده</returns>
        Task<int> SaveInvoiceAsync(List<Invoice> invoices, CancellationToken cToken);

        /// <summary>
        /// رخداد ثبت فاکتور
        /// </summary>
        event EventHandler<InvoiceEventArgs> InsertInvoice;

        /// <summary>
        /// رخداد ثبت ردیف
        /// </summary>
        event EventHandler<InvoiceEventArgs> InsertInvoiceDetail;
    }
}
