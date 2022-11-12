using WhaleExtension.Model;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using WhaleExtension.Class;
using System.Linq;
using System.Threading;

namespace WhaleExtension.Repositories
{
    public class InvoiceRepository : BaseRepository, IInvoiceRepository
    {
        #region Variables
        private readonly string _connectionString;
        #endregion

        #region Constructor
        public InvoiceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        #region Events
        public event EventHandler<InvoiceEventArgs> InsertInvoice;
        public event EventHandler<InvoiceEventArgs> InsertInvoiceDetail;

        public void OnInsertInvoice(InvoiceEventArgs e)
        {
            InsertInvoice?.Invoke(this, e);
        }

        public void OnInsertInvoiceDetail(InvoiceEventArgs e)
        {
            InsertInvoiceDetail?.Invoke(this, e);
        }
        #endregion

        #region Functions
        public async Task<int> SaveInvoiceAsync(List<Invoice> invoices, CancellationToken cToken)
        {
            var count = 0;
            if (invoices.Count == 0) return count;

            #region Variables
            var rowNumber = 0;
            int ShygunFactorId = 0;
            int ShygunInvNo = 1;
            int STID = Convert.ToInt32(ConfigurationManager.AppSettings["STID"]);
            bool IsError;
            var InvTyp = 2;

            SqlTransaction sqlTransaction = null;
            var sqlconnectionShygun = new SqlConnection(_connectionString);
            #endregion

            return await Task.Run(() =>
            {
                try
                {
                    sqlconnectionShygun.Open();

                    using (var invoiceRow = invoices.FirstOrDefault())
                    {
                        if (cToken.IsCancellationRequested)
                        {
                            throw new TaskCanceledException("توقف عملیات توسط کاربر");
                        }

                        sqlTransaction = sqlconnectionShygun.BeginTransaction(IsolationLevel.ReadUncommitted);
                        IsError = false;

                        try
                        {
                            #region Initialize command
                            var sqlCommandShygun = new SqlCommand()
                            {
                                Connection = sqlconnectionShygun,
                                Transaction = sqlTransaction,
                                CommandType = CommandType.Text
                            };

                            //ایدی حساب شایگان
                            sqlCommandShygun.CommandText = string.Format("SELECT [AccountId] FROM [dbo].[AC_0101_N] WHERE [AccountNumber] = '{0}'", ConfigurationManager.AppSettings["AccountCode"]);
                            Int32 AccountId = Convert.ToInt32(sqlCommandShygun.ExecuteScalar());

                            sqlCommandShygun.CommandText = string.Format("SELECT JobId FROM [dbo].[AC_0602_N] WHERE [Guid] = '{0}'", (string.IsNullOrEmpty(ConfigurationManager.AppSettings["JobCode"]) ? Guid.Empty.ToString() :
                                ConfigurationManager.AppSettings["JobCode"]));
                            sqlCommandShygun.CommandType = CommandType.Text;
                            Int32? JobCostingId = null;
                            var obj = sqlCommandShygun.ExecuteScalar();
                            if (obj != null)
                                JobCostingId = Convert.ToInt32(obj);

                            //شماره فاکتور سریع شایگان
                            sqlCommandShygun.CommandText = String.Format("SELECT MAX([InvNo]) AS InvNo FROM [dbo].[AC_4101_F] WHERE InvTyp = {0}", InvTyp);

                            SqlParameter InvNo = new SqlParameter("@InvNo", SqlDbType.Int) { Direction = ParameterDirection.Output };

                            var tempNo = sqlCommandShygun.ExecuteScalar();
                            if (tempNo != null && tempNo.ToString() != string.Empty)
                                InvNo.Value = Convert.ToInt32(tempNo);

                            invoiceRow.Number = InvNo.Value.ToString();

                            //ایدی یوزر شایگان
                            sqlCommandShygun.CommandText = String.Format("SELECT Userid FROM [ct" + ConfigurationManager.AppSettings["InitialCatalog"] + "].dbo.SCU01_N WHERE [Userid]='{0}'", 1);
                            Int32 Userid = 1;

                            var tempUserid = sqlCommandShygun.ExecuteScalar();
                            if (tempUserid != null && tempUserid.ToString() != string.Empty)
                                Userid = Convert.ToInt32(tempUserid);
                            #endregion

                            //Inser To 4101_F Header
                            #region Inser To 4101_F Header
                            sqlCommandShygun.Parameters.Clear();
                            sqlCommandShygun.Transaction = sqlTransaction;
                            sqlCommandShygun.CommandText = "CAS_AC_4101_F_Insert";
                            sqlCommandShygun.CommandType = CommandType.StoredProcedure;

                            //sqlCommandShygun = new SqlCommand()
                            //{
                            //    Transaction = sqlTransaction,
                            //    CommandText = "CAS_AC_4101_F_Insert",

                            //};

                            SqlParameter InvHeaderId = new SqlParameter("@InvHeaderId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            sqlCommandShygun.Parameters.Add(InvHeaderId);
                            sqlCommandShygun.Parameters.AddWithValue("@InvTyp", InvTyp);
                            sqlCommandShygun.Parameters.Add(InvNo);
                            sqlCommandShygun.Parameters.AddWithValue("@InvDescription", invoiceRow.Description.Length > 47 ? invoiceRow.Description.Substring(0, 48) : invoiceRow.Description.Substring(0, invoiceRow.Description.Length));

                            if (System.Globalization.CultureInfo.CurrentCulture.Name == "fa-IR")
                            {
                                sqlCommandShygun.Parameters.AddWithValue("@InvDate", Convert.ToDateTime(invoiceRow.Date.PersianCalendarToDateTime()));
                            }
                            else
                                sqlCommandShygun.Parameters.AddWithValue("@InvDate", Convert.ToDateTime(Common.ToEnglishNumber(invoiceRow.Date.ToString())).ToString("d"));

                            sqlCommandShygun.Parameters.AddWithValue("@InvCon", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@InvPayDue", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@InvPayCon", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@GeneralRef", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@PInvoiceRef", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@OrderRef", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@RegisterRef", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@CurrencyId", 1);
                            sqlCommandShygun.Parameters.AddWithValue("@Rate", 1);
                            sqlCommandShygun.Parameters.AddWithValue("@AccountId", AccountId);
                            sqlCommandShygun.Parameters.AddWithValue("@JobCostingId", (object)JobCostingId ?? DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@SCode", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@JobSCode", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@SCode2", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@JobSCode2", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@Comm", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@Comm2", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@DiscPer", 0);
                            //sqlCommandShygun.Parameters.AddWithValue("@DiscPer", Convert.ToDouble(invoiceRow["INV_TOTAL_DISCOUNT"]));
                            sqlCommandShygun.Parameters.AddWithValue("@DiscRial", Convert.ToDouble(invoices.Sum(i => i.Discount)));
                            sqlCommandShygun.Parameters.AddWithValue("@DiscAmount", Convert.ToDouble(invoices.Sum(i => i.Discount)));
                            sqlCommandShygun.Parameters.AddWithValue("@tick", SqlDbType.Bit).Value = false;
                            sqlCommandShygun.Parameters.AddWithValue("@STID1", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@STID2", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@RefTransfer", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@UpPriceFlag", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@DepartmentId", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@firstissuerid", Userid);
                            sqlCommandShygun.Parameters.AddWithValue("@lastissuerid", Userid);
                            sqlCommandShygun.Parameters.AddWithValue("@printed", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@bijak", DBNull.Value);
                            sqlCommandShygun.Parameters.AddWithValue("@ExpFixAmount", DBNull.Value);
                            //OutPut
                            //sqlCommandShygun.Parameters["@InvHeaderId"].Direction = ParameterDirection.Output;
                            sqlCommandShygun.ExecuteNonQuery();
                            ShygunFactorId = Convert.ToInt32(InvHeaderId.Value.ToString());
                            ShygunInvNo = Convert.ToInt32(InvNo.Value);
                            #endregion

                            foreach (var detail in invoices)
                            {
                                if (cToken.IsCancellationRequested)
                                {
                                    throw new TaskCanceledException("توقف عملیات توسط کاربر");
                                }

                                #region Get Id Product
                                sqlCommandShygun.Transaction = sqlTransaction;
                                sqlCommandShygun.CommandType = CommandType.Text;
                                sqlCommandShygun.CommandText =
                                    string.Format("SELECT ItemId FROM [dbo].[AC_3001_N] WHERE ItemCode = '{0}'",
                                        detail.ProductCode);
                                Int32 ItemId = Convert.ToInt32(sqlCommandShygun.ExecuteScalar());
                                #endregion

                                //Inser To 4001_F Detail
                                #region Insert To 4001_F Detail

                                SqlParameter LineItemId =
                                    new SqlParameter("@LineItemId", SqlDbType.Int)
                                    {
                                        Direction = ParameterDirection.Output
                                    };
                                sqlCommandShygun.Transaction = sqlTransaction;
                                sqlCommandShygun.CommandText = "CAS_AC_4001_F_Insert";
                                sqlCommandShygun.CommandType = CommandType.StoredProcedure;
                                sqlCommandShygun.Parameters.Clear();
                                sqlCommandShygun.Parameters.Add(LineItemId);
                                sqlCommandShygun.Parameters.AddWithValue("@InvHeaderId", ShygunFactorId);
                                sqlCommandShygun.Parameters.AddWithValue("@LineItemOrder", Convert.ToInt32(detail.LineItemOrder));
                                sqlCommandShygun.Parameters.AddWithValue("@CardItemType", 0);
                                sqlCommandShygun.Parameters.AddWithValue("@RefProduct", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@RefTransfer", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@ItemDefId", DBNull.Value);
                                if (STID == 0)
                                    sqlCommandShygun.Parameters.AddWithValue("@STID", DBNull.Value);
                                else
                                    sqlCommandShygun.Parameters.AddWithValue("@STID", STID);
                                sqlCommandShygun.Parameters.AddWithValue("@ItemId", ItemId);
                                sqlCommandShygun.Parameters.AddWithValue("@Quan", detail.Quan);
                                sqlCommandShygun.Parameters.AddWithValue("@Quan2", 0);
                                sqlCommandShygun.Parameters.AddWithValue("@Price", Convert.ToDouble(detail.Price));
                                sqlCommandShygun.Parameters.AddWithValue("@Price2", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@Rial", Convert.ToDouble(detail.Price));
                                sqlCommandShygun.Parameters.AddWithValue("@ShekanPr", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@difShekanPr", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@InvoiceRowTypeID", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@SegmentRial", Convert.ToDouble(detail.Price));
                                sqlCommandShygun.Parameters.AddWithValue("@CurRial", Convert.ToDouble(detail.Price));
                                sqlCommandShygun.Parameters.AddWithValue("@LineDiscPer", Convert.ToDouble(detail.Discount));
                                sqlCommandShygun.Parameters.AddWithValue("@LineDiscAmount", Convert.ToDouble(detail.Discount));
                                sqlCommandShygun.Parameters.AddWithValue("@LineSalePer", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@LineSalePer2", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@LineSaleAmount", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@LineSaleAmount2", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@CostCenterId", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@LineItemDesc", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@PriceSegmentId", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@TransType", SqlDbType.Bit).Value = false;
                                sqlCommandShygun.Parameters.AddWithValue("@ReturnRial", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@OrderRemQ", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@OrderRemQ2", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@up_cross_boun_type", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@up_cross_boun_id", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@finalOrderQ", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@finalOrderQ2", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@relatedlineitemid", DBNull.Value);
                                sqlCommandShygun.Parameters.AddWithValue("@InvBodyBarcode", DBNull.Value);
                                //OutPut
                                sqlCommandShygun.ExecuteNonQuery();
                                int tempItem = Convert.ToInt32(LineItemId.Value.ToString());

                                count = count + 1;
                                #endregion

                                rowNumber++;
                                OnInsertInvoiceDetail(new InvoiceEventArgs(invoiceRow.Number, rowNumber, invoices.Count));
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLOG(ex);
                            IsError = true;
                        }
                        if (!IsError)
                            sqlTransaction.Commit();

                        OnInsertInvoice(new InvoiceEventArgs(invoiceRow.Number, rowNumber, invoices.Count));
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    Logger.WriteLOG(ex, "SaveInvoiceAsync");

                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                    return count;
                }
                finally
                {
                    sqlconnectionShygun.Close();
                }
            }, cToken);
        }
        #endregion
    }
}
