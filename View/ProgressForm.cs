using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WhaleExtension.View;

namespace WhaleExtension
{
    public partial class ProgressForm : Form, IProgressView
    {
        #region Delegates
        private delegate void SetMinDelegate(Int32 Min, Int32 Max);
        private delegate void SetValueDelegate(string Message, int progress, Int16 CurrentStep, Int16 TotalStep);
        private delegate void StopDelegate();
        private delegate bool GetState();
        #endregion

        #region Variables
        public static bool status = true;
        private Thread _thread = null;
        #endregion

        #region Property
        public int Process { set => prgProcess.Value = value; }
        public string Step { set => lblStep.Text = value; }
        public string Title { set => lblTitle.Text = value; }
        #endregion

        #region Constructor
        public ProgressForm()
        {
            InitializeComponent();
            _thread = new Thread(new ThreadStart(ShowForm));
            _thread.Start();
        }
        #endregion

        #region Functions
        public void ResetProgress()
        {
            prgProcess.BeginInvoke(new Action(() => prgProcess.Value = 0));
        }

        public void SetMinMax(Int32 Min, Int32 Max)
        {
            if (InvokeRequired)
                Invoke(new SetMinDelegate(SetMinMax), Min, Max);
            else
            {
                prgProcess.Minimum = Min;
                prgProcess.Maximum = Max;
            }
        }

        public void SetValue(string Message, int progress, Int16 CurrentStep, Int16 TotalStep)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetValueDelegate(SetValue), Message, progress, CurrentStep, TotalStep);
            else
            {
                int percent = (100 * progress) / prgProcess.Maximum;
                prgProcess.Value = progress;
                lblTitle.Text = Message + " " + percent.ToString() + "%";
                lblStep.Text = CurrentStep.ToString() + "/" + TotalStep.ToString();
            }
            Thread.Sleep(50);
        }

        public void Stop()
        {
            if (InvokeRequired)
                Invoke(new StopDelegate(Stop));
            else
            {
                status = false;
                this.Close();
            }
        }

        public bool GetStatus()
        {
            if (InvokeRequired)
                Invoke(new GetState(GetStatus));

            return status;
        }

        public void ShowForm()
        {
            if (status)
                this.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            status = false;
        } 
        #endregion
    }
}
