using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Embedded;

namespace FormsApp3
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public Version GetVersion()
        {
            return new Version(0, 2, 2);
        }

        System.Threading.CancellationTokenSource stopWait = new System.Threading.CancellationTokenSource();
        int count = 0;
        async void Handle_Clicked_Async(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Handle_Clicked_Async: tid={System.Threading.Thread.CurrentThread.ManagedThreadId}");
            var butt = (Button)sender;
            ++count;
            butt.Text = $"You clicked {count} times.";

            var nano = new Nano(GetSource());
            var r = await nano.GetRecord();
            if (r.stamp != 0)
                butt.Text = $"{count}: HUM: {r.h / 100.00}% TEMP: {r.c / 100.00}C {r.f / 100.00}F";
            else
                butt.Text = "Timeout...";
            System.Diagnostics.Debug.WriteLine($"Handle_Clicked_Async: tid={System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        async void Handle_Clicked_Ping(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Clicked_Ping: tid={System.Threading.Thread.CurrentThread.ManagedThreadId}");
            var butt = (Button)sender;
            ++count;
            butt.Text = $"You clicked {count} times.";

            var nano = new Nano(GetSource());
            string r = await nano.Ping();
            butt.Text = $"{nano.RemoteEndpoint.Address} {r.Trim()}";

            try
            {
                stopWait = new System.Threading.CancellationTokenSource();
                await Task.Delay(10 * 1000, stopWait.Token);
            }
            catch {  }
            butt.Text = "Ping Me";

            System.Diagnostics.Debug.WriteLine($"Clicked_Ping: tid={System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        int source_cnt = 0;
        string[] sources = { "192.168.1.136" , "192.168.1.137" };

        string GetSource()
        {
            int pos = source_cnt % sources.Length;
            return sources[pos];
        }

        async void Handle_Clicked_Source(object sender, System.EventArgs e)
        {
            var butt = (Button)sender;
            ++source_cnt;

            butt.Text = $"Selected: {GetSource()}";

            await Task.Delay(5 * 1000, stopWait.Token);

            butt.Text = "Select Source";
        }

        // Handle_Clicked_Version
        async void Handle_Clicked_Version(object sender, System.EventArgs e)
        {
            var butt = (Button)sender;

            butt.Text = $"Version: {GetVersion()}\nSource: {GetSource()}";

            try 
            {
                stopWait = new System.Threading.CancellationTokenSource();
                await Task.Delay(3000, stopWait.Token);
            }  
            catch  
            {
                // ...
            }

            butt.Text = "Info";
        }

        void OnSourceRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var butt = (RadioButton)sender;

            if (butt == null)
                return;
            if (butt.IsChecked)
            {
                System.Diagnostics.Debug.Write($"value={butt.ContentAsString()}, IsChecked={butt.IsChecked}");
                var pos = Array.IndexOf(sources, butt.ContentAsString());
                if (pos >= 0)
                    source_cnt = pos;
                System.Diagnostics.Debug.Write($"source_cnt={source_cnt}");
                stopWait.Cancel();
            }
        }
    }
}
