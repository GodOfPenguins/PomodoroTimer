using System;
using System.Xml;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Media;
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int workTime = 1500;
        private int restTime = 300;
        private int pomodoros = 4;
        private int bigRest = 1800;
        private bool isWorkTime = true;
        private bool isBigRest = false;
        private int nowTime;
        private int pomodorosNow = 1;
        private SoundPlayer sound;
        private XmlDocument conf;
        public MainWindow()
        {
            InitializeComponent();
            Title = "Pomodoro Timer";
            sound = new SoundPlayer(@"Resources/sound.wav");
            loadConf();
            timeLabel.Content = GetTimeText(workTime);
            workBox.Text = workTime.ToString();
            restBox.Text = restTime.ToString();
            pomodoroBox.Text = pomodoros.ToString();
            bigRestBox.Text = bigRest.ToString();
            nowTime = workTime;
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(Timer_tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void Timer_tick(object sender, EventArgs e)
        {
            nowTime -= 1;
            if (nowTime != -1)
            {
                timeLabel.Content = GetTimeText(nowTime);
            }
            else
            {
                if (pomodorosNow == pomodoros)
                {
                    nowTime = bigRest + 1;
                    isWorkTime = false;
                    isBigRest = true;
                    pomodorosNow = 1;
                    nowInfo.Content = "Сейчас: Большой отдых";
                }
                else if (isWorkTime)
                {
                    nowTime = restTime + 1;
                    isWorkTime = false;
                    pomodorosNow += 1;
                    nowInfo.Content = "Сейчас: Отдых";
                }
                else
                {
                    nowTime = workTime + 1;
                    isWorkTime = true;
                    isBigRest = false;
                    nowInfo.Content = "Сейчас: Помидор " + pomodorosNow.ToString();
                }
                sound.Play();
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (workBox.Text == "" || restBox.Text == "" || pomodoroBox.Text == "" || bigRestBox.Text == "")
            {
                MessageBox.Show("Введите корректные значения!");
                return;
            }
            const int day = 86400;
            int work = Convert.ToInt32(workBox.Text);
            if (work > day)
            {
                work = day;
            }
            int rest = Convert.ToInt32(restBox.Text);
            if (rest > day)
            {
                rest = day;
            }
            int poms = Convert.ToInt32(pomodoroBox.Text);
            int bRst = Convert.ToInt32(bigRestBox.Text);
            if (bRst > day)
            {
                bRst = day;
            }
            

            workTime = work;
            restTime = rest;
            pomodoros = poms;
            bigRest = bRst;
            if (isWorkTime)
            {
                nowTime = work;
            }
            else if (isBigRest)
            {
                nowTime = bRst;
            }
            else
            {
                nowTime = rest;
            }
            saveConf();
        }

        private void saveConf()
        {
            XmlWriter xmlWriter = XmlWriter.Create("config.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("conf");

            xmlWriter.WriteStartElement("time");
            xmlWriter.WriteAttributeString("name", "work");
            xmlWriter.WriteAttributeString("sec", workTime.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("time");
            xmlWriter.WriteAttributeString("name", "rest");
            xmlWriter.WriteAttributeString("sec", restTime.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("time");
            xmlWriter.WriteAttributeString("name", "pomodoros");
            xmlWriter.WriteAttributeString("sec", pomodoros.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("time");
            xmlWriter.WriteAttributeString("name", "bigRest");
            xmlWriter.WriteAttributeString("sec",  bigRest.ToString());

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private void loadConf()
        {
            conf = new XmlDocument();
            if (File.Exists("config.xml"))
            {
                conf.Load(@"config.xml");
                XmlElement xRoot = conf.DocumentElement;
                XmlNodeList nodes = xRoot.SelectNodes("time");
                foreach (XmlNode n in nodes)
                {
                    if (n.SelectSingleNode("@name") is XmlNode && n.SelectSingleNode("@sec") is XmlNode)
                    {
                        string nodeName = n.SelectSingleNode("@name").Value;
                        string secString = n.SelectSingleNode("@sec").Value;
                        if (nodeName is string && secString is string)
                        {
                            int sec = Convert.ToInt32(secString);
                            if (nodeName == "work")
                            {
                                workTime = sec;
                            }
                            else if (nodeName == "rest")
                            {
                                restTime = sec;
                            }
                            else if (nodeName == "pomodoros")
                            {
                                pomodoros = sec;
                            }
                            else if (nodeName == "bigRest")
                            {
                                bigRest = sec;
                            }
                        }
                    }
                }
            }
            else
            {
                saveConf();
            }
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private string GetTimeText(int sec)
        {
            TimeSpan time = TimeSpan.FromSeconds(sec);
            string str = time.ToString(@"hh\:mm\:ss");
            return str;
        }
    }
}
