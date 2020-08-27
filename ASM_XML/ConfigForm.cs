using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASM_XML
{
    public partial class ConfigForm : Form
    {
        public List<string> konf;

        public ConfigForm(string[] сonfNames)
        {
            InitializeComponent();

            CheckBox button;

            for (int i = 1; i < сonfNames.Length + 1; i++)
            {
                button = new CheckBox();
                Height = 175;
                Ok.Top = 80;
                Controls.Add(button);
                button.Width = 200;
                button.Height = 20;
                button.Left = 20;
                button.Top = i * 10 + (i - 1) * 20;
                button.Text = сonfNames[i - 1];

                Height += 20;
                Ok.Top += 20;
            }
        }

        



        //private void Ok_Click(object sender, EventArgs e)
        //{
        //    konf = new List<string>();
        //    foreach (CheckBox but in Controls)
        //    {
        //        if (but.Checked) { konf.Add(but.Text); }
        //        Close();

        //    }
        //}
    }
}
