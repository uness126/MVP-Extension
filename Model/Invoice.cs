using System;
using System.ComponentModel.DataAnnotations;

namespace WhaleExtension.Model
{
    /// <summary>
    /// کلاس نگهداری فاکتور
    /// </summary>
    public class Invoice : IDisposable
    {
        private static int Order;
        public Invoice()
        {
            Order = 1;
        }

        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(48, ErrorMessage = "حداکثر 48 کاراکتر")]
        public string Description { get; set; }

        /// <summary>
        /// تاریخ فاکتور
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// تخفیف
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// قیمت 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// قیمت کل
        /// </summary>
        public decimal SumPrice { get; set; }

        /// <summary>
        /// مقدار
        /// </summary>
        public double Quan { get; set; }

        /// <summary>
        /// کد محصول
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LineItemOrder => Order++ * 1024;

        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string Number { get; set; }

        public void Dispose()
        {
            
        }
    }
}