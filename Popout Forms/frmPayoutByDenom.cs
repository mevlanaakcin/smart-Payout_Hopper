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
    public partial class frmPayoutByDenom : Form
    {
        CPayout Payout;
        CHopper Hopper;
        TextBox Output;

        TextBox[] AmountToPayout_Hopper;
        TextBox[] AmountToPayout_Payout;
        public frmPayoutByDenom(CPayout payout, CHopper hopper, TextBox output)
        {
            InitializeComponent();
            Payout = payout;
            Hopper = hopper;
            Output = output;
        }

        private void frmPayoutByDenom_Shown(object sender, EventArgs e)
        {
            // Create textbox arrays based on number of channels
            AmountToPayout_Hopper = new TextBox[Hopper.NumberOfChannels];
            AmountToPayout_Payout = new TextBox[Payout.NumberOfChannels];

            bool payoutHighest = true;
            if (Hopper.NumberOfChannels > Payout.NumberOfChannels)
                payoutHighest = false;

            System.OperatingSystem osInfo = System.Environment.OSVersion;
            int titleXSize = 0;
            // XP, 2000, Server 2003
            if (osInfo.Platform == PlatformID.Win32NT && osInfo.Version.Major == 5)
            {
                titleXSize = 80;
            }
            // Vista, 7
            else if (osInfo.Platform == PlatformID.Win32NT && osInfo.Version.Major == 6)
            {
                titleXSize = 120;
            }

            // Add description labels for Hopper
            Label l = new Label();
            l.Size = new Size(titleXSize, 45);
            l.Location = new Point(10, 10);
            l.Text = "Number to\nPayout (Hopper)";
            Controls.Add(l);

            // Iterate through the channels in the Hopper
            for (int i = 0; i < Hopper.NumberOfChannels; i++)
            {
                // Create an entry box and label for each denomination (channel)
                l = new Label();
                l.Size = new Size(80, 25);
                l.Location = new Point(10, 55 + i * 25);
                ChannelData d = Hopper.UnitDataList[i];
                l.Text = d.Value / 100f + " " + new String(d.Currency);
                Controls.Add(l);

                AmountToPayout_Hopper[i] = new TextBox();
                AmountToPayout_Hopper[i].Size = new Size(60, 25);
                AmountToPayout_Hopper[i].Location = new Point(100, 55 + i * 25);
                Controls.Add(AmountToPayout_Hopper[i]);
            }

            // Add description labels for Payout
            l = new Label();
            l.Size = new Size(titleXSize, 45);
            l.Location = new Point(200, 10);
            l.Text = "Number to\nPayout (Payout)";
            Controls.Add(l);

            // Iterate through the channels in the Payout
            for (int i = 0; i < Payout.NumberOfChannels; i++)
            {
                // Create an entry box and label for each denomination (channel)
                l = new Label();
                l.Size = new Size(80, 25);
                l.Location = new Point(200, 55 + i * 25);
                ChannelData d = Payout.UnitDataList[i];
                l.Text = d.Value / 100 + " " + new String(d.Currency);
                Controls.Add(l);

                AmountToPayout_Payout[i] = new TextBox();
                AmountToPayout_Payout[i].Size = new Size(60, 25);
                AmountToPayout_Payout[i].Location = new Point(300, 55 + i * 25);
                Controls.Add(AmountToPayout_Payout[i]);
            }

            // Add button to payout
            Button b = new Button();
            b.Size = new Size(360, 25);
            Point p;
            if (payoutHighest)
                p = AmountToPayout_Payout[Payout.NumberOfChannels-1].Location;
            else
                p = AmountToPayout_Hopper[Hopper.NumberOfChannels-1].Location;
            p.Y += 25;
            p.X = 10;
            b.Location = p;
            b.Text = "Payout";
            b.Click += new EventHandler(PayoutBtn_Click);
            Controls.Add(b);
        }

        private void PayoutBtn_Click(object sender, EventArgs e)
        {
            // First calculate Hopper
            bool payoutRequired = false;
            byte[] data = new byte[9 * Hopper.NumberOfChannels]; // create to size of maximum possible
            byte length = 0;
            int dataIndex = 0;
            byte denomsToPayout = 0;
            // For each denomination
            for (int i = 0; i < Hopper.NumberOfChannels; i++)
            {
                try
                {
                    // Check if there is input in the box
                    if (AmountToPayout_Hopper[i].Text != "" && AmountToPayout_Hopper[i].Text != "0")
                    {
                        // If textbox isn't blank then this denom is being paid out
                        denomsToPayout++;
                        length += 9; // 9 bytes per denom to payout (2 amount, 4 value, 3 currency)
                        payoutRequired = true; // need to do a payout as there is now > 0 denoms

                        // Number of this denomination to payout
                        UInt16 numToPayout = UInt16.Parse(AmountToPayout_Hopper[i].Text);
                        byte[] b = CHelpers.ConvertInt16ToBytes((short)numToPayout);
                        data[dataIndex++] = b[0];
                        data[dataIndex++] = b[1];

                        // Value of this denomination
                        ChannelData d = Hopper.UnitDataList[i];
                        b = CHelpers.ConvertInt32ToBytes(d.Value);
                        data[dataIndex++] = b[0];
                        data[dataIndex++] = b[1];
                        data[dataIndex++] = b[2];
                        data[dataIndex++] = b[3];

                        // Currency of this denomination
                        data[dataIndex++] = (Byte)d.Currency[0];
                        data[dataIndex++] = (Byte)d.Currency[1];
                        data[dataIndex++] = (Byte)d.Currency[2];
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    payoutRequired = false; // don't payout on exception
                }
            }

            if (payoutRequired)
            {
                // Send payout command
                Hopper.PayoutByDenomination(denomsToPayout, data, length, Output);
            }

            // Now calculate Payout
            payoutRequired = false;
            data = new byte[9 * Payout.NumberOfChannels]; // create to size of maximum possible
            length = 0;
            dataIndex = 0;
            denomsToPayout = 0;
            // For each denomination
            for (int i = 0; i < Payout.NumberOfChannels; i++)
            {
                try
                {
                    // Check if there is input in the box
                    if (AmountToPayout_Payout[i].Text != "" && AmountToPayout_Payout[i].Text != "0")
                    {
                        // If textbox isn't blank then this denom is being paid out
                        denomsToPayout++;
                        length += 9; // 9 bytes per denom to payout (2 amount, 4 value, 3 currency)
                        payoutRequired = true; // need to do a payout as there is now > 0 denoms

                        // Number of this denomination to payout
                        UInt16 numToPayout = UInt16.Parse(AmountToPayout_Payout[i].Text);
                        byte[] b = CHelpers.ConvertInt16ToBytes((short)numToPayout);
                        data[dataIndex++] = b[0];
                        data[dataIndex++] = b[1];

                        // Value of this denomination
                        ChannelData d = Payout.UnitDataList[i];
                        b = CHelpers.ConvertInt32ToBytes(d.Value);
                        data[dataIndex++] = b[0];
                        data[dataIndex++] = b[1];
                        data[dataIndex++] = b[2];
                        data[dataIndex++] = b[3];

                        // Currency of this denomination
                        data[dataIndex++] = (Byte)d.Currency[0];
                        data[dataIndex++] = (Byte)d.Currency[1];
                        data[dataIndex++] = (Byte)d.Currency[2];
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    payoutRequired = false; // don't payout on exception
                }
            }

            if (payoutRequired)
            {
                // Send payout command and shut this form
                Payout.PayoutByDenomination(denomsToPayout, data, length, Output);
                base.Dispose();
            }
        }
    }
}
