using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace HorseRaces05._03
{
    public partial class Form1 : Form
    {
        public SynchronizationContext uiContext;
        public List<Thread> threads = new List<Thread>();
        private int completed;
        private readonly object lockObj = new object();
        public Form1()
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current;
        }

        private void ThreadFunk(ProgressBar bar, string name)
        { 

            Random rnd = new Random();
            int speed = rnd.Next(30, 50);

            for(int i = 0; i < 1000; i++) { 
                Thread.Sleep(speed);
                uiContext.Send(d => bar.Value = i, null);
            }


            lock (lockObj)
            {
                completed++;
                uiContext.Send(d => listBox1.Items.Add($"{completed}е место - {name}"), null); 
                if(completed == 5)
                {
                    uiContext.Send(d => button1.Enabled = true, null);
                    
                }
            }
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (thread != null)
            //    thread.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            List<ProgressBar> progressBars = new List<ProgressBar> { progressBar1, progressBar2, progressBar3, progressBar4, progressBar5 };
            List<string> labeles = new List<string> { label1.Text,  label2.Text, label3.Text , label4.Text , label5.Text };
            listBox1.Items.Clear();

            foreach(var bar in progressBars)
            {
                uiContext.Send(d => bar.Maximum = 1000, null);
                uiContext.Send(d => bar.Value = 0, null);
            }
            completed = 0;
            int k = 0;
            foreach (var bar in progressBars)
            {
                int current = k;
                k++;
                Thread thread = new Thread(() => ThreadFunk(bar, labeles[current]));
                thread.IsBackground = true;
                threads.Add(thread);
                thread.Start();
                
            }
        }
    }
}
