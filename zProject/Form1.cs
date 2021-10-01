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

namespace zProject
{ 
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine speech = new SpeechRecognitionEngine();
        SpeechSynthesizer jarvis = new SpeechSynthesizer();
        
        public Form1()
        {
            InitializeComponent();
            jarvis.SelectVoice("Microsoft David Desktop");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            jarvis.Speak("Welcome back Sir!");
            Home home = new Home();
            home.Show();
            this.Hide();
        }
    }
}
