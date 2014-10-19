using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AAHalc
{
    public partial class Form1 : Form
    {
        private delegate void UpdateDelegate();
        private readonly Object _thisLock = new Object();
        private readonly Image _bibleThump;
        public Form1()
        {
            InitializeComponent();
            UpdateDelegate up = TimerTick;
            TimeLeft = 1000;
            Thread thread1 = new Thread(new ThreadStart(up));
            thread1.Start();
            Phase.SelectedIndex = 0;
            _bibleThump = Start.BackgroundImage;
            Start.BackgroundImage = null;
        }

        public decimal TimeLeft = 0;
        public bool CountdownEnabled = false;
        void TimerTick()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if(!CountdownEnabled) continue;
                lock (_thisLock)
                {
                    SetTimer(SecondsToString((int)TimeLeft));
                    TimeLeft--;
                }
            }
        }

        string SecondsToString(int value)
        {
            int numberOfSeconds = value%60;
            int numberOfMinutes = (value / 60) % 60;
            int numberOfHours = (value / 3600) % 3600;
            return numberOfHours.ToString("0") + ":" + numberOfMinutes.ToString("0") + ":" + numberOfSeconds;
        }

        delegate void SetTextCallback(string text);
        private void SetTimer(string s)
        {
            if (TimeLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTimer);
                Invoke(d, new object[] {s});
            }
            else
            {
                TimeLabel.Text = s;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        // halcyona starts, at war for 1h 30minutes
        // 4 hours at peace
        // 15 minutes conflict
        // total: 5 hours 45 minutes
        private const int HalcyonaWarHours = 1;
        private const int HalcyonaWarMinutes = 30;
        private const int HalcyonaPeaceHours = 4;
        private const int HalcyonaPeaceMinutes = 0;
        private const int HalcyonaConflictMinutes = 15;
        const int HoursBetweenHalcyonas = HalcyonaWarHours + HalcyonaPeaceHours;
        const int MinutesBetweenHalcyonas = HalcyonaWarMinutes + HalcyonaPeaceMinutes + HalcyonaConflictMinutes;
        private void Start_Click(object sender, EventArgs e)
        {
            CountdownEnabled = true;
            DateTime today = DateTime.Now;
            int hoursLeft = (int)HoursInput.Value;
            int minutesLeft = (int)MinutesInput.Value;
            
            Console.WriteLine(Phase.SelectedItem.ToString());
            switch (Phase.SelectedItem.ToString())
            {
                case "At War":
                    hoursLeft += HalcyonaPeaceHours;
                    minutesLeft += HalcyonaPeaceMinutes + HalcyonaConflictMinutes;
                    break;
                case "At Peace":
                    minutesLeft += HalcyonaConflictMinutes;
                    break;
                case "Conflict":
                    break;
                default:
                    throw new ArgumentException("Phase is wrong");
            }

            lock (_thisLock)
            {
                TimeLeft = 3600 * hoursLeft + 60 * minutesLeft;
                HalcTimes.Items.Clear();
                for (int i = 0; i < 8; i++)
                {
                    DateTime tmp = today;
                    Console.WriteLine(tmp.TimeOfDay);
                    tmp = tmp.AddHours(hoursLeft + i * (HoursBetweenHalcyonas));
                    tmp = tmp.AddMinutes(minutesLeft + i * (MinutesBetweenHalcyonas));
                    string s = string.Format("{0:hh:mm:ss t}", tmp);
                    HalcTimes.Items.Add(tmp.ToShortTimeString());
                }
                Start.BackgroundImage = _bibleThump;
            }
        }

        private void HalcTimes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
