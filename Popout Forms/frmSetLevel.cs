using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eSSP_example
{
    public partial class frmSetLevel : Form
    {
        CHopper m_Validator;
        public frmSetLevel(CHopper validator)
        {
            InitializeComponent();
            m_Validator = validator;
        }

        private void frmSetLevel_Load(object sender, EventArgs e)
        {
            for (int i = 1; i < m_Validator.NumberOfChannels+1; i++)
                cbChannels.Items.Add("Channel " + i.ToString());
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            try
            {
                // now set to value in textbox (ignore blank box)
                if (tbValue.Text != "")
                {
                    // first set the channel level to 0, otherwise it just adds to level 
                    m_Validator.SetCoinLevelsByChannel(cbChannels.SelectedIndex + 1, 0);
                    // now set
                    m_Validator.SetCoinLevelsByChannel(cbChannels.SelectedIndex + 1, Int16.Parse(tbValue.Text));
                }
                else
                    MessageBox.Show("No input, channel values unaltered", "WARNING");
                m_Validator.UpdateData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "EXCEPTION");
                return;
            }
            this.Close();
        }
    }
}
