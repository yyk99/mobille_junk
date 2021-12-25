using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsApp3
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        int count = 0;
        Nano nano;
        Task<Record> tsk;
        void Handle_Clicked(object sender, System.EventArgs e)
        {
            count++;
#if false
            ((Button)sender).Text = $"You clicked {count} times.";
#else
            var butt = (Button)sender;
            if (nano == null)
            {
                nano = new Nano();
                tsk = nano.GetRecord();
                butt.Text = "Started...";
            } else
            {
                if (tsk.IsCompleted)
                {
                    var r = tsk.Result;
                    butt.Text = $"{count}: HUM: {r.h / 100.00}% TEMP: {r.c / 100.00}C {r.f / 100.00}F";
                    nano.Dispose();
                    tsk.Dispose();
                    nano = null;
                    tsk = null;
                } else
                {
                    butt.Text = "In progress...";
                }
            }
#endif
        }
    }
}
