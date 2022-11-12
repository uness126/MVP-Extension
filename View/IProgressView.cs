using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhaleExtension.View
{
    public interface IProgressView
    {
        /// <summary>
        /// درصد پیشرفت
        /// </summary>
        int Process { set; }

        /// <summary>
        /// مرحله جاری
        /// </summary>
        string Step { set; }

        /// <summary>
        /// عنوان کار
        /// </summary>
        string Title { set; }

        /// <summary>
        /// آپدیت وضعیت پروسس
        /// </summary>
        /// <param name="Message">نمایش متن عنوان کار</param>
        /// <param name="progress">درصد پیشرفت</param>
        /// <param name="CurrentStep">مرحله جاری</param>
        /// <param name="TotalStep">کل مراحل</param>
        void SetValue(string Message, int progress, Int16 CurrentStep, Int16 TotalStep);

        /// <summary>
        /// ریست کردن پروسس
        /// </summary>
        void ResetProgress();

        /// <summary>
        /// تنظیم مین و ماکس پروسس
        /// </summary>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        void SetMinMax(Int32 Min, Int32 Max);

        /// <summary>
        /// توقف پروسس
        /// </summary>
        void Stop();

        /// <summary>
        /// دریافت وضعیت پروسس
        /// </summary>
        /// <returns></returns>
        bool GetStatus();

        void ShowForm();
    }
}
