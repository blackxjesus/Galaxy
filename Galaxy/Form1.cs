using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Shell32;
using Microsoft.Win32;
using System.Net;


namespace Galaxy
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine speech = new SpeechRecognitionEngine();
        SpeechSynthesizer galaxy = new SpeechSynthesizer();
        Random random = new Random();

        List<string> commands = new List<string>();

        class zInfo
        {
            public static string author = "Mohamed Ziad";
            public static string version = "B1.0";
            public static string name = "Galaxy";
        }

        public Form1()
        {
            InitializeComponent();

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("Galaxy", Application.ExecutablePath);

            speech.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
            LoadGrammar();
            speech.SetInputToDefaultAudioDevice();
            speech.RecognizeAsync(RecognizeMode.Multiple);
            galaxy.SelectVoice("Microsoft David Desktop");

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            label3.Text = DateTime.Now.ToString("dddd") + ", "+ DateTime.Now.ToString("M");

            galaxy.Speak("System is initialized. My name is " + zInfo.name);

            //string getMetarString = theWebClient.DownloadString("http://avwx.rest/api/metar/" + "lhbp" + "?options=&format=json&onfail=cache");
            //MessageBox.Show(getMetarString);
        }

        private void LoadGrammar()
        {
            #region -> Decleare commands <-
            commands.Add("galaxy");
            commands.Add("hey galaxy");
            commands.Add("goodbye");
            commands.Add("hi");
            commands.Add("hello");
            commands.Add("good morning");
            commands.Add("good afternoon");
            commands.Add("open google");
            commands.Add("tell me about yourself");
            commands.Add("tell me the time");
            commands.Add("search on the web");
            commands.Add("show desktop");
            commands.Add("minimize yourself");
            commands.Add("call me");
            commands.Add("open github");
            commands.Add("who are you");
            commands.Add("fuck you");
            #endregion

            #region - Understanding the command with AI -
            StreamWriter z = new StreamWriter("commands.dat");
            for (int i = 0; i < commands.Count; i++)
            {
                z.WriteLine(commands[i]);
            }
            z.Close();

            Choices text = new Choices();
            string[] lines = File.ReadAllLines(Environment.CurrentDirectory + "\\commands.dat");
            text.Add(lines);
            Grammar wordsList = new Grammar(new GrammarBuilder(text));
            speech.LoadGrammar(wordsList);
            #endregion
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //richTextBox1.Text = e.Result.Text;
            if (e.Result.Confidence > 0.4f)
            {
                string cmd = e.Result.Text;
                int r = 0;
                switch (cmd)
                {
                    #region -> Default <-
                    case "hey galaxy":
                    case "galaxy":
                        r = random.Next(4);
                        if (r == 0)
                        {
                            galaxy.Speak("Yes?");
                        }
                        else if (r == 1)
                        {
                            galaxy.Speak("Yes Sir?");
                        }
                        else if (r == 2)
                        {
                            galaxy.Speak("Hmmm?");
                        }
                        else if (r == 3)
                        {
                            galaxy.Speak("How can I help you?");
                        }
                        break;
                    case "good morning":
                        galaxy.Speak("Have a beautiful morning Sir");
                        break;
                    case "good afternoon":
                        galaxy.Speak("Have a good one!");
                        break;
                    case "goodbye":
                        r = random.Next(3);
                        if (r == 0)
                        {
                            galaxy.Speak("It was fun, bye bye! --");
                        }
                        else if (r == 1)
                        {
                            galaxy.Speak("Have a good day Sir!");
                        }
                        else if (r == 2)
                        {
                            galaxy.Speak("Shutting down, goodbye Sir!");
                        }
                        foreach (var item in Process.GetProcessesByName("Galaxy"))
                        {
                            System.Threading.Thread.Sleep(100);
                            item.Kill();
                        }
                        break;
                    #endregion

                    #region -> Web <-
                    case "open google":
                        galaxy.Speak("Opening google...");
                        OpenUrl("https://google.com");
                        break;
                    case "open github":
                        galaxy.Speak("Opening github...");
                        OpenUrl("https://github.com/blackxjesus/");
                        break;
                    case "search on the web":
                        galaxy.Speak("What do you want me to search for?");
                        if (galaxy.State == SynthesizerState.Ready)
                        {
                            OpenUrl("www.google.com/search?q=" + e.Result.Text);
                        }
                        //jarvis.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(
                        break;
                    #endregion

                    

                    #region phone call -> kidolgozandó
                    /*case "call me":
                        try
                        {
                            string phoneNumber = "+3670405155";
                            SerialPort sp = new SerialPort();
                            sp.PortName = "COM5";
                            sp.Open();
                            sp.WriteLine("AT" + Environment.NewLine);
                            Thread.Sleep(100);
                            sp.WriteLine("AT+CMFG=1" + Environment.NewLine);
                            Thread.Sleep(100);
                            sp.WriteLine("AT+CSCS=\"GSM\"" + Environment.NewLine);
                            Thread.Sleep(100);
                            sp.WriteLine("AT+CMGS=\"" + phoneNumber + "\"" + Environment.NewLine);
                            Thread.Sleep(100);
                            sp.WriteLine("szia" + Environment.NewLine);
                            Thread.Sleep(100);
                            sp.Write(new byte[] { 26 }, 0, 1);
                            Thread.Sleep(100);
                            var respone = sp.ReadExisting();
                            if (respone.Contains("ERROR"))
                            {
                                MessageBox.Show("Send failed", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("SMS sent", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            sp.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;*/
                    #endregion

                    #region -> System Commands <-
                    //Asztal mutatása
                    case "show desktop":
                        Shell objShel = new Shell();
                        objShel.ToggleDesktop();
                        break;
                    //Tálcára tétel
                    case "minimize yourself":
                        this.WindowState = FormWindowState.Minimized;
                        break;
                    //Pontos idő
                    case "tell me the time":
                        galaxy.Speak(DateTime.Now.ToString("HH") + "hour, " + DateTime.Now.ToString("mm") + " minutes.");
                        break;
                    #endregion

                    #region -> Others <-
                    case "fuck you":
                        galaxy.Speak("Ohh fuck you too!");
                        break;
                    case "tell me about yourself":
                        galaxy.Speak("My name is" + zInfo.name + ". I was created by Mohamed Ziad, the godfather.");
                        break;
                    #endregion

                    default:
                        break;
                }
            }
        }

        #region -> Methods <-

        private void SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            galaxy.Dispose();

        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString("HH");
            label2.Text = DateTime.Now.ToString("mm");
        }
    }
}
