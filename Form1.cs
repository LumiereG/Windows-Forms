using System.Diagnostics;
using System.Globalization;

namespace lab4PwSG
{
    public partial class Form1 : Form
    {
        private string file;
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += OnFormClosing;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Unsaved changes will be lost.\nAre you sure you want to close the app?",
                                         "Exit", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
                e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var addTaskDialog = new Form2(monthCalendar1.SelectionStart);
            if (addTaskDialog.ShowDialog() == DialogResult.OK)
            {
                string date = addTaskDialog.SelectedDate.ToShortDateString();
                string note = addTaskDialog.TaskDescription;
                ListViewItem item = new ListViewItem(new[] { note, date });
                listView1.Items.Add(item);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
                listView1.Items.Remove(item);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string lastLoadedFile = openFileDialog.FileName;
                    listView1.Items.Clear();
                    foreach (var line in File.ReadLines(lastLoadedFile))
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            var item = new ListViewItem("") { Checked = bool.Parse(parts[0]) };
                            item.SubItems.Add(parts[1]);
                            item.SubItems.Add(parts[2]);
                            listView1.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                saveFileDialog.Title = "Save Task List";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file = saveFileDialog.FileName;
                    if (string.IsNullOrEmpty(file)) return;
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        foreach (ListViewItem item in listView1.Items)
                        {
                            writer.WriteLine($"{item.Checked},{item.SubItems[0].Text},{item.SubItems[1].Text}");
                        }
                    }
                }
            }
            MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
