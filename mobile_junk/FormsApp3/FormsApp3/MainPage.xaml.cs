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
            var butt = (Button)sender;
            ++count;
            butt.Text = $"You clicked {count} times.";

            var nano = new Nano();
            var r = await nano.GetRecord();
            butt.Text = $"{count}: HUM: {r.h / 100.00}% TEMP: {r.c / 100.00}C {r.f / 100.00}F";
        }

        // Handle_Clicked_Version
        async void Handle_Clicked_Version(object sender, System.EventArgs e)
        {
            var butt = (Button)sender;

            butt.Text = "0.0.2";

            await Task.Delay(1000);

            butt.Text = "Version";
        }
    }
}
