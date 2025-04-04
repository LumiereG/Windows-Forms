using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab4PwSG
{
    public partial class Form2 : Form
    {
        public DateTime SelectedDate => dateTimePicker1.Value;
        public string TaskDescription => textBox1.Text;
        public Form2(DateTime defaultDate)
        {
            InitializeComponent();
            dateTimePicker1.Value = defaultDate;
        }

    }
}
