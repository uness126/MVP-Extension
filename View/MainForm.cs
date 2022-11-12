using WhaleExtension.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhaleExtension
{
    public partial class MainForm : Form, IMainView
    {
        #region Property
        OpenFileDialog openFile;

        public string Number
        {
            get => txtNumber.Text;
            set
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action(() => txtNumber.Text = value));
                else
                {
                    txtNumber.Text = value;
                }
            }
        }

        #endregion

        #region Events
        public event EventHandler SelectFileEvent;
        public event EventHandler InsertInvoiceEvent;
        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            AssociateAndRaiseViewEvents();
        }
        #endregion

        #region Functions
        private void AssociateAndRaiseViewEvents()
        {
            openFile = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                Multiselect = false
            };

            openFile.FileOk += (sender, e) =>
            {
                try
                {
                    var fileName = ((OpenFileDialog)sender).FileName;
                    txtPath.Text = fileName;
                    SelectFileEvent?.Invoke(fileName, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };

            btnBrowse.Click += delegate { openFile?.ShowDialog(this); };

            btnSave.Click += (sender, e) =>
            {
                try
                {
                    InsertInvoiceEvent?.Invoke(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
        }
        #endregion

    }
}
