using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RaspberryPi
{
    public sealed partial class Add : ContentDialog
    {
        public List<String> dayslookup = new List<string>() { "Sunday" , "Monday", "Tuesday", "Wednesday", "Thursday", "Fryday", "Saturday"};
        public Add()
        {
            this.InitializeComponent();
            foreach (String day in dayslookup)
            {
                this.DayPicker.Items.Add(day);
            }
            this.DayPicker.SelectedIndex = 0;
        }


        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            String dag = this.DayPicker.SelectedItem.ToString();
            int daynum = 0;
            for (int i = 0; i < dayslookup.Count; i++)
            {
                if (dayslookup[i].Contains(dag))
                {
                    daynum = i;
                }
            }

            ApplicationData.Current.LocalSettings.Values["NewDay"] = daynum;
            ApplicationData.Current.LocalSettings.Values["NewHour"] = this.Hour.Text;
            ApplicationData.Current.LocalSettings.Values["NewMinute"] = this.Minute.Text;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
