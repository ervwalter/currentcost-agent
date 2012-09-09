using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurrentCostAgent
{
    public partial class StatusForm : Form
    {
        private int _pending = 0;
        private SortedDictionary<int, string> _readings = new SortedDictionary<int, string>();

        public StatusForm()
        {
            InitializeComponent();
        }

        public void NewReading(SensorReading reading)
        {
            _pending++;
            labelPendingUpload.Text = string.Format("{0} readings waiting to be uploaded", _pending);
            string text = reading.Watts.ToString("#,0");
            if (reading.Sensor == 0)
            {
                labelOverall.Text = text;
            }
            else
            {
                _readings[reading.Sensor] = text;
                listViewAppliances.Items.Clear();
                foreach (var key in _readings.Keys)
                {
                    listViewAppliances.Items.Add(new ListViewItem(new string[] { key.ToString(), _readings[key] }));
                }
            }
        }

        public void NotifyUpload()
        {
            _pending = 0;
            labelPendingUpload.Text = "No uploads pending";
            labelLastUpload.Text = "Last upload: " + DateTime.Now.ToString();
        }

        private void StatusForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
