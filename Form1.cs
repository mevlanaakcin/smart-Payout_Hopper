using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using ITLlib;
using whCcTalkCommunication;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Reflection;
namespace eSSP_example
{
    public partial class Form1 : Form
    {
        // Variables
        public bool hopperRunning = false, payoutRunning = false;
        public double odemeal = 0;
        volatile public bool hopperConnecting = false, payoutConnecting = false;
        int pollTimer = 250; // timer in ms
        CHopper Hopper; // The class that interfaces with the Hopper
        CPayout Payout; // The class that interfaces with the Payout
        bool FormSetup = false; // To ensure the form is only set up on first run
        frmPayoutByDenom payoutByDenomFrm; // Payout by denomination form
        delegate void OutputMessage(string msg); // Delegate for invoking on cross thread calls
        Thread tHopRec, tSPRec; // Handles to each of the reconnection threads for the 2 units

        // Constructor
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = pollTimer;
            timer2.Interval = 500; // update UI every 500ms
            this.Location = new Point(Screen.PrimaryScreen.Bounds.X + 50, Screen.PrimaryScreen.Bounds.Y + 30);
            this.Enabled = false;
        }


        private whSelectorComm CoinSelector = new whSelectorComm();
        private whCoinValue[] CoinValues = new whCoinValue[16];

        private whSelCoinStatus[] SelCoinStates = new whSelCoinStatus[16];



        private int PollCounter;
        public bool deviceEnable = false;
        public bool paramEnable = false;

        // WM_COPYDATA START

        const int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }
        [DllImport("User32.dll", SetLastError = true, EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        // WM_COPYDATA END

        // COMPORT READ DATA 

        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        
        private string comportRead(string tableName, string variableName)
        {
            StringBuilder sb = new StringBuilder(10);

            if (System.IO.File.Exists(Application.StartupPath +"\\comport.ini"))
            {
                
                GetPrivateProfileString(tableName, variableName, "", sb, sb.Capacity, Application.StartupPath + "\\comport.ini");
              
            }
            else
            {
                MessageBox.Show("Comport dosyası yok!");
                Environment.Exit(0);
            }
            return sb.ToString();
        }
      
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                    byte[] buff = new byte[cds.cbData];
                    Marshal.Copy(cds.lpData, buff, 0, cds.cbData);
                    string msg = Encoding.Unicode.GetString(buff, 0, cds.cbData);
                    sorgu(msg);
                    m.Result = (IntPtr)0;
                    return;
            }
            base.WndProc(ref m);
        }
    


        // WM_COPYDATA veri gönder
      public void send(string msg)
        {
            try
            {
                IntPtr hwnd = FindWindow(null, "Master");

                var cds = new COPYDATASTRUCT();
                byte[] buff = Encoding.Unicode.GetBytes(msg);
                cds.dwData = (IntPtr)0;
                cds.lpData = Marshal.AllocHGlobal(buff.Length);
                Marshal.Copy(buff, 0, cds.lpData, buff.Length);
                cds.cbData = buff.Length;
                var ret = SendMessage(hwnd, WM_COPYDATA, 0, ref cds);

                Marshal.FreeHGlobal(cds.lpData);
            }
            catch (Exception s)
            {
                textBox1.AppendText("Veri gönderme hatası :"+s.ToString()+"\r\n");

            }
            

           
           

            
        }
        // Ödeme alma ve Ödeme üstü Ücret İadesi fonksiyonu

        
       public void paypal(double tutar)
        {
            if (tutar > 0 && paramEnable==false)
            {
                deviceEnable = true;
            }
            
            //Yatırılan miktar istenen miktardan büyük veya eşit ise
            if (Properties.Settings.Default.kredi >= tutar && tutar!=0 && timeOutPay.Enabled==true) // hesaplama işlemine geçelim
                {

                timeOutPay.Enabled = false; // Zaman Aşımını Kapat

                if ((Properties.Settings.Default.kredi - tutar) > 0) // Ödeme istenenden fazla ise
                    {

                            
                        string para_ustu = (Properties.Settings.Default.kredi - tutar).ToString();

                        textBox1.AppendText("Yatırılacak Tutar : " + tutar+ " ₺\r\n" +
                                            "Yatırılan Tutar : " + Properties.Settings.Default.kredi+ " ₺\r\n" +
                                            "Ödenecek Para Üstü : " + para_ustu + " ₺\r\n");
                        try
                        {
                        CalculatePayout(para_ustu, "TRY".ToCharArray());
                           
                        send("Yatırılacak Tutar : " + tutar+ " ₺\r\n" +
                                "Yatırılan Tutar : " + Properties.Settings.Default.kredi+ " ₺\r\n" +
                                "Ödenecek Para Üstü : " + para_ustu + " ₺\r\n");

                        odemeal = 0;
                        Properties.Settings.Default.kredi = 0;

                        // Cihazlar kapatılıyor
                        deviceEnable = false;

                        }
                        catch (Exception e)
                        {
                        // Cihazlar kapatılıyor
                        deviceEnable = false;
                        send("Hata : "+e.ToString());
                        odemeal = 0;
                        Properties.Settings.Default.kredi = 0;
                        tutar = 0;
                    }

                        
                    }

                else // ödeme istenen ile aynı ise
                {
                    timeOutPay.Enabled = false; // Zaman Aşımını Kapat
                    try
                    {
                        string para_ustu = (Properties.Settings.Default.kredi - tutar).ToString();

                        textBox1.AppendText("Yatırılacak Tutar : " + tutar + " ₺\r\n" +
                                            "Yatırılan Tutar : " + Properties.Settings.Default.kredi + " ₺\r\n" +
                                            "Ödenecek Para Üstü : " + para_ustu + " ₺\r\n");

                        send("Yatırılacak Tutar : " + tutar + " ₺\r\n" +
                            "Yatırılan Tutar : " + Properties.Settings.Default.kredi + " ₺\r\n" +
                            "Ödenecek Para Üstü : " + para_ustu + " ₺\r\n");
                        odemeal = 0;
                        Properties.Settings.Default.kredi = 0;
                        tutar = 0;
                        // Cihazlar kapatılıyor
                        deviceEnable = false;
                    }
                    catch (Exception e)
                    {
                        // Cihazlar kapatılıyor
                        deviceEnable=false;
                        send("Hata : " + e.ToString());
                        odemeal = 0;
                        Properties.Settings.Default.kredi = 0;
                        tutar = 0;
                    }
                }
                
                }

             // İstenen Ödeme İşleminin tamamı istenilen zamanda yapılmamış ise
            if ( tutar > Properties.Settings.Default.kredi && tutar != 0 && timeOutPay.Enabled==false) // Zaman Aşımı durumu
            {
                string bakiye = Properties.Settings.Default.kredi.ToString();

                textBox1.AppendText("Zaman Aşımı \r\n"+
                                    "+Yatırılacak Tutar : " + tutar + " ₺\r\n" +
                                    "Yatırılan Tutar : " + Properties.Settings.Default.kredi + " ₺\r\n" +
                                    "İade Tutar: " + bakiye + " ₺\r\n");
                try
                {
                    CalculatePayout(bakiye, "TRY".ToCharArray());

                    send("Zaman Aşımı \r\n"+
                        "Yatırılacak Tutar : " + tutar + " ₺\r\n" +
                            "Yatırılan Tutar : " + Properties.Settings.Default.kredi + " ₺\r\n" +
                            "İade Tutar : " + bakiye + " ₺\r\n");
                    odemeal = 0;
                    Properties.Settings.Default.kredi = 0;

                    // Cihazlar kapatılıyor
                    deviceEnable = false;
                }
                catch (Exception e)
                {
                    // Cihazlar kapatılıyor
                    deviceEnable = false;
                    send("Hata : " + e.ToString());
                    odemeal = 0;
                    Properties.Settings.Default.kredi = 0;
                }
            }
            
        }

        // Coin para izinlerini setting için bool haline çevirirsin
        public string booler(string a)
        {
            string b;

            switch (a)
            {
                case "1":
                    b= "true";
                    break;
                case "0":
                    b = "false";
                    break;

                default:
                    b = "false";
                    break;
            }

            return b;
        }
        //Sistem kullanılan para kanallarını kaydetmiyoruz bu şekilde çözmüş olduk
        // Payout kanallarını set etme
        public void setStartPayoutChannel()
        {
            int len = 6;
            ChannelData kanal = new ChannelData();
            Payout.EnableValidator();

            for (int i = 1; i <= len; i++)
            {
                Payout.GetDataByChannel(i, ref kanal);

                if (Properties.Settings.Default.paperWay[i-1] == "1")
                {
                    Payout.ChangeNoteRoute(kanal.Value, kanal.Currency, false, textBox1);
                }
                else
                {
                    Payout.ChangeNoteRoute(kanal.Value, kanal.Currency, true, textBox1);
                }
                // Ensure payout ability is enabled in the validator
                Payout.EnablePayout();
            }
            Payout.DisableValidator();
            ConnectToSMARTPayout(textBox1);//İşlemden sonra master program tekrar bağlandı değişiklikler kaydedilsin diye

        }

        // Verileri wm_copydata ile paylaş
        public void sorgu(string m)
        {
            char[] d = m.ToCharArray();

            // Hangi EMP800 madeni paraların kabul edileceği kanalların belirlenmesi
            if (d[0]=='#') // Kanalları ayarla gönderilen key #1,0,0,0,0
            {
                char[] ayrac = {',','#' };
                string[] parcalar = m.Split(ayrac);
                // Gelen Madeni kanal izinlerini setting.settings e kaydediyoruz
                Properties.Settings.Default.coinWay[0] = booler(parcalar[1]);
                Properties.Settings.Default.coinWay[1] = booler(parcalar[2]);
                Properties.Settings.Default.coinWay[2] = booler(parcalar[3]);
                Properties.Settings.Default.coinWay[3] = booler(parcalar[4]);
                Properties.Settings.Default.coinWay[4] = booler(parcalar[5]);
                // Log textboxa durumu göster
                textBox1.AppendText("EMP800 izinleri: \r\n" +
                "Channel [0,05] --> {" + Properties.Settings.Default.coinWay[0].ToString() + "} \r\n" +
                "Channel [0,10] --> {" + Properties.Settings.Default.coinWay[1].ToString() + "} \r\n" +
                "Channel [0,25] --> {" + Properties.Settings.Default.coinWay[2].ToString() + "} \r\n" +
                "Channel [0,50] --> {" + Properties.Settings.Default.coinWay[3].ToString() + "} \r\n" +
                "Channel [1,00] --> {" + Properties.Settings.Default.coinWay[4].ToString() + "} \r\n"
                    );
                send(
                    "EMP800: \r\n\t[0,05 TRY] : {" + Properties.Settings.Default.coinWay[0].ToString() + "} \r\n" +
                    "\t   [0,10 ₺] : {" + Properties.Settings.Default.coinWay[1].ToString() + "} \r\n" +
                    "\t   [0,25 ₺] : {" + Properties.Settings.Default.coinWay[2].ToString() + "} \r\n" +
                    "\t   [0,50 ₺] : {" + Properties.Settings.Default.coinWay[3].ToString() + "} \r\n" +
                    "\t   [1,00 ₺] : {" + Properties.Settings.Default.coinWay[4].ToString() + "} \r\n" 
                    );

                //İşlemden sonra master program resetleme yapmalı değişiklikler kaydedilsin
            }

            // Hangi banknotların kabul edileceği ve geri ödeme yaparken kullanılacağı kanalların belirlenmesi
            else if (d[0]=='&') // Kanalları ayarla gönderilen key &1,1,1,1,0,0
            {
                char[] ayir = {',','&'};
                string[] parcalar = m.Split(ayir);
            
                //Geri Ödeme yaparken hangi kağıt para kanallarının kullanılacağı belirleniyor 
                try
                {

                    // Gelen kanal izinlerini İnhibit için kaydediyoruz
                    Properties.Settings.Default.paperWay[0] = parcalar[1];
                    Properties.Settings.Default.paperWay[1] = parcalar[2];
                    Properties.Settings.Default.paperWay[2] = parcalar[3];
                    Properties.Settings.Default.paperWay[3] = parcalar[4];
                    Properties.Settings.Default.paperWay[4] = parcalar[5];
                    Properties.Settings.Default.paperWay[5] = parcalar[6];

                    ChannelData kanal = new ChannelData();
                    Payout.EnableValidator();

                    for (int i = 1; i < parcalar.Length; i++)
                    {
                        Payout.GetDataByChannel(i, ref kanal);

                        if (parcalar[i] == "1")
                        {
                            Payout.ChangeNoteRoute(kanal.Value, kanal.Currency, false, textBox1);
                        }
                        else
                        {
                            Payout.ChangeNoteRoute(kanal.Value, kanal.Currency, true, textBox1);
                        }
                        // Ensure payout ability is enabled in the validator
                        Payout.EnablePayout();
                    }
                    // Thread.Sleep(2000);
                    //SetupFormLayout();
                    Payout.DisableValidator();
                    ConnectToSMARTPayout(textBox1);//İşlemden sonra master program tekrar bağlandı değişiklikler kaydedilsin diye




                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }

                

            }

            // Hopper Para üstü verme kanalları ayarlanıyor
            else if (d[0] == '%')
            {
                char[] ayir = { ',', '%' };
                string[] parcalar = m.Split(ayir);

                // Hopper ödeme kanallarının ayarları

                    for (int i = 1; i < parcalar.Length; i++)
                    {
                       
                        try
                        {
                            if (parcalar[i]=="1")
                                Hopper.RouteChannelToStorage(i, textBox1);
                            else
                                Hopper.RouteChannelToCashbox(i, textBox1);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            return;
                        }
                    }

               //İşlemden sonra master program resetleme yapmalı ki değişiklikler kaydedilsin

            }
            // ÖDEME AL
            else if (d[0]=='₺')
            {
                char[] charTrim = { '₺', ' ' };
                string payStr = m.Trim(charTrim);
                odemeal = Convert.ToDouble(payStr);

                deviceEnable = true;
                timeOutPay.Enabled = true; // zaman aşımını başlat
            }

            // PARA VER
            else if (d[0]=='=')
            {
                char[] charsToTrim = { '=', ' ' };
                string para_ustu = m.Trim(charsToTrim);
                

                textBox1.AppendText(para_ustu+"₺ para üstü ödemesi yapılıyor \r\n");
                CalculatePayout(para_ustu, "TRY".ToCharArray());
                //send(para_ustu+" ₺ ödendi !");
            }

            // Gelen verinin başında  '#' , '$' ve '&' yok ise
            else
            {
                switch (m)
                {
                    case "hopperLevel":  // Haznedeki Madeni para miktarı
                        send(Hopper.GetChannelLevelInfo());
                        break;

                    case "payoutLevel":  // Haznedeki Kağıt para miktarı
                        send(Payout.GetChannelLevelInfo());
                        break;

                    case "payoutKanal": // PAPER ödeme alırken hangi kanallar kullanılacak
                        // Log textboxa durumu goster
                        textBox1.AppendText("Payout:\r\n");

                        String msg = "";
                        msg="Paper:\r\n";

                        ChannelData kanal = new ChannelData();

                        for (int i = 1; i <= Payout.NumberOfChannels; i++)
                        {
                            Payout.GetDataByChannel(i, ref kanal);
                            textBox1.AppendText( CHelpers.FormatToCurrency(kanal.Value) +" " + new String(kanal.Currency)+":"+"[" +kanal.Recycling.ToString()+"] \r\n");
                            msg+=CHelpers.FormatToCurrency(kanal.Value) + " " + new String(kanal.Currency) + ":" + "[" + kanal.Recycling.ToString() + "] \r\n";
                        }
                        send(msg);
                        break;

                    case "empKanal": //Madeni para EMP800 ödeme alırken hangi kanallar kullanılacak
                       
                        send(
                            "EMP800: \r\n\t[0,05 TRY] : {" + Properties.Settings.Default.coinWay[0].ToString() + "} \r\n" +
                            "\t   [0,10 ₺] : {" + Properties.Settings.Default.coinWay[1].ToString() + "} \r\n" +
                            "\t   [0,25 ₺] : {" + Properties.Settings.Default.coinWay[2].ToString() + "} \r\n" +
                            "\t   [0,50 ₺] : {" + Properties.Settings.Default.coinWay[3].ToString() + "} \r\n" +
                            "\t   [1,00 ₺] : {" + Properties.Settings.Default.coinWay[4].ToString() + "} \r\n"
                            );
                        break;
                    case "hopperKanal": // kullanılan kanalları söyle

                        String Hmsg = "Hopper:\r\n";
                        // Hopper ödeme kanalları
                        for (int i = 1; i <= Hopper.NumberOfChannels; i++)
                        {
                            
                            textBox1.AppendText(CHelpers.FormatToCurrency(Hopper.GetChannelValue(i)) + " " + new String(Hopper.GetChannelCurrency(i))+": ["+ Hopper.IsChannelRecycling(i)+"]\r\n");
                            Hmsg += CHelpers.FormatToCurrency(Hopper.GetChannelValue(i)) + " " + new String(Hopper.GetChannelCurrency(i)) + ": [" + Hopper.IsChannelRecycling(i) + "]\r\n";
                        }

                        send(Hmsg);
                        break;
                    case "hopperReset": // Hopper Ünitesine RESET at
                        Hopper.Reset(textBox1);
                        send("OK");
                        break;

                    case "payoutReset": // Payout Ünitesine RESET at 
                        Payout.Reset(textBox1);
                        send("OK");
                        break;

                    case "empReset": // Madeni para Ünitesine RESET at
                        
                        CoinSelector.ResetDevice(50);
                       
                        send("OK");
                        break;

                    case "selen":  // Selenoid aç kapat
                        if (CoinSelector.IsOpen)
                        {
                            CoinSelector.TestSolenoids(0x01);
                            send("OK");
                        }
                        else{ send("NO"); }
                        break;

                    case "disable":   // Cihazları Pasif et

                        try
                        {
                            // Cihazlar Kapatılıyor
                            deviceEnable = false;
                            send("Cihazlar Kapatıldı!");
                        }
                        catch (Exception e)
                        {
                            send("Hata : "+e.ToString());
                        }
                        break;

                    case "enable":  // Cihazları Aktif et
                        try
                        {
                            // Cihazlar Açılıyor
                            deviceEnable = true;
                            send("Cihazlar Açıldı!");
                        }
                        catch (Exception e )
                        {
                            send("Hata : "+e.ToString());
                        }
                        break;

                    case "hopperEmpty":  // Hopperı Saymadan Direkt Kasaya Boşalt
                        Hopper.EnableValidator();
                        Hopper.EmptyDevice(textBox1);
                        send("Hopper Boşaltılıyor!\r\n");
                        break;

                    case "payoutEmpty":  // Payoutu Saymadan Direkt Kasaya Boşalt
                        Payout.EmptyPayoutDevice(textBox1);
                        send("Payout Boşaltılıyor!\r\n");
                        break;
                    case "hopperSmartEmpty": // Hopperı Sayarak Boşalt
                        Hopper.EnableValidator();
                        Hopper.SmartEmpty(textBox1);
                        break;

                    case "payoutSmartEmpty":  // Payoutu Sayarak Boşalt 
                        Payout.SmartEmpty(textBox1);
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    case "ss":
                        MessageBox.Show(Global.CoinComPort.ToString());
                        break;
                    default:
                        send("Tanımsız...");
                        break;
                }
            }
        }

        //EMP800 AKTİF
        public void emp800Enable()
        {
            if (CoinSelector.IsOpen)
            {
                EmpRead();
                timerCoin.Enabled = true;
                PollCounter = 0;
            }

            //Coin almayı aktif et
            CoinSelector.MasterInhibit = false;
            ReadCoinStates();
            CoinSelector.SetCoinInhibit(SelCoinStates);
            textBox1.AppendText(CoinSelector.LastError.ToString());

        }

        //EMP800 PASİF
        public void emp800Disable()
        {
            CoinSelector.MasterInhibit = true;
            CoinSelector.SetCoinInhibit(false);
            textBox1.AppendText(CoinSelector.LastError.ToString());

            timerCoin.Enabled = false;

        }
        // okunacak coin listesi
        private void ReadCoinStates()
        {
            
            // coin lerin giriş çıkışlarına  izin verdiğim yer
            SelCoinStates[00].Inhibit = bool.Parse(Properties.Settings.Default.coinWay[0]);  // 0,05 TRY
            SelCoinStates[01].Inhibit = bool.Parse(Properties.Settings.Default.coinWay[1]); ; // 0,10 TRY
            SelCoinStates[02].Inhibit = bool.Parse(Properties.Settings.Default.coinWay[2]); ; // 0,25 TRY
            SelCoinStates[03].Inhibit = bool.Parse(Properties.Settings.Default.coinWay[3]); ; // 0,50 TRY
            SelCoinStates[04].Inhibit = bool.Parse(Properties.Settings.Default.coinWay[4]); ; // 1,00 TRY
            SelCoinStates[05].Inhibit = false; // Tanımsız
            SelCoinStates[06].Inhibit = false; // Tanımsız
            SelCoinStates[07].Inhibit = false; // Tanımsız
            SelCoinStates[08].Inhibit = false; // Tanımsız
            SelCoinStates[09].Inhibit = false; // Tanımsız
            SelCoinStates[10].Inhibit = false; // Tanımsız
            SelCoinStates[11].Inhibit = false; // Tanımsız
            SelCoinStates[12].Inhibit = false; // Tanımsız
            SelCoinStates[13].Inhibit = false; // Tanımsız
            SelCoinStates[14].Inhibit = false; // Tanımsız
            SelCoinStates[15].Inhibit = false; // Tanımsız

        }


        public void EmpRead()
        {
            CoinSelector.GetCoinValues(ref CoinValues);
            if (CoinSelector.LastError == whCcTalkErrors.Ok)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (CoinValues[i].IntValue > 0)
                    {
                        textBox1.AppendText(string.Format("{0:d02}: {1:f2} {2} \r\n", i + 1, CoinValues[i].Value, CoinValues[i].ID));
                    }
                    else
                    {
                        textBox1.AppendText(string.Format("{0:d02}: \r\n", i + 1));
                    }
                }
            }

            textBox1.AppendText(CoinSelector.LastError.ToString() + "\r\n");
        }
        // coin çalıştırma 
     

        public void ConnectEmp800()
        {
            if (!CoinSelector.IsOpen)
            {
                //---------- EMP800 Connect--------
                CoinSelector.Port = Global.CoinComPort;
                CoinSelector.OpenComm();
                textBox1.AppendText("** ");
                textBox1.AppendText("EMP800 Status :" + CoinSelector.LastError.ToString());
                textBox1.AppendText("** \r\n");
                if (CoinSelector.LastError == whCcTalkErrors.Ok)
                {
                    CoinSelector.ResetDevice(30);
                }

            }
        }
        public void disConnectEmp800()
        {
            //EMP800 bağlantı koparma işlemi 
            if (CoinSelector.IsOpen)
            {
                CoinSelector.CloseComm();
                textBox1.AppendText(CoinSelector.LastError.ToString() + "\n");
                timerCoin.Enabled = false;
            }

        }


        // This updates UI variables such as textboxes etc.
        void UpdateUI()
        {
            // Get stored notes info from SMART Payout and SMART Hopper at intervals
            if (!timer2.Enabled)
            {
                tbChannelLevels.Text = Payout.GetChannelLevelInfo();
                tbCoinLevels.Text = Hopper.GetChannelLevelInfo();
                timer2.Enabled = true;
            }
        }

        // The main program loop, this is to control the validator, it polls at
        // a value set in this class (pollTimer).
        public void MainLoop()
        {
            Properties.Settings.Default.kredi = 0;
            this.Enabled = true;
            btnRun.Enabled = false;
            btnHalt.Enabled = true;

            // Connect to the validators (non-threaded for initial connect)
            ConnectToSMARTPayout(textBox1);
            ConnectToHopper(textBox1);
            ConnectEmp800();
            // Enable validators
            // Payout.EnableValidator();
            // Hopper.EnableValidator();
            
            // While app active
            while (!CHelpers.Shutdown)
            {
                if (deviceEnable==true && paramEnable==false)
                {
                    // Cihazlar Açılıyor
                    Payout.EnableValidator();
                    Hopper.EnableValidator();
                    emp800Enable();
                    paramEnable = true;
                    send("Cihazlar Açıldı!");
                }
                if (deviceEnable==false && paramEnable==true)
                {
                    Properties.Settings.Default.kredi = 0;   // yatırılan toplam parayı sıfırla
                    // Cihazlar kapatılıyor
                    Payout.DisableValidator();
                    Hopper.DisableValidator();
                    emp800Disable();
                    paramEnable = false;
                    send("Cihazlar Kapatıldı!");
                }
                // Setup form layout on first run
                if (!FormSetup)
                {
                    setStartPayoutChannel();
                    SetupFormLayout();
                    FormSetup = true;
                }

                // If the Hopper is supposed to be running but the poll fails
                if (hopperRunning && !hopperConnecting && !Hopper.DoPoll(textBox1))
                {
                    textBox1.AppendText("Lost connection to SMART Hopper\r\n");
                    // If the other unit isn't running, refresh the port by closing it
                    if (!payoutRunning) LibraryHandler.ClosePort();
                    hopperRunning = false;
                    // Create and start a reconnection thread, this allows this loop to continue executing
                    // and polling the other validator
                    tHopRec = new Thread(() => ReconnectHopper());
                    tHopRec.Start();
                }
                // Same as above but for the Payout
                if (payoutRunning && !payoutConnecting && !Payout.DoPoll(textBox1))
                {
                    textBox1.AppendText("Lost connection to SMART Payout\r\n");
                    // If the other unit isn't running, refresh the port by closing it
                    if (!hopperRunning) LibraryHandler.ClosePort();
                    payoutRunning = false;
                    // Create and start a reconnection thread, this allows this loop to continue executing
                    // and polling the other validator
                    tSPRec = new Thread(() => ReconnectPayout());
                    tSPRec.Start();
                }

                if (!CoinSelector.IsOpen)
                {
                    textBox1.AppendText("EMP800 bağlantı koptu, tekrar bağlanılıyor...\r\n");
                    ConnectEmp800();
                }

                istenen.Text = odemeal.ToString() + " ₺";
                tutar.Text = Properties.Settings.Default.kredi.ToString() + " ₺";
             
                
                paypal(odemeal);
                
                UpdateUI();
                timer1.Enabled = true;
                while (timer1.Enabled) Application.DoEvents();
            }

            btnRun.Enabled = true;
            btnHalt.Enabled = false;
        }

        // This is a one off function that is called the first time the MainLoop()
        // function runs, it just sets up a few of the UI elements that only need
        // updating once.
        public void SetupFormLayout()
        {
            // Get channel levels in hopper
            tbCoinLevels.Text = Hopper.GetChannelLevelInfo();

            // setup list of recyclable channel tick boxes based on OS type
            System.OperatingSystem osInfo = System.Environment.OSVersion;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            // XP, 2000, Server 2003
            if (osInfo.Platform == PlatformID.Win32NT && osInfo.Version.Major == 5)
            {
                x1 = this.Location.X + 455;
                y1 = this.Location.Y + 10;
                x2 = this.Location.X + 780;
                y2 = this.Location.Y + 10;
            }
            // Vista, 7
            else if (osInfo.Platform == PlatformID.Win32NT && osInfo.Version.Major == 6)
            {
                x1 = this.Location.X + 458;
                y1 = this.Location.Y + 12;
                x2 = this.Location.X + 780;
                y2 = this.Location.Y + 12;
            }

            GroupBox g1 = new GroupBox();
            g1.Size = new Size(100, 380);
            g1.Location = new Point(x1, y1);
            g1.Text = "Hopper Recycling";

            GroupBox g2 = new GroupBox();
            g2.Size = new Size(100, 380);
            g2.Location = new Point(x2, y2);
            g2.Text = "Payout Recycling";

            // Hopper checkboxes
            for (int i = 1; i <= Hopper.NumberOfChannels; i++)
            {
                CheckBox c = new CheckBox();
                c.Location = new Point(5, 20 + (i * 20));
                c.Name = i.ToString();
                c.Text = CHelpers.FormatToCurrency(Hopper.GetChannelValue(i)) + " " + new String(Hopper.GetChannelCurrency(i));
                c.Checked = Hopper.IsChannelRecycling(i);
                c.CheckedChanged += new EventHandler(recycleBoxHopper_CheckedChange);
                g1.Controls.Add(c);
            }

            // Payout checkboxes
            for (int i = 1; i <= Payout.NumberOfChannels; i++)
            {
                CheckBox c = new CheckBox();
                c.Location = new Point(5, 20 + (i * 20));
                c.Name = i.ToString();
                ChannelData d = new ChannelData();
                Payout.GetDataByChannel(i, ref d);
                c.Text = CHelpers.FormatToCurrency(d.Value) + " " + new String(d.Currency);
                c.Checked = d.Recycling;
                //c.Checked = bool.Parse(Properties.Settings.Default.paperWay[i-1]); // Seçimleri işaretle
                c.CheckedChanged += new EventHandler(recycleBoxPayout_CheckedChange);
                g2.Controls.Add(c);
            }

            Controls.Add(g1);
            Controls.Add(g2);
        }
     
        public void ConnectToHopper(TextBox log = null)
        {
            hopperConnecting = true;
            // setup timer, timeout delay and number of attempts to connect
            System.Windows.Forms.Timer reconnectionTimer = new System.Windows.Forms.Timer();
            reconnectionTimer.Tick += new EventHandler(reconnectionTimer_Tick);
            reconnectionTimer.Interval = 1000; // ms
            int attempts = 10;

            // Setup connection info
            Hopper.CommandStructure.ComPort = Global.ValidatorComPort;
            Hopper.CommandStructure.SSPAddress = Global.Validator2SSPAddress;
            Hopper.CommandStructure.BaudRate = 9600;
            Hopper.CommandStructure.Timeout = 1000;
            Hopper.CommandStructure.RetryLevel = 3;

            // Run for number of attempts specified
            for (int i = 0; i < attempts; i++)
            {
                if (log != null) log.AppendText("Trying connection to SMART Hopper\r\n");

                // turn encryption off for first stage
                Hopper.CommandStructure.EncryptionStatus = false;

                // if the key negotiation is successful then set the rest up
                if (Hopper.OpenPort() && Hopper.NegotiateKeys(log))
                {
                    Hopper.CommandStructure.EncryptionStatus = true; // now encrypting
                    // find the max protocol version this validator supports
                    byte maxPVersion = FindMaxHopperProtocolVersion();
                    if (maxPVersion >= 6)
                        Hopper.SetProtocolVersion(maxPVersion, log);
                    else
                    {
                        MessageBox.Show("This program does not support units under protocol 6!", "ERROR");
                        hopperConnecting = false;
                        return;
                    }
                    // get info from the hopper and store useful vars
                    Hopper.HopperSetupRequest(log);
                    // check the right unit is connected
                    if (!IsHopperSupported(Hopper.UnitType))
                    {
                        MessageBox.Show("Unsupported type shown by SMART Hopper, this SDK supports the SMART Payout and the SMART Hopper only");
                        hopperConnecting = false;
                        Application.Exit();
                        return;
                    }
                    // inhibits, this sets which channels can receive coins
                    Hopper.SetInhibits(log);
                    // Get serial number.
                    Hopper.GetSerialNumber(textBox1);
                    // set running to true so the hopper begins getting polled
                    hopperRunning = true;
                    hopperConnecting = false;
                    return;
                }
                // reset timer
                reconnectionTimer.Enabled = true;
                while (reconnectionTimer.Enabled)
                {
                    if (CHelpers.Shutdown)
                    {
                        hopperConnecting = false;
                        return;
                    }

                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }
            hopperConnecting = false;
            return;
        }

    
   

        public void ConnectToSMARTPayout(TextBox log = null)
        {


            payoutConnecting = true;
            // setup timer, timeout delay and number of attempts to connect
            System.Windows.Forms.Timer reconnectionTimer = new System.Windows.Forms.Timer();
            reconnectionTimer.Tick += new EventHandler(reconnectionTimer_Tick);
            reconnectionTimer.Interval = 1000; // ms
            int attempts = 10;

            // Setup connection info
            Payout.CommandStructure.ComPort = Global.ValidatorComPort;
            Payout.CommandStructure.SSPAddress = Global.Validator1SSPAddress;
            Payout.CommandStructure.BaudRate = 9600;
            Payout.CommandStructure.Timeout = 1000;
            Payout.CommandStructure.RetryLevel = 3;

            // Run for number of attempts specified
            for (int i = 0; i < attempts; i++)
            {
                if (log != null) log.AppendText("Trying connection to SMART Payout\r\n");

                // turn encryption off for first stage
                Payout.CommandStructure.EncryptionStatus = false;

                // Open port first, if the key negotiation is successful then set the rest up
                if (Payout.OpenPort() && Payout.NegotiateKeys(log))
                {
                    Payout.CommandStructure.EncryptionStatus = true; // now encrypting
                    // find the max protocol version this validator supports
                    byte maxPVersion = FindMaxPayoutProtocolVersion();
                    if (maxPVersion >= 6)
                        Payout.SetProtocolVersion(maxPVersion, log);
                    else
                    {
                        MessageBox.Show("This program does not support units under protocol 6!", "ERROR");
                        payoutConnecting = false;
                        return;
                    }
                    // get info from the validator and store useful vars
                    Payout.PayoutSetupRequest(log);
                    // check the right unit is connected
                    if (!IsValidatorSupported(Payout.UnitType))
                    {
                        MessageBox.Show("Unsupported type shown by SMART Payout, this SDK supports the SMART Payout and the SMART Hopper only");
                        payoutConnecting = false;
                        Application.Exit();
                        return;
                    }
                    // inhibits, this sets which channels can receive notes
                    Payout.SetInhibits(log);

                    // Get serial number.
                    Payout.GetSerialNumber(log);
                    // enable payout
                    Payout.EnablePayout(log);
                    // set running to true so the validator begins getting polled
                    payoutRunning = true;
                    payoutConnecting = false;
                    return;
                }
                // reset timer
                reconnectionTimer.Enabled = true;
                while (reconnectionTimer.Enabled)
                {
                    if (CHelpers.Shutdown)
                    {
                        payoutConnecting = false;
                        return;
                    }

                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }
            payoutConnecting = false;
            return;
        }

        // Used when invoking for cross-thread calls
        private void AppendToTextBox(string s)
        {
            textBox1.AppendText(s);
        }

        // These functions are run in a seperate thread to allow the main loop to continue executing.
        private void ReconnectHopper()
        {
            OutputMessage m = new OutputMessage(AppendToTextBox);
            while (!hopperRunning)
            {
                if (textBox1.InvokeRequired)
                    textBox1.Invoke(m, new object[] { "Attempting to reconnect to SMART Hopper...\r\n" });
                else
                    textBox1.AppendText("Attempting to reconnect to SMART Hopper...\r\n");

                ConnectToHopper();
                CHelpers.Pause(1000);
                if (CHelpers.Shutdown) return;
            }
            if (textBox1.InvokeRequired)
                textBox1.Invoke(m, new object[] { "Reconnected to SMART Hopper\r\n" });
            else
                textBox1.AppendText("Reconnected to SMART Hopper\r\n");
            Hopper.EnableValidator();
        }

        private void ReconnectPayout()
        {
            OutputMessage m = new OutputMessage(AppendToTextBox);
            while (!payoutRunning)
            {
                if (textBox1.InvokeRequired)
                    textBox1.Invoke(m, new object[] { "Attempting to reconnect to SMART Payout...\r\n" });
                else
                    textBox1.AppendText("Attempting to reconnect to SMART Payout...\r\n");

                ConnectToSMARTPayout(null); // Have to pass null as can't update text box from a different thread without invoking

                CHelpers.Pause(1000);
                if (CHelpers.Shutdown) return;
            }
            if (textBox1.InvokeRequired)
                textBox1.Invoke(m, new object[] { "Reconnected to SMART Payout\r\n" });
            else
                textBox1.AppendText("Reconnected to SMART Payout\r\n");
            Payout.EnableValidator();
        }

        // This function finds the maximum protocol version that a validator supports. To do this
        // it attempts to set a protocol version starting at 6 in this case, and then increments the
        // version until error 0xF8 is returned from the validator which indicates that it has failed
        // to set it. The function then returns the version number one less than the failed version.
        private byte FindMaxHopperProtocolVersion()
        {
            // not dealing with protocol under level 6
            // attempt to set in hopper
            byte b = 0x06;
            while (true)
            {
                // If command can't get through, break out
                if (!Hopper.SetProtocolVersion(b)) break;
                // If it fails then it can't be set so fall back to previous iteration and return it
                if (Hopper.CommandStructure.ResponseData[0] == CCommands.SSP_RESPONSE_FAIL)
                    return --b;
                b++;

                if (b > 20)
                    return 0x06; // return default if protocol gets too high (must be failure)
            }
            return 0x06; // default
        }

        private byte FindMaxPayoutProtocolVersion()
        {
            // not dealing with protocol under level 6
            // attempt to set in validator
            byte b = 0x06;
            while (true)
            {
                Payout.SetProtocolVersion(b);
                // If it fails then it can't be set so fall back to previous iteration and return it
                if (Payout.CommandStructure.ResponseData[0] == CCommands.SSP_RESPONSE_FAIL)
                    return --b;
                b++;

                // If the protocol version 'runs away' because of a drop in comms. Return the default value.
                if (b > 20)
                    return 0x06;
            }
        }

        // This function shows a simple example of calculating a payout split between the SMART Payout and the 
        // SMART Hopper. It works on a highest value split, first the notes are looked at, then any remainder
        // that can't be paid out with a note is paid from the SMART Hopper.
        private void CalculatePayout(string amount, char[] currency)
        {
            float payoutAmount;
            try
            {
                // Parse it to a number
                payoutAmount = float.Parse(amount) * 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            int payoutList = 0;
            // Obtain the list of sorted channels from the SMART Payout, this is sorted by channel value
            // - lowest first
            List<ChannelData> reverseList = new List<ChannelData>(Payout.UnitDataList);
            reverseList.Reverse(); // Reverse the list so the highest value is first

            // Iterate through each
            foreach(ChannelData d in reverseList)
            {
                ChannelData temp = d; // Don't overwrite real values
                // Keep testing to see whether we need to payout this note or the next note
                while (true)
                {
                    // If the amount to payout is greater than the value of the current note and there is
                    // some of that note available and it is the correct currency
                    if (payoutAmount >= temp.Value && temp.Level > 0 && String.Equals(new String(temp.Currency), new String(currency)))
                    {
                        payoutList += temp.Value; // Add to the list of notes to payout from the SMART Payout
                        payoutAmount -= temp.Value; // Minus from the total payout amount
                        temp.Level--; // Take one from the level
                    }
                    else
                        break; // Don't need any more of this note
                }
            }

            // Test the proposed payout values
            if (payoutList > 0)
            {
                // First test SP
                Payout.PayoutAmount(payoutList, currency, true);
                if (Payout.CommandStructure.ResponseData[0] != 0xF0)
                {
                    DialogResult res =
                        MessageBox.Show("Smart Payout unable to pay requested amount, attempt to pay all from Hopper?",
                        "Error with Payout", MessageBoxButtons.YesNo);
                    send("Ödeme yapılamadı! \r\n");

                    if (res == System.Windows.Forms.DialogResult.No)
                        return ;
                    else
                        payoutAmount += payoutList;
                }

                // SP is ok to pay
                Payout.PayoutAmount(payoutList, currency, false, textBox1);
                //send((payoutList/100).ToString() + " ₺ payout Ödedi !");
            }

            // Now if there is any left over, request from Hopper
            if (payoutAmount > 0)
            {
                // Test Hopper first
                Hopper.PayoutAmount((int)payoutAmount, currency, true);
                if (Hopper.CommandStructure.ResponseData[0] != 0xF0)
                {
                    MessageBox.Show("İstenen tutar ödenemiyor!");
                    send("Ödeme yapılamadı! \r\n");
                    return;
                }

                // Hopper is ok to pay
                Hopper.PayoutAmount((int)payoutAmount, currency, false, textBox1);
                // send((payoutAmount/100).ToString() +" ₺ hopper Ödedi !");
                
            }
        }

        // This function checks whether the type of validator is supported by this program.
        private bool IsValidatorSupported(char type)
        {
            if (type == (char)0x06)
                return true;
            return false;
        }

        private bool IsHopperSupported(char type)
        {
            if (type == (char)0x03)
                return true;
            return false;
        }
        /* Events handling section */

        NotifyIcon Myicon = new NotifyIcon();
        private void Form1_Load(object sender, EventArgs e)
        {
            Myicon.Icon = new Icon(@"C:\Users\Mevlana\Desktop\SMART Payout - SMART Hopper\VS2010 Project\eSSP_example\img\atm.ico");
            // Create instances of the validator classes
            Hopper = new CHopper();
            Payout = new CPayout();
            if (Hopper == null || Payout == null)
            {
                MessageBox.Show("Error with memory allocation, exiting", "ERROR");
                Application.Exit();
            }
            btnHalt.Enabled = true;
            // Load settings
            logTickBox.Checked = Properties.Settings.Default.Comms;

            // Position comms windows
            Point p = Location;
            p.Y += this.Height;
           // Hopper.Comms.Location = p;
            //p.X += Hopper.Comms.Width;
           // Payout.CommsLog.Location = p;
        }

        // sanırım burada form açılış ayarı var
        private void Form1_Shown(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;

            try
            {

                Global.ValidatorComPort =comportRead("PORTS","INOV_COM").ToString();
                Global.Validator1SSPAddress =Byte.Parse(comportRead("PORTS","SSP1"),System.Globalization.NumberStyles.None);
                Global.Validator2SSPAddress =Byte.Parse(comportRead("PORTS", "SSP2"), System.Globalization.NumberStyles.None);
                Global.CoinComPort = Convert.ToInt16(comportRead("PORTS", "EMP_COM"));// EMP800 port bilgisini al

                MainLoop();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "EXCEPTION");
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("Started poll loop\r\n");
            MainLoop();
        }

        private void btnHalt_Click(object sender, EventArgs e)
        {

            textBox1.AppendText("Poll loop stopped\r\n");
            
            btnRun.Enabled = true;
            btnHalt.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
       
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
        }

      

        private void logTickBox_CheckedChanged(object sender, EventArgs e)
        {
            if (logTickBox.Checked)
            {
                //Hopper.Comms.Show();
                //Payout.CommsLog.Show();
            }
            else
            {
                //Hopper.Comms.Hide();
                //Payout.CommsLog.Hide();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult exit = MessageBox.Show("Programdan çıkmak istediğinize emin misiniz ?","Test",MessageBoxButtons.YesNo);

            if (exit==DialogResult.Yes)
            {
                CHelpers.Shutdown = true;
                LibraryHandler.ClosePort();
                Properties.Settings.Default.Comms = logTickBox.Checked;
                Properties.Settings.Default.Save();

                // Cihazlar kapatılıyor
                deviceEnable = false;
                
            }

            else if (exit==DialogResult.No)
            {
                
                notifyIcon1.Visible = true;
                e.Cancel = true;
            }
           
        }

        private void btnPayout_Click(object sender, EventArgs e)
        {
            if (tbPayout.Text != "" && tbPayoutCurrency.Text != "")
                CalculatePayout(tbPayout.Text, tbPayoutCurrency.Text.ToCharArray());
        }

        private void btnEmptyHopper_Click(object sender, EventArgs e)
        {
            Hopper.EmptyDevice(textBox1);
        }

        private void btnSmartEmptyHopper_Click(object sender, EventArgs e)
        {
            Hopper.SmartEmpty(textBox1);
        }

        private void btnEmptySMARTPayout_Click(object sender, EventArgs e)
        {
            Payout.EmptyPayoutDevice(textBox1);
        }

        private void btnSMARTEmpty_Click(object sender, EventArgs e)
        {
            Payout.SmartEmpty(textBox1);
        }


        // Kullanılan hopper paraları belirler hopper
        private void recycleBoxHopper_CheckedChange(object sender, EventArgs e)
        {
            CheckBox c = sender as CheckBox;
            try
            {
                if (c.Checked)
                    Hopper.RouteChannelToStorage(Int32.Parse(c.Name), textBox1);
                else
                    Hopper.RouteChannelToCashbox(Int32.Parse(c.Name), textBox1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }
      
        // Kullanılan payout paraları belirler payout
        private void recycleBoxPayout_CheckedChange(object sender, EventArgs e)
        {
            CheckBox chkbox = sender as CheckBox;
            try
            {
                ChannelData d = new ChannelData();
                Payout.GetDataByChannel(Int32.Parse(chkbox.Name), ref d);

                if (chkbox.Checked)
                {
                    Payout.ChangeNoteRoute(d.Value, d.Currency, false, textBox1);
                }
                else
                {
                    Payout.ChangeNoteRoute(d.Value, d.Currency, true, textBox1);
                }
                // Ensure payout ability is enabled in the validator
                Payout.EnablePayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }



        private void btnResetHopper_Click(object sender, EventArgs e)
        {
            Hopper.Reset(textBox1);
            // Force reconnect by closing com port
            //CommsLibrary.CloseComPort();
        }

        private void btnResetPayout_Click(object sender, EventArgs e)
        {
            Payout.Reset(textBox1);
            // Force reconnect by closing com port
            //CommsLibrary.CloseComPort();
        }

        private void setLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new frmSetLevel(Hopper);
            f.Show();
        }

        private void setAllToZeroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < Hopper.NumberOfChannels + 1; i++)
                Hopper.SetCoinLevelsByChannel(i, 0, textBox1);
            Hopper.UpdateData();
        }

        private void timerCoin_Tick(object sender, EventArgs e)
        {

            whSelPollResponse[] PollResponses = new whSelPollResponse[5];
            int Events;

            CoinSelector.PollSelector(ref PollResponses, out Events);

            if (CoinSelector.LastError == whCcTalkErrors.Ok)
            {
                if (Events > 0)
                {
                    for (int i = 0; i < Events; i++)
                    {
                        PollCounter++;
                        if (PollResponses[i].Status == whSelPollEvent.Coin)
                        {
                            textBox1.AppendText(string.Format("{0:d04}: {1:f2} {2} \r\n", PollCounter, CoinValues[PollResponses[i].CoinIndex].Value, CoinValues[PollResponses[i].CoinIndex].ID));
                            //MessageBox.Show(string.Format("{0:d04}: {1:f2} {2}\n", PollCounter, CoinValues[PollResponses[i].CoinIndex].Value, CoinValues[PollResponses[i].CoinIndex].ID));
                            // MessageBox.Show( PollCounter.ToString()+" | "+ CoinValues[PollResponses[i].CoinIndex].Value.ToString()+" | "+ CoinValues[PollResponses[i].CoinIndex].ID.ToString());

                            // Kanal 
                            //1 =0,10
                            //2 =0,25
                            //3 =0,50
                            //4 =1.00 kanalı olarak kullanılır artan değeride işleyiniz
                            //Hopper.SetCoinLevelsByChannel(4,1,textBox1);
                            

                            switch (CoinValues[PollResponses[i].CoinIndex].Value.ToString())
                            {
                                case "1":
                                    Hopper.SetCoinLevelsByChannel(4, 1, textBox1);
                                    Properties.Settings.Default.kredi += 1;
                                    break;
                                case "0,5":
                                    Hopper.SetCoinLevelsByChannel(3, 1, textBox1);
                                    Properties.Settings.Default.kredi += 0.5;
                                    break;
                                case "0,25":
                                    Hopper.SetCoinLevelsByChannel(2, 1, textBox1);
                                    Properties.Settings.Default.kredi += 0.25;
                                    break;
                                case "0,1":
                                    Hopper.SetCoinLevelsByChannel(1, 1, textBox1);
                                    Properties.Settings.Default.kredi += 0.10;
                                    break;


                                default:
                                    break;
                            }
                        }
                        else
                        {

                            textBox1.AppendText(string.Format("{0:d04}: {1} \r\n", PollCounter, PollResponses[i].Status.ToString()));

                        }
                    }
                }
            }
            else
            {
                PollCounter++;
                textBox1.AppendText(string.Format("{0:d04}: {1} \r\n", PollCounter, CoinSelector.LastError.ToString()));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Kanal 
            //1 =0,10
            //2 =0,25
            //3 =0,50
            //4 =1.00 kanalı olarak kullanılır artan değeride işleyiniz
            //Hopper.SetCoinLevelsByChannel(4,1,textBox1);
            Form f = new frmSetLevel(Hopper);
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void timeOutPay_Tick(object sender, EventArgs e)
        {
            textBox1.AppendText("Zaman Aşımı ! Ödeme İstenen sürede yapılmadı \r\n");
            timeOutPay.Enabled = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState== FormWindowState.Minimized)
            {
                Hide();
                Myicon.Visible = true;
                Myicon.MouseDoubleClick += new MouseEventHandler(Form1_DoubleClick);
            }

        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void reconnectionTimer_Tick(object sender, EventArgs e)
        {
            if (sender is  System.Windows.Forms.Timer)
            {
                System.Windows.Forms.Timer t = sender as System.Windows.Forms.Timer;
                t.Enabled = false;
            }
        }

        private void btnFloat_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (tbFloatAmount.Text == "" || tbMinPayout.Text == "" || tbFloatCurrency.Text == "")
                    return;

                // Parse to a float
                float fFa = float.Parse(tbFloatAmount.Text);
                float fMp = float.Parse(tbMinPayout.Text);

                int fa = (Int32)(fFa * 100); // multiply by 100 for penny value
                // If payout selected
                if (cbFloatSelect.Text == "SMART Payout")
                {
                    int mp = (int)(fMp * 100); // multiply by 100 for penny value
                    Payout.SetFloat(mp, fa, tbFloatCurrency.Text.ToCharArray(), textBox1);
                }
                // Or if Hopper
                else if (cbFloatSelect.Text == "SMART Hopper")
                {
                    short mp = (short)(fMp * 100); // multiply by 100 for penny value
                    Hopper.SetFloat(mp, fa, tbFloatCurrency.Text.ToCharArray(), textBox1);
                }
                else
                    MessageBox.Show("Choose a device to float from!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void btnPayoutByDenom_Click(object sender, EventArgs e)
        {
            if (hopperRunning && payoutRunning && ((payoutByDenomFrm == null) || (payoutByDenomFrm != null && !payoutByDenomFrm.Visible)))
            {
                payoutByDenomFrm = new frmPayoutByDenom(Payout, Hopper, textBox1);
                payoutByDenomFrm.Show();
            }
        }
    }
}
