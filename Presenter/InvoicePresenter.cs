using WhaleExtension.Class;
using WhaleExtension.Model;
using WhaleExtension.Repositories;
using WhaleExtension.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Security.Principal;

namespace WhaleExtension.Presenter
{
    public class InvoicePresenter
    {
        private IMainView _mainView;
        private IProgressView _progressView;
        private IInvoiceRepository _invoiceRepository;
        private readonly string _sqlConnectionString;
        private DataTable _invoiceTable;

        public InvoicePresenter(string sqlConnectionString, IMainView mainView, IInvoiceRepository invoiceRepository)
        {
            _sqlConnectionString = sqlConnectionString;

            _mainView = mainView;
            _mainView.SelectFileEvent += MainView_SelectFileEvent;
            _mainView.InsertInvoiceEvent += MainView_InsertInvoiceEvent;

            _invoiceRepository = invoiceRepository;
            _invoiceRepository.InsertInvoice += Repository_InsertInvoice;
            _invoiceRepository.InsertInvoiceDetail += Repository_InsertInvoiceDetail;
        }



        private void MainView_InsertInvoiceEvent(object sender, EventArgs e)
        {
            try
            {
                _invoiceTable.AcceptChanges();
                var invoices = _invoiceTable.AsEnumerable().Select(p => new Invoice
                {
                    Date = DateTime.Now,
                    ProductCode = p.Field<string>(2),
                    Price = Convert.ToDecimal(p.Field<string>(5)),
                    Quan = Convert.ToDouble(p.Field<string>(6)),
                    Discount = Convert.ToDecimal(p.Field<string>(7)),
                    SumPrice = Convert.ToDecimal(p.Field<string>(8)),
                    Description = string.Empty
                }).ToList();

                ProgressForm.status = true;
                _progressView = new ProgressForm();
                Thread backgroundThread1 = new Thread(
                            new ThreadStart(() =>
                            {
                                _progressView.SetMinMax(0, invoices.Count);
                                var cancellationTokenSource = new CancellationTokenSource();

                                try
                                {
                                    var task = _invoiceRepository.SaveInvoiceAsync(invoices, cancellationTokenSource.Token);

                                    while (!task.IsCompleted)
                                    {
                                        //ProgressForm.status = false;
                                        if (_progressView.GetStatus() == false)
                                        {
                                            if (!cancellationTokenSource.IsCancellationRequested)
                                                cancellationTokenSource.Cancel(true);
                                            _progressView.Stop();
                                        }
                                    }

                                    _progressView.Stop();
                                }
                                catch (OperationCanceledException ex)
                                {
                                    Logger.WriteLOG(ex, "Cancel task: MainView_InsertInvoiceEvent");
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLOG(ex, "MainView_InsertInvoiceEvent");
                                }
                                finally
                                {
                                    cancellationTokenSource.Dispose();
                                }
                            }));

                backgroundThread1.Start();

                //                 lblRes.Text = "تعداد " + count + " حساب ذخیره شد.";

                //await _invoiceRepository.SaveInvoiceAsync(inv);
            }
            catch (Exception ex)
            {
                Logger.WriteLOG(ex, "MainView_InsertInvoiceEvent");
                MessageBox.Show(ex.Message);
            }
        }

        private void Repository_InsertInvoice(object sender, InvoiceEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder(_mainView.Number);
            stringBuilder.Append(e.InvoiceNumber);
            stringBuilder.Append(", ");
            _mainView.Number = stringBuilder.ToString();
        }
        private void Repository_InsertInvoiceDetail(object sender, InvoiceEventArgs e)
        {
            _progressView.SetValue("در حال پردازش...", e.InvoiceRow, Convert.ToInt16(e.InvoiceRow), Convert.ToInt16(e.TotalRow));
        }

        private void _mainView_InsertInvoiceEvent(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MainView_SelectFileEvent(object sender, EventArgs e)
        {
            try
            {
                _invoiceTable = ExcelToDataTable.Convert(sender.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteLOG(ex, "MainView_SelectFileEvent");
                throw;
            }
        }
    }
}
