using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Diagnostics;
using System.Threading;
using ITLlib;

namespace eSSP_example
{
    public class CPayout
    {
        // ssp library variables

        SSP_COMMAND m_cmd;
        SSP_KEYS keys;
        SSP_FULL_KEY sspKey;
        SSP_COMMAND_INFO info;

        // variable declarations

        // The logging class
        //CCommsWindow m_Comms;

        // The protocol version this hopper is using, set in setup request
        int m_ProtocolVersion;

        // The number of channels used in this validator
        int m_NumberOfChannels;

        // The type of unit this class represents, set in the setup request
        char m_UnitType;

        // The multiplier by which the channel values are multiplied to get their
        // true penny value.
        int m_ValueMultiplier;

        //Integer to hold total number of Hold messages to be issued before releasing note from escrow
        int m_HoldNumber;

        //Integer to hold number of hold messages still to be issued
        int m_HoldCount;

        //Bool to hold flag set to true if a note is being held in escrow
        bool m_NoteHeld;


        int tutar = 0;
        // A list of dataset data, sorted by value. Holds the info on channel number, value, currency,
        // level and whether it is being recycled.
        List<ChannelData> m_UnitDataList;

        // constructor
        public CPayout()
        {
            m_cmd = new SSP_COMMAND();
            keys = new SSP_KEYS();
            sspKey = new SSP_FULL_KEY();
            info = new SSP_COMMAND_INFO();

           // m_Comms = new CCommsWindow("SMARTPayout");
           // m_Comms.Text = "SMART Payout Comms";
            m_NumberOfChannels = 0;
            m_ValueMultiplier = 1;
            m_UnitDataList = new List<ChannelData>();
            m_HoldCount = 0;
            m_HoldNumber = 0;
        }

        /* Variable Access */

        // access to ssp vars
        public SSP_COMMAND CommandStructure
        {
            get { return m_cmd; }
            set { m_cmd = value; }
        }

        public SSP_COMMAND_INFO InfoStructure
        {
            get { return info; }
            set { info = value; }
        }

        // access to the comms log
       /* public CCommsWindow CommsLog
        {
            get { return m_Comms; }
            set { m_Comms = value; }
        }*/

        // access to number of channels
        public int NumberOfChannels
        {
            get { return m_NumberOfChannels; }
            set { m_NumberOfChannels = value; }
        }

        // access to value multiplier
        public int Multiplier
        {
            get { return m_ValueMultiplier; }
            set { m_ValueMultiplier = value; }
        }
        // acccess to hold number
        public int HoldNumber
        {
            get { return m_HoldNumber; }
            set { m_HoldNumber = value; }

        }
        //Access to flag showing note is held in escrow
        public bool NoteHeld
        {
            get { return m_NoteHeld; }
        }
        // access to sorted list of hash entries
        public List<ChannelData> UnitDataList
        {
            get { return m_UnitDataList; }
        }

        // access to the type of unit
        public char UnitType
        {
            get { return m_UnitType; }
        }

        /* Command functions */

        // The enable command enables the validator, allowing it to receive and act on commands.
        public void EnableValidator(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;
            // check response
            if (log != null)
                log.AppendText("Payout Etkin\r\n");
        }

        // Disable command stops the validator from acting on commands sent to it.
        public void DisableValidator(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_DISABLE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;
            // check response
            if (log != null)
                log.AppendText("Payout Etkin\r\n");
        }
        // Return Note command returns note held in escrow to bezel. 
        public void ReturnNote(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_REJECT_BANKNOTE;
            m_cmd.CommandDataLength = 1;
            if (!SendCommand(log)) return;

            if (CheckGenericResponses(log))
            {
                if (log != null)
                {
                    log.AppendText("Dönüş notu\r\n");
                }
                m_HoldCount = 0;
            }
        }
        // Enable payout allows the validator to payout and store notes.
        public void EnablePayout(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE_PAYOUT_DEVICE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            if (log != null)
                log.AppendText("Ödeme etkinleştirildi\r\n");
        }

        // Disable payout stops the validator being able to store/payout notes.
        public void DisablePayout(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_DISABLE_PAYOUT_DEVICE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;
            if (log != null)
                log.AppendText("Ödeme devre dışı\r\n");
        }

        // Empty payout device takes all the notes stored and moves them to the cashbox.
        public void EmptyPayoutDevice(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_EMPTY_ALL;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            if (log != null)
                log.AppendText("Payout kasaya boşalt\r\n");
        }

        // This function uses the command GET NOTE AMOUNT to find out the number of
        // a specified type of note stored in the payout. Returns the number of notes stored
        // of that denomination.
        public int CheckNoteLevel(int note, char[] currency, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_DENOMINATION_LEVEL;
            byte[] b = CHelpers.ConvertInt32ToBytes(note);
            m_cmd.CommandData[1] = b[0];
            m_cmd.CommandData[2] = b[1];
            m_cmd.CommandData[3] = b[2];
            m_cmd.CommandData[4] = b[3];

            m_cmd.CommandData[5] = (byte)currency[0];
            m_cmd.CommandData[6] = (byte)currency[1];
            m_cmd.CommandData[7] = (byte)currency[2];
            m_cmd.CommandDataLength = 8;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return 0;

            int i = (int)m_cmd.ResponseData[1];
            return i;
        }

        // The set routing command changes the way the validator deals with a note, either it can send the note straight to the cashbox
        // or it can store the note for payout. This is specified in the second byte (0x00 to store for payout, 0x01 for cashbox). The 
        // bytes after this represent the 4 bit value of the note.
        // This function allows the note to be specified as an int in the param note, the stack bool is true for cashbox, false for storage.
        public void ChangeNoteRoute(int note, char[] currency, bool stack, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_DENOMINATION_ROUTE;

            // if this note is being changed to stack (cashbox)
            if (stack)
                m_cmd.CommandData[1] = 0x01;
            // note being stored (payout)
            else
                m_cmd.CommandData[1] = 0x00;

            // get the note as a byte array
            byte[] b = BitConverter.GetBytes(note);
            m_cmd.CommandData[2] = b[0];
            m_cmd.CommandData[3] = b[1];
            m_cmd.CommandData[4] = b[2];
            m_cmd.CommandData[5] = b[3];

            // send country code (protocol 6+)
            m_cmd.CommandData[6] = (byte)currency[0];
            m_cmd.CommandData[7] = (byte)currency[1];
            m_cmd.CommandData[8] = (byte)currency[2];

            m_cmd.CommandDataLength = 9;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            if (log != null)
            {
                string s = new string(currency);
                if (stack)
                    s += " to cashbox)\r\n";
                else
                    s += " to storage)\r\n";

                log.AppendText("Banknot yönlendirme başarılı (" + CHelpers.FormatToCurrency(note) + " " + s);
            }
        }

        // The reset command instructs the validator to restart (same effect as switching on and off)
        public void Reset(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_RESET;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;
        }

        // This just sends a sync command to the validator.
        public bool SendSync(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SYNC;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return false;
            if (log != null) log.AppendText("Senkronizasyon gönderildi\r\n");
            return true;
        }

        // This function sets the protocol version in the validator to the version passed across. Whoever calls
        // this needs to check the response to make sure the version is supported.
        public void SetProtocolVersion(byte pVersion, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_HOST_PROTOCOL_VERSION;
            m_cmd.CommandData[1] = pVersion;
            m_cmd.CommandDataLength = 2;
            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;
        }

        // This function calls the PAYOUT AMOUNT command to payout a specified value. This can be sent
        // with the option byte 0x19 to test whether the payout is possible or 0x58 to actually do the payout.
        public void PayoutAmount(int amount, char[] currency, bool test = false, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_PAYOUT_AMOUNT;
            byte[] b = CHelpers.ConvertInt32ToBytes(amount);
            m_cmd.CommandData[1] = b[0];
            m_cmd.CommandData[2] = b[1];
            m_cmd.CommandData[3] = b[2];
            m_cmd.CommandData[4] = b[3];

            m_cmd.CommandData[5] = (byte)currency[0];
            m_cmd.CommandData[6] = (byte)currency[1];
            m_cmd.CommandData[7] = (byte)currency[2];

            if (!test)
                m_cmd.CommandData[8] = 0x58; // real payout
            else
                m_cmd.CommandData[8] = 0x19; // test payout

            m_cmd.CommandDataLength = 9;
            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            if (log != null)
            {
                log.AppendText("Ödeme " + CHelpers.FormatToCurrency(amount) + " " +
                    new string(currency) + "\r\n");
            }
        }

        // Payout by denomination. This function allows a developer to payout specified amounts of certain
        // notes. Due to the variable length of the data that could be passed to the function, the user 
        // passes an array containing the data to payout and the length of that array along with the number
        // of denominations they are paying out.
        public void PayoutByDenomination(byte numDenoms, byte[] data, byte dataLength, TextBox log = null)
        {
            // First is the command byte
            m_cmd.CommandData[0] = CCommands.SSP_CMD_PAYOUT_BY_DENOMINATION;

            // Next is the number of denominations to be paid out
            m_cmd.CommandData[1] = numDenoms;

            // Copy over data byte array parameter into command structure
            int currentIndex = 2;
            for (int i = 0; i < dataLength; i++)
                m_cmd.CommandData[currentIndex++] = data[i];

            // Perform a real payout (0x19 for test)
            m_cmd.CommandData[currentIndex++] = 0x58;

            // Length of command data (add 3 to cover the command byte, num of denoms and real/test byte)
            dataLength += 3;
            m_cmd.CommandDataLength = dataLength;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            if (log != null)
                log.AppendText("Belirlenen miktarda ödeme . . .\r\n");
        }

        // This function performs a number of commands in order to setup the encryption between the host and the validator.
        public bool NegotiateKeys(TextBox log = null)
        {
            // make sure encryption is off
            m_cmd.EncryptionStatus = false;

            // send sync
            if (log != null) log.AppendText("Senkronizasyon... ");
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SYNC;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return false;
            if (log != null) log.AppendText("Başarılı\r\n");

            LibraryHandler.InitiateKeys(ref keys, ref m_cmd);

            // send generator
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_GENERATOR;
            m_cmd.CommandDataLength = 9;
            if (log != null) log.AppendText("Ayar jeneratörü... ");

            // Convert generator to bytes and add to command data.
            BitConverter.GetBytes(keys.Generator).CopyTo(m_cmd.CommandData, 1);

            if (!SendCommand(log)) return false;
            if (log != null) log.AppendText("Başarılı\r\n");

            // send modulus
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_MODULUS;
            m_cmd.CommandDataLength = 9;
            if (log != null) log.AppendText("Modül gönderme... ");

            // Convert modulus to bytes and add to command data.
            BitConverter.GetBytes(keys.Modulus).CopyTo(m_cmd.CommandData, 1);

            if (!SendCommand(log)) return false;
            if (log != null) log.AppendText("Başarılı\r\n");

            // send key exchange
            m_cmd.CommandData[0] = CCommands.SSP_CMD_REQUEST_KEY_EXCHANGE;
            m_cmd.CommandDataLength = 9;
            if (log != null) log.AppendText("Anahtar değişimi... ");

            // Convert host intermediate key to bytes and add to command data.
            BitConverter.GetBytes(keys.HostInter).CopyTo(m_cmd.CommandData, 1);


            if (!SendCommand(log)) return false;
            if (log != null) log.AppendText("Başarılı\r\n");

            // Read slave intermediate key.
            keys.SlaveInterKey = BitConverter.ToUInt64(m_cmd.ResponseData, 1);

            LibraryHandler.CreateFullKey(ref keys);

            // get full encryption key
            m_cmd.Key.FixedKey = 0x0123456701234567;
            m_cmd.Key.VariableKey = keys.KeyHost;

            if (log != null) log.AppendText("Anahtarlar başarıyla müzakere edildi\r\n");

            return true;
        }

        // This function uses the setup request command to get all the information about the validator.
        public void PayoutSetupRequest(TextBox log = null)
        {
            StringBuilder sbDisplay = new StringBuilder(1000);

            // send setup request
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SETUP_REQUEST;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;

            // display setup request


            // unit type
            int index = 1;
            sbDisplay.Append("Birim Tipi: ");
            m_UnitType = (char)m_cmd.ResponseData[index++];
            switch (m_UnitType)
            {
                case (char)0x00: sbDisplay.Append("Validator"); break;
                case (char)0x03: sbDisplay.Append("SMART Hopper"); break;
                case (char)0x06: sbDisplay.Append("SMART Payout"); break;
                case (char)0x07: sbDisplay.Append("NV11"); break;
                default: sbDisplay.Append("Unknown Type"); break;
            }

            // firmware
            sbDisplay.AppendLine();
            sbDisplay.Append("Firmware: ");
            while (index <= 5)
            {
                sbDisplay.Append((char)m_cmd.ResponseData[index++]);
                if (index == 4)
                    sbDisplay.Append(".");
            }
            sbDisplay.AppendLine();
            // country code.
            // legacy code so skip it.
            index += 3;

            // value multiplier.
            // legacy code so skip it.
            index += 3;

            // Number of channels
            sbDisplay.AppendLine();
            sbDisplay.Append("Number of Channels: ");
            m_NumberOfChannels = m_cmd.ResponseData[index++];
            sbDisplay.Append(m_NumberOfChannels);
            sbDisplay.AppendLine();

            // channel values.
            // legacy code so skip it.
            index += m_NumberOfChannels; // Skip channel values

            // channel security
            // legacy code so skip it.
            index += m_NumberOfChannels;

            // real value multiplier
            // (big endian)
            sbDisplay.Append("Real Value Multiplier: ");
            m_ValueMultiplier = m_cmd.ResponseData[index + 2];
            m_ValueMultiplier += m_cmd.ResponseData[index + 1] << 8;
            m_ValueMultiplier += m_cmd.ResponseData[index] << 16;
            sbDisplay.Append(m_ValueMultiplier);
            sbDisplay.AppendLine();
            index += 3;


            // protocol version
            sbDisplay.Append("Protocol Version: ");
            m_ProtocolVersion = m_cmd.ResponseData[index++];
            sbDisplay.Append(m_ProtocolVersion);
            sbDisplay.AppendLine();

            // Add channel data to list then display.
            // Clear list.
            m_UnitDataList.Clear();
            // Loop through all channels.

            for (byte i = 0; i < m_NumberOfChannels; i++)
            {
                ChannelData loopChannelData = new ChannelData();
                // Channel number.
                loopChannelData.Channel = (byte)(i + 1);
                    
                // Channel value.
                loopChannelData.Value = BitConverter.ToInt32(m_cmd.ResponseData, index + (m_NumberOfChannels * 3) + (i * 4)) * m_ValueMultiplier;

                // Channel Currency
                loopChannelData.Currency[0] = (char)m_cmd.ResponseData[index + (i * 3)];
                loopChannelData.Currency[1] = (char)m_cmd.ResponseData[(index + 1) + (i * 3)];
                loopChannelData.Currency[2] = (char)m_cmd.ResponseData[(index + 2) + (i * 3)];

                // Channel level.
                loopChannelData.Level = CheckNoteLevel(loopChannelData.Value, loopChannelData.Currency, log);

                // Channel recycling
                bool recycling = false;
                IsNoteRecycling(loopChannelData.Value, loopChannelData.Currency, ref recycling, log);
                loopChannelData.Recycling = recycling;

                // Add data to list.
                m_UnitDataList.Add(loopChannelData);

                //Display data
                sbDisplay.Append("Channel ");
                sbDisplay.Append(loopChannelData.Channel);
                sbDisplay.Append(": ");
                sbDisplay.Append(loopChannelData.Value / m_ValueMultiplier);
                sbDisplay.Append(" ");
                sbDisplay.Append(loopChannelData.Currency);
                sbDisplay.AppendLine();
            }

            // Sort the list by .Value.
            m_UnitDataList.Sort((d1, d2) => d1.Value.CompareTo(d2.Value));

            if (log != null)
                log.AppendText(sbDisplay.ToString());
        }

        // Kağıt para kanal izinlerini hex olarak ayarla
        /*public byte izinhex_code()
        {
            Properties.Settings.Default.paperWay[0] = "1";
            Properties.Settings.Default.paperWay[1] = "1";
            Properties.Settings.Default.paperWay[2] = "1";
            Properties.Settings.Default.paperWay[3] = "1";
            Properties.Settings.Default.paperWay[4] = "1";
            Properties.Settings.Default.paperWay[5] = "0";

            string bin = "00";


            for (int j = 5; j >= 0; j--)
            {
                bin += Properties.Settings.Default.paperWay[j];
            }

            int numOfbyte = bin.Length / 8;
            byte[] bytes = new byte[numOfbyte];

            for (int i = 0; i < numOfbyte; i++)
            {
                bytes[i] = Convert.ToByte(bin.Substring(8 * i, 8), 2);
            }

            MessageBox.Show(bytes.GetType().ToString());
            MessageBox.Show(BitConverter.ToString(bytes));
            return bytes[0];
        }*/
        // Kağıt para kanal izinlerini hex olarak ayarlayıp para engelleme işini yaptık
        public string izinhex_code()
        {
           /* Properties.Settings.Default.paperWay[0] = "0"; // 5 tl
            Properties.Settings.Default.paperWay[1] = "1"; // 10 tl
            Properties.Settings.Default.paperWay[2] = "0"; // 20 tl
            Properties.Settings.Default.paperWay[3] = "0"; // 50 tl
            Properties.Settings.Default.paperWay[4] = "0"; // 100 tl
            Properties.Settings.Default.paperWay[5] = "0"; // 200 tl*/

            string bin = "00";


            for (int j = 5; j >= 0; j--)
            {
                bin += Properties.Settings.Default.paperWay[j];
            }

            int numOfbyte = bin.Length / 8;
            byte[] bytes = new byte[numOfbyte];

            for (int i = 0; i < numOfbyte; i++)
            {
                bytes[i] = Convert.ToByte(bin.Substring(8 * i, 8), 2);
            }

            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            

            //MessageBox.Show(hex.ToString());


            return hex.ToString();
        }

        // This function sends the set inhibits command to set the inhibits on the validator. An additional two
        // bytes are sent along with the command byte to indicate the status of the inhibits on the channels.
        // For example 0xFF and 0xFF in binary is 11111111 11111111. This indicates all 16 channels supported by
        // the validator are uninhibited. If a user wants to inhibit channels 8-16, they would send 0x00 and 0xFF.
        public void SetInhibits(TextBox log = null)
        {
            // set inhibits
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SET_CHANNEL_INHIBITS;
            m_cmd.CommandData[1] = Convert.ToByte(izinhex_code(),16);
            //m_cmd.CommandData[1] = 0x1F; // 1--8 arası kanallar
            m_cmd.CommandData[2] = 0x01; //8--16 arası kanallar && 0x01 değeri ile sadece 16 ıncı kanal açık
            m_cmd.CommandDataLength = 3;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            if (log != null)
                log.AppendText("Seti engeller\r\n");
        }

        

        // This function uses the GET ROUTING command to see if a specified note is recycling. The
        // caller passes a bool across which is set by the function.
        public void IsNoteRecycling(int noteValue, char[] currency, ref bool response, TextBox log = null)
        {
            // Determine if the note is currently being recycled
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_DENOMINATION_ROUTE;
            byte[] b = CHelpers.ConvertInt32ToBytes(noteValue);
            m_cmd.CommandData[1] = b[0];
            m_cmd.CommandData[2] = b[1];
            m_cmd.CommandData[3] = b[2];
            m_cmd.CommandData[4] = b[3];

            // Add currency
            m_cmd.CommandData[5] = (byte)currency[0];
            m_cmd.CommandData[6] = (byte)currency[1];
            m_cmd.CommandData[7] = (byte)currency[2];
            m_cmd.CommandDataLength = 8;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            // True if it is currently being recycled
            if (m_cmd.ResponseData[1] == 0x00)
                response = true;
            // False if not
            else if (m_cmd.ResponseData[1] == 0x01)
                response = false;
        }

        // This function uses the FLOAT AMOUNT command to set the float amount. The validator will empty
        // notes into the cashbox leaving the requested floating amount in the payout. The minimum payout
        // is also setup so the validator will leave itself the ability to payout the minimum value requested.
        public void SetFloat(int minPayout, int floatAmount, char[] currency, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_FLOAT_AMOUNT;
            byte[] b = CHelpers.ConvertInt32ToBytes(minPayout);
            m_cmd.CommandData[1] = b[0];
            m_cmd.CommandData[2] = b[1];
            m_cmd.CommandData[3] = b[2];
            m_cmd.CommandData[4] = b[3];
            b = CHelpers.ConvertInt32ToBytes(floatAmount);
            m_cmd.CommandData[5] = b[0];
            m_cmd.CommandData[6] = b[1];
            m_cmd.CommandData[7] = b[2];
            m_cmd.CommandData[8] = b[3];

            // Add currency
            m_cmd.CommandData[9] = (byte)currency[0];
            m_cmd.CommandData[10] = (byte)currency[1];
            m_cmd.CommandData[11] = (byte)currency[2];

            m_cmd.CommandData[12] = 0x58; // real float (could use 0x19 for test)

            m_cmd.CommandDataLength = 13;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            if (log != null)
                log.AppendText("Tutar başarıyla kaydırıldı \r\n");
        }

        // This function uses the SMART EMPTY command which empties all the notes in the note
        // storage to the cashbox but unlike the EMPTY command it keeps a track of all the notes
        // it has moved. This data can be retrieved using the CASHBOX PAYOUT OPERATION DATA command.
        public void SmartEmpty(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_SMART_EMPTY;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return;

            if (log != null)
                log.AppendText("SMART EMPTY...\r\n");
        }

        // This function can be called after SMART events such as SMART empty. It will return a report
        // of what was moved to the cashbox. It returns a formatted string. The command can be used for
        // more useful things such as detecting what has been paid into the cashbox in the case of a payout
        // error.
        public string GetCashboxPayoutOpData(TextBox log = null)
        {
            // first send the command
            m_cmd.CommandData[0] = CCommands.SSP_CMD_CASHBOX_PAYOUT_OPERATION_DATA;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log))
                return "";

            // now deal with the response data
            // number of different notes
            int n = m_cmd.ResponseData[1];
            string displayString = "Number of Total Notes: " + n.ToString() + "\r\n\r\n";
            int i = 0;

            int adet = 0;
            int miktar = 0;
            int total=0;
            for (i = 2; i < (9 * n); i += 9)
            {
                displayString += "Moved " + CHelpers.ConvertBytesToInt16(m_cmd.ResponseData, i) +
                    " x " + CHelpers.FormatToCurrency(CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, i + 2)) +
                    " " + (char)m_cmd.ResponseData[i + 6] + (char)m_cmd.ResponseData[i + 7] + (char)m_cmd.ResponseData[i + 8] + " to cashbox\r\n";

                adet = CHelpers.ConvertBytesToInt16(m_cmd.ResponseData, i);
                miktar = CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, i + 2);

                total += adet * miktar;
            }


            var mfrm = (Form1)Application.OpenForms["Form1"];
           
           

            mfrm.send(
                "Boşaltılan Miktar: "+(total/100).ToString()+" ₺ \r\n"+
                CHelpers.ConvertBytesToInt16(m_cmd.ResponseData, 2) + "X" + CHelpers.FormatToCurrency(CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, 2 + 2)) + "\r\n" +
                CHelpers.ConvertBytesToInt16(m_cmd.ResponseData, 11) + "X" + CHelpers.FormatToCurrency(CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, 11 + 2)) + "\r\n" +
                CHelpers.ConvertBytesToInt16(m_cmd.ResponseData, 20) + "X" + CHelpers.FormatToCurrency(CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, 20 + 2)) + "\r\n" +
                CHelpers.ConvertBytesToInt16(m_cmd.ResponseData, 29) + "X" + CHelpers.FormatToCurrency(CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, 29 + 2)) + "\r\n" +
                CHelpers.ConvertBytesToInt16(m_cmd.ResponseData, 38) + "X" + CHelpers.FormatToCurrency(CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, 38 + 2)) + "\r\n" +
                CHelpers.ConvertBytesToInt16(m_cmd.ResponseData, 47) + "X" + CHelpers.FormatToCurrency(CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, 47 + 2)) + "\r\n" 

                );






            displayString += CHelpers.ConvertBytesToInt32(m_cmd.ResponseData, i) + " notes not recognised\r\n";

            if (log != null) log.AppendText(displayString);
            return displayString;
            
        }

        // This function sends the command LAST REJECT CODE which gives info about why a note has been rejected. It then
        // outputs the info to a passed across textbox.
        public void QueryRejection(TextBox log)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_LAST_REJECT_CODE;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log) || !CheckGenericResponses(log) || log == null)
                return;
            switch (m_cmd.ResponseData[1])
            {
                case 0x00: log.AppendText("Note accepted\r\n"); break;
                case 0x01: log.AppendText("Note length incorrect\r\n"); break;
                case 0x02: log.AppendText("Invalid note\r\n"); break;
                case 0x03: log.AppendText("Invalid note\r\n"); break;
                case 0x04: log.AppendText("Invalid note\r\n"); break;
                case 0x05: log.AppendText("Invalid note\r\n"); break;
                case 0x06: log.AppendText("Channel inhibited\r\n"); break;
                case 0x07: log.AppendText("Second note inserted during read\r\n"); break;
                case 0x08: log.AppendText("Host rejected note\r\n"); break;
                case 0x09: log.AppendText("Invalid note\r\n"); break;
                case 0x0A: log.AppendText("Invalid note read\r\n"); break;
                case 0x0B: log.AppendText("Note too long\r\n"); break;
                case 0x0C: log.AppendText("Validator disabled\r\n"); break;
                case 0x0D: log.AppendText("Mechanism slow/stalled\r\n"); break;
                case 0x0E: log.AppendText("Strim attempt\r\n"); break;
                case 0x0F: log.AppendText("Fraud channel reject\r\n"); break;
                case 0x10: log.AppendText("No notes inserted\r\n"); break;
                case 0x11: log.AppendText("Invalid note read\r\n"); break;
                case 0x12: log.AppendText("Twisted note detected\r\n"); break;
                case 0x13: log.AppendText("Escrow time-out\r\n"); break;
                case 0x14: log.AppendText("Bar code scan fail\r\n"); break;
                case 0x15: log.AppendText("Invalid note read\r\n"); break;
                case 0x16: log.AppendText("Invalid note read\r\n"); break;
                case 0x17: log.AppendText("Invalid note read\r\n"); break;
                case 0x18: log.AppendText("Invalid note read\r\n"); break;
                case 0x19: log.AppendText("Incorrect note width\r\n"); break;
                case 0x1A: log.AppendText("Note too short\r\n"); break;
            }
        }
        // This function gets the serial number of the device.  An optional Device parameter can be used
        // for TEBS systems to specify which device's serial number should be returned.
        // 0x00 = NV200
        // 0x01 = SMART Payout
        // 0x02 = Tamper Evident Cash Box.
        public void GetSerialNumber(byte Device, TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_SERIAL_NUMBER;
            m_cmd.CommandData[1] = Device;
            m_cmd.CommandDataLength = 2;


            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null)
            {
                // Response data is big endian, so reverse bytes 1 to 4.
                Array.Reverse(m_cmd.ResponseData, 1, 4);
                log.AppendText("Serial Number Device " + Device + ": ");
                log.AppendText(BitConverter.ToUInt32(m_cmd.ResponseData, 1).ToString());
                log.AppendText("\r\n");
            }
        }

        public void GetSerialNumber(TextBox log = null)
        {
            m_cmd.CommandData[0] = CCommands.SSP_CMD_GET_SERIAL_NUMBER;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log)) return;
            if (CheckGenericResponses(log) && log != null)
            {
                // Response data is big endian, so reverse bytes 1 to 4.
                Array.Reverse(m_cmd.ResponseData, 1, 4);
                log.AppendText("Serial Number ");
                log.AppendText(": ");
                log.AppendText(BitConverter.ToUInt32(m_cmd.ResponseData, 1).ToString());
                log.AppendText("\r\n");
            }
        }
        // The poll function is called repeatedly to poll to validator for information, it returns as
        // a response in the command structure what events are currently happening.
        public bool DoPoll(TextBox log)
        {
            byte i;

            // If a not is to be held in escrow, send hold commands, as poll releases note.
            if (m_HoldCount > 0)
            {
                m_NoteHeld = true;
                m_HoldCount--;
                m_cmd.CommandData[0] = CCommands.SSP_CMD_HOLD;
                m_cmd.CommandDataLength = 1;
                log.AppendText("Emanette tutulan Banknot: " + m_HoldCount + "\r\n");
                if (!SendCommand(log)) return false;
                return true;

            }

            //send poll
            m_cmd.CommandData[0] = CCommands.SSP_CMD_POLL;
            m_cmd.CommandDataLength = 1;

            if (!SendCommand(log))
                return false;
            if (m_cmd.ResponseData[0] == 0xFA)
                return false;

            // store response locally so data can't get corrupted by other use of the cmd variable
            byte[] response = new byte[255];
            m_cmd.ResponseData.CopyTo(response, 0);
            byte responseLength = m_cmd.ResponseDataLength;

            //parse poll response
            ChannelData data = new ChannelData();
            for (i = 1; i < responseLength; ++i)
            {
                switch (response[i])
                {
                    // This response indicates that the unit was reset and this is the first time a poll
                    // has been called since the reset.
                    case CCommands.SSP_POLL_SLAVE_RESET:
                        log.AppendText("Payout Reset\r\n");
                        UpdateData();
                        break;
                    // This response indicates the unit is disabled.
                    case CCommands.SSP_POLL_DISABLED:
                        log.AppendText("Payout Pasif...\r\n");
                        break;
                    // A note is currently being read by the validator sensors. The second byte of this response
                    // is zero until the note's type has been determined, it then changes to the channel of the 
                    // scanned note.
                    case CCommands.SSP_POLL_READ_NOTE:
                        if (m_cmd.ResponseData[i + 1] > 0)
                        {
                            GetDataByChannel(response[i + 1], ref data);
                            log.AppendText("Emanetteki Banknot, miktar: " + CHelpers.FormatToCurrency(data.Value) + "\r\n");
                            m_HoldCount = m_HoldNumber;
                        }
                        else
                            log.AppendText("Bankot okuma\r\n");
                        i++;
                        break;
                    // A credit event has been detected, this is when the validator has accepted a note as legal currency.
                    case CCommands.SSP_POLL_CREDIT_NOTE:
                        GetDataByChannel(response[i + 1], ref data);
                        log.AppendText("Kredi " + CHelpers.FormatToCurrency(data.Value) + "\r\n");
                        tutar = data.Value/100;

                        UpdateData();
                        i++;
                        break;
                    // A note is being rejected from the validator. This will carry on polling while the note is in transit.
                    case CCommands.SSP_POLL_NOTE_REJECTING:
                        log.AppendText("Banknot reddediliyor\r\n");
                        break;
                    // A note has been rejected from the validator, the note will be resting in the bezel. This response only
                    // appears once.
                    case CCommands.SSP_POLL_NOTE_REJECTED:
                        log.AppendText("Banknot reddedildi\r\n");
                        QueryRejection(log);
                        break;
                    // A note is in transit to the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKING:
                        log.AppendText("Banknot istifleniyor \r\n");
                        break;
                    // The payout device is 'floating' a specified amount of notes. It will transfer some to the cashbox and
                    // leave the specified amount in the payout device.
                    case CCommands.SSP_POLL_FLOATING:
                        log.AppendText("Kayan Banknotlar\r\n");
                        // Now the index needs to be moved on to skip over the data provided by this response so it
                        // is not parsed as a normal poll response.
                        // In this response, the data includes the number of countries being floated (1 byte), then a 4 byte value
                        // and 3 byte currency code for each country. 
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // A note has reached the cashbox.
                    case CCommands.SSP_POLL_NOTE_STACKED:
                        log.AppendText("Banknot yığılmış\r\n");
                        break;
                    // The float operation has been completed.
                    case CCommands.SSP_POLL_FLOATED:
                        log.AppendText("Tamamlanan yüzer\r\n");
                        UpdateData();
                        EnableValidator();
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // A note has been stored in the payout device to be paid out instead of going into the cashbox.
                    case CCommands.SSP_POLL_NOTE_STORED_IN_PAYOUT:
                        log.AppendText("Banknot kaydedildi\r\n");
                        
                        Properties.Settings.Default.kredi = Properties.Settings.Default.kredi + tutar;// sttings de tutyoruz toplanan değeri

                        UpdateData();
                        break;
                    // A safe jam has been detected. This is where the user has inserted a note and the note
                    // is jammed somewhere that the user cannot reach.
                    case CCommands.SSP_POLL_SAFE_NOTE_JAM:
                        log.AppendText("Safe jam\r\n");
                        break;
                    // An unsafe jam has been detected. This is where a user has inserted a note and the note
                    // is jammed somewhere that the user can potentially recover the note from.
                    case CCommands.SSP_POLL_UNSAFE_NOTE_JAM:
                        log.AppendText("Unsafe jam\r\n");
                        break;
                    // An error has been detected by the payout unit.
                    case CCommands.SSP_POLL_ERROR_DURING_PAYOUT: // Note: Will be reported only when Protocol version >= 7
                        log.AppendText("Payout cihazıyla ilgili hata algılandı\r\n");
                        i += (byte)((response[i + 1] * 7) + 2);
                        break;
                    // A fraud attempt has been detected. 
                    case CCommands.SSP_POLL_FRAUD_ATTEMPT:
                        log.AppendText("Dolandırıcılık girişimi\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The stacker (cashbox) is full.
                    case CCommands.SSP_POLL_STACKER_FULL:
                        log.AppendText("İstifleyici dolu\r\n");
                        break;
                    // A note was detected somewhere inside the validator on startup and was rejected from the front of the
                    // unit.
                    case CCommands.SSP_POLL_NOTE_CLEARED_FROM_FRONT:
                        log.AppendText("Note cleared from front of validator\r\n");
                        i++;
                        break;
                    // A note was detected somewhere inside the validator on startup and was cleared into the cashbox.
                    case CCommands.SSP_POLL_NOTE_CLEARED_TO_CASHBOX:
                        log.AppendText("Note cleared to cashbox\r\n");
                        i++;
                        break;
                    // A note has been detected in the validator on startup and moved to the payout device 
                    case CCommands.SSP_POLL_NOTE_PAID_INTO_STORE_AT_POWER_UP:
                        log.AppendText("Note paid into payout device on startup\r\n");
                        i += 7;
                        break;
                    // A note has been detected in the validator on startup and moved to the cashbox
                    case CCommands.SSP_POLL_NOTE_PAID_INTO_STACKER_AT_POWER_UP:
                        log.AppendText("Note paid into cashbox on startup\r\n");
                        i += 7;
                        break;
                    // The cashbox has been removed from the unit. This will continue to poll until the cashbox is replaced.
                    case CCommands.SSP_POLL_CASHBOX_REMOVED:
                        log.AppendText("Kasa kaldırıldı\r\n");
                        break;
                    // The cashbox has been replaced, this will only display on a poll once.
                    case CCommands.SSP_POLL_CASHBOX_REPLACED:
                        log.AppendText("Kasa değiştirildi\r\n");
                        break;
                    // The validator is in the process of paying out a note, this will continue to poll until the note has 
                    // been fully dispensed and removed from the front of the validator by the user.
                    case CCommands.SSP_POLL_DISPENSING:
                        log.AppendText("Dispensing note(s)\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The note has been dispensed and removed from the bezel by the user.
                    case CCommands.SSP_POLL_DISPENSED:
                        log.AppendText("Dispensed note(s)\r\n");
                        UpdateData();
                        EnableValidator();
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The payout device is in the process of emptying all its stored notes to the cashbox. This
                    // will continue to poll until the device is empty.
                    case CCommands.SSP_POLL_EMPTYING:
                        log.AppendText("Boşaltma\r\n");
                        break;
                    // This single poll response indicates that the payout device has finished emptying.
                    case CCommands.SSP_POLL_EMPTIED:
                        log.AppendText("Boşaltıldı\r\n");
                        Form1 frm = new Form1();
                        frm.send("Payout Boşaltıldı!\r\n");
                        UpdateData();
                        //EnableValidator();
                        break;
                    // The payout device is in the process of SMART emptying all its stored notes to the cashbox, keeping
                    // a count of the notes emptied. This will continue to poll until the device is empty.
                    case CCommands.SSP_POLL_SMART_EMPTYING:
                        log.AppendText("SMART Emptying...\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The payout device has finished SMART emptying, the information of what was emptied can now be displayed
                    // using the CASHBOX PAYOUT OPERATION DATA command.
                    case CCommands.SSP_POLL_SMART_EMPTIED:
                        log.AppendText("Akıllı Boşaltıldı, Bilgiler...\r\n");
                        UpdateData();
                        GetCashboxPayoutOpData(log);
                        //EnableValidator();
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The payout device has encountered a jam. This will not clear until the jam has been removed and the unit
                    // reset.
                    case CCommands.SSP_POLL_JAMMED:
                        log.AppendText("Unit jammed...\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // This is reported when the payout has been halted by a host command. This will report the value of
                    // currency dispensed upto the point it was halted. 
                    case CCommands.SSP_POLL_HALTED:
                        log.AppendText("Halted...\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // This is reported when the payout was powered down during a payout operation. It reports the original amount
                    // requested and the amount paid out up to this point for each currency.
                    case CCommands.SSP_POLL_INCOMPLETE_PAYOUT:
                        log.AppendText("Incomplete payout\r\n");
                        i += (byte)((response[i + 1] * 11) + 1);
                        break;
                    // This is reported when the payout was powered down during a float operation. It reports the original amount
                    // requested and the amount paid out up to this point for each currency.
                    case CCommands.SSP_POLL_INCOMPLETE_FLOAT:
                        log.AppendText("Incomplete float\r\n");
                        i += (byte)((response[i + 1] * 11) + 1);
                        break;
                    // A note has been transferred from the payout unit to the stacker.
                    case CCommands.SSP_POLL_NOTE_TRANSFERED_TO_STACKER:
                        log.AppendText("Note transferred to stacker\r\n");
                        i += 7;
                        break;
                    // A note is resting in the bezel waiting to be removed by the user.
                    case CCommands.SSP_POLL_NOTE_HELD_IN_BEZEL:
                        log.AppendText("Note in bezel...\r\n");
                        i += 7;
                        break;
                    // The payout has gone out of service, the host can attempt to re-enable the payout by sending the enable payout
                    // command.
                    case CCommands.SSP_POLL_PAYOUT_OUT_OF_SERVICE:
                        log.AppendText("Payout out of service...\r\n");
                        break;
                    // The unit has timed out while searching for a note to payout. It reports the value dispensed before the timeout
                    // event.
                    case CCommands.SSP_POLL_TIME_OUT:
                        log.AppendText("Timed out searching for a note\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    default:
                        log.AppendText("Unsupported poll response received: " + (int)m_cmd.ResponseData[i] + "\r\n");
                        break;
                }
            }
            return true;
        }

        /* Non-Command functions */

        // Use the library handler to open the port
        public bool OpenPort()
        {
            return LibraryHandler.OpenPort(ref m_cmd);
        }

        // Returns a nicely formatted string of the values and currencies of notes stored in each channel
        public string GetChannelLevelInfo()
        {
            string s = "";
            foreach (ChannelData d in m_UnitDataList)
            {
                s += (d.Value / 100f).ToString() + " " + d.Currency[0] + d.Currency[1] + d.Currency[2];
                s += " [" + d.Level + "] = " + ((d.Level * d.Value) / 100f).ToString();
                s += " " + d.Currency[0] + d.Currency[1] + d.Currency[2] + "\r\n";
            }
            return s;
        }

        // Updates all the data in the list.
        public void UpdateData(TextBox log = null)
        {
            foreach (ChannelData d in m_UnitDataList)
            {
                d.Level = CheckNoteLevel(d.Value, d.Currency, log);
                IsNoteRecycling(d.Value, d.Currency, ref d.Recycling, log);
            }
        }

        // This allows the caller to access all the data stored with a channel. An empty ChannelData struct is passed
        // over which gets filled with info.
        public void GetDataByChannel(int channel, ref ChannelData d)
        {
            // Iterate through each 
            foreach (ChannelData dList in m_UnitDataList)
            {
                if (dList.Channel == channel) // Compare channel
                {
                    d = dList; // Copy data from list to param
                    break;
                }
            }
        }

        /* Exception and Error Handling */

        // This is used for generic response error catching, it outputs the info in a
        // meaningful way.
        private bool CheckGenericResponses(TextBox log)
        {
            if (m_cmd.ResponseData[0] == CCommands.SSP_RESPONSE_OK)
                return true;
            else
            {
                if (log != null)
                {
                    switch (m_cmd.ResponseData[0])
                    {
                        case CCommands.SSP_RESPONSE_COMMAND_CANNOT_BE_PROCESSED:
                            if (m_cmd.ResponseData[1] == 0x03)
                            {
                                log.AppendText("Unit responded with a \"Busy\" response, command cannot be " +
                                    "processed at this time\r\n"); 
                            }
                            else
                            {
                                log.AppendText("Command response is CANNOT PROCESS COMMAND, error code - 0x"
                                + BitConverter.ToString(m_cmd.ResponseData, 1, 1) + "\r\n");
                            }
                            return false;
                        case CCommands.SSP_RESPONSE_FAIL:
                            log.AppendText("Command response is FAIL\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_KEY_NOT_SET:
                            log.AppendText("Command response is KEY NOT SET, renegotiate keys\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_PARAMETER_OUT_OF_RANGE:
                            log.AppendText("Command response is PARAM OUT OF RANGE\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_SOFTWARE_ERROR:
         
                            log.AppendText("Command response is SOFTWARE ERROR\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_COMMAND_NOT_KNOWN:
                            log.AppendText("Command response is UNKNOWN\r\n");
                            return false;
                        case CCommands.SSP_RESPONSE_WRONG_NO_PARAMETERS:
                            log.AppendText("Command response is WRONG PARAMETERS\r\n");
                            return false;
                        default:
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool SendCommand(TextBox log = null)
        {
            // attempt to send the command
            if (!LibraryHandler.SendCommand(ref m_cmd, ref info))
            {
                //m_Comms.UpdateLog(info, true);
                if (log != null) log.AppendText("Sending command failed\r\nPort status: " + m_cmd.ResponseStatus.ToString() + "\r\n");
                return false;
            }

            // update the log after every command
           // m_Comms.UpdateLog(info);
            return true;
        }
    };
}
