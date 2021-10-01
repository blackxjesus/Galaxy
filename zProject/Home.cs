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

namespace zProject
{
    public partial class Home : Form
    {
        SpeechRecognitionEngine speech = new SpeechRecognitionEngine();
        SpeechSynthesizer jarvis = new SpeechSynthesizer();
        Random random = new Random();

        List<string> commands = new List<string>();
        
        class zInfo
        {
            public static string author = "Mohamed Ziad";
            public static string version = "B1.0";
            public static string name = "Galaxy";
        }

        public Home()
        {
            InitializeComponent();

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("zProject", Application.ExecutablePath);

            speech.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
            LoadGrammar();
            speech.SetInputToDefaultAudioDevice();
            speech.RecognizeAsync(RecognizeMode.Multiple);
            jarvis.SelectVoice("Microsoft David Desktop");
        }

        private void Home_Load(object sender, EventArgs e)
        {
            //jarvis.State == SynthesizerState.Ready
            //jarvis.Speak("System initialized. My name is" + zInfo.name);
        }

        private void LoadGrammar()
        {
            #region -> Decleare commands <-
            commands.Add("jarvis");
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
                    case "jarvis":
                        r = random.Next(4);
                        if (r == 0)
                        {
                            jarvis.Speak("Yes?");
                        }
                        else if (r == 1)
                        {
                            jarvis.Speak("Yes Sir?");
                        }
                        else if (r == 2)
                        {
                            jarvis.Speak("Hmmm?");
                        }
                        else if (r == 3)
                        {
                            jarvis.Speak("How can I help you?");
                        }
                        break;
                    case "good morning":
                        jarvis.Speak("Have a beautiful morning Sir");
                        break;
                    case "good afternoon":
                        jarvis.Speak("Have a good one!");
                        break;
                    case "goodbye": 
                        r = random.Next(3);
                        if (r == 0)
                        {
                            jarvis.Speak("It was fun, bye bye! --");
                        }
                        else if (r == 1)
                        {
                            jarvis.Speak("Have a good day Sir!");
                        }
                        else if (r == 2)
                        {
                            jarvis.Speak("Shutting down, goodbye Sir!");
                        }
                        foreach (var item in Process.GetProcessesByName("zProject"))
                        {
                            System.Threading.Thread.Sleep(1006);
                            item.Kill();
                        }
                        break;
                    #endregion

                    #region -> Web <-
                    case "open google":
                        jarvis.Speak("Opening google...");
                        OpenUrl("https://google.com");
                        break;
                    case "search on the web":
                        jarvis.Speak("What do you want me to search for?");
                        if (jarvis.State == SynthesizerState.Ready)
                        {
                            OpenUrl("www.google.com/search?q=" + e.Result.Text);
                        }
                        //jarvis.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(
                        break;
                    #endregion

                    case "tell me about yourself":
                        jarvis.Speak("My name is" + zInfo.name + ". I was created by Mohamed Ziad, the godfather.");
                        break;

                    case "call me":
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
                        break;


                    #region -> System Commands <-
                    case "show desktop":
                        Shell objShel = new Shell();
                        objShel.ToggleDesktop();
                        break;
                    case "minimize yourself":
                        this.WindowState = FormWindowState.Minimized;
                        break;
                    case "tell me the time":
                        jarvis.Speak(DateTime.Now.ToString("HH") + "hour, " + DateTime.Now.ToString("mm") + " minutes.");
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
            jarvis.Dispose();
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
    }
}
