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

        int count = 0;
        async void Handle_Clicked_Async(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Handle_Clicked_Async: tid={System.Threading.Thread.CurrentThread.ManagedThreadId}");
            var butt = (Button)sender;
            ++count;
            butt.Text = $"You clicked {count} times.";

            var nano = new Nano();
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

            var nano = new Nano();
            string r = await nano.Ping();
            butt.Text = $"{count}: {r.TrimEnd()}";

            await Task.Delay(1000);
            butt.Text = "Ping Me";

            System.Diagnostics.Debug.WriteLine($"Clicked_Ping: tid={System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        // Handle_Clicked_Version
        async void Handle_Clicked_Version(object sender, System.EventArgs e)
        {
            var butt = (Button)sender;

            butt.Text = "0.0.4";

            await Task.Delay(1000);

            butt.Text = "Version";
        }
    }
}
