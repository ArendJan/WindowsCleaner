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
using Windows.ApplicationModel.Core;
using Windows.Data;

using System.Net.Http;
using Windows.UI.Core;
using Windows.Storage;
using System.Xml.Serialization;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RaspberryPi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class data : Page
    {
        public String IP = "";
        public List<String> dayslookup = new List<string>() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Fryday", "Saturday" };
        public Boolean AUTO = true;
        public int x;
        public System.Threading.Timer timer;
        public List<SEvent> ScheduledList;
        private String day;
        public bool CurrentlyWorking = false;

        public data()
        {
            this.InitializeComponent();



        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            ScheduledList = new List<SEvent> { };
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("ScheduledList"))
            {
                String serialize = ApplicationData.Current.LocalSettings.Values["ScheduledList"].ToString();
                XmlSerializer xml = new XmlSerializer(ScheduledList.GetType());
                StringReader reader = new StringReader(serialize);

                ScheduledList = (List<SEvent>)xml.Deserialize(reader);
            }
            else
            {
                ScheduledList.Add(new SEvent() { Day = 0, hour = 12, minutes = 00 });
                XmlSerializer xml = new XmlSerializer(ScheduledList.GetType());
                StringWriter textWriter = new StringWriter();
                xml.Serialize(textWriter, ScheduledList);
                String serialized = textWriter.ToString();
                ApplicationData.Current.LocalSettings.Values["ScheduledList"] = serialized;
            }
            foreach (SEvent even in ScheduledList)
            {
                this.listBox.Items.Add(dayslookup[even.Day] + " " + even.hour + ":" + even.minutes);

            }

            IP = e.Parameter.ToString();
            this.StatusBox.Text = "Connected!";
            this.AutoSwitch.IsOn = true;
            day = DateTime.Today.DayOfWeek.ToString();
            //List<SEvent> ScheduledList = new List<SEvent>();
            int CHour = (int)System.DateTime.Now.Hour;
            int CMin = (int)System.DateTime.Now.Minute;
            this.StatusBox.Text = CHour.ToString() + ":" + CMin.ToString();
            // S
            timer = new System.Threading.Timer(TimerTick, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 1));

            //int lastMinute = 1;



        }
        async void TimerTick(object state)
        {
            x++;
            if (x <= 20)
            {
                x = 0;
                int CleaningTime = -10; //The time needed for 1 clean, with a minus, this feature is for roomba's which don't autoDock after a specific amount of time, or the battery dies before that.
                int Dock2 = 5; //The time needed for appr docking.
                /*
                Normal use:
                Checking time --> time is right --> cleaning --> stop and go seek dock after CleaningTime minutes (twice dock command)
                    --> after Dock2 minutes it receives another dock command, but it ignores it because of it is already there.
                When crashed or something:
                ....... --> Receives twice Dock command, so doesn't do anything --> after Dock2 minutes it receives again a dock command, so it goes seek dock.

                Hopefully always good result.


                */
                int CDay = (int)System.DateTime.Now.DayOfWeek; //CurrentTime
                int CHour = (int)System.DateTime.Now.Hour;
                int CMin = (int)System.DateTime.Now.Minute;

                int DDay = (int)System.DateTime.Now.AddMinutes(CleaningTime).DayOfWeek; //DockTime
                int DHour = (int)System.DateTime.Now.AddMinutes(CleaningTime).Hour;
                int DMin = (int)System.DateTime.Now.AddMinutes(CleaningTime).Minute;

                Dock2 = CleaningTime - Dock2;
                int D2Day = (int)System.DateTime.Now.AddMinutes(Dock2).DayOfWeek; //Dock2Time
                int D2Hour = (int)System.DateTime.Now.AddMinutes(Dock2).Hour;
                int D2Min = (int)System.DateTime.Now.AddMinutes(Dock2).Minute;
                
                foreach (SEvent e in ScheduledList)
                {
                    
                    if (e.Day == CDay && e.hour == CHour && e.minutes == CMin)
                    {
                        CurrentlyWorking = true;
                         await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {



                            this.StatusBox.Text = "CLEANING...";

                        });
                        try
                        {
                            //String IP = this.IPaddres.Text;
                            String requestURL = "CLEAN";
                            HttpClient z = new HttpClient();
                            z.BaseAddress = new Uri(IP);
                            HttpResponseMessage response = await z.GetAsync(requestURL);
                            

                        }
                        catch (HttpRequestException error)
                        {

                             //this.StatusBox.Text = error.Message.ToString();

                        }
                    }
                    else if (e.Day == DDay && e.hour == DHour && e.minutes == DMin)
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {



                            this.StatusBox.Text = "DOCKING...(1)";

                        });
                        for (int x = 0; x < 2; x++)
                        {
                           try
                            {
                                //String IP = this.IPaddres.Text;
                                String requestURL = "DOCK";
                                HttpClient z = new HttpClient();
                                z.BaseAddress = new Uri(IP);
                                HttpResponseMessage response = await z.GetAsync(requestURL);


                            }
                            catch (HttpRequestException error)
                            {

                                // this.StatusBox.Text = error.Message.ToString();

                            }
                        }
                    }
                    else if (e.Day == D2Day && e.hour == D2Hour && e.minutes == D2Min)
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {



                            this.StatusBox.Text = "DOCKING...(2)";

                        });
                        
                            try
                            {
                                //String IP = this.IPaddres.Text;
                                String requestURL = "DOCK";
                                HttpClient z = new HttpClient();
                                z.BaseAddress = new Uri(IP);
                                HttpResponseMessage response = await z.GetAsync(requestURL);


                            }
                            catch (HttpRequestException error)
                            {

                               //  this.StatusBox.Text = error.Message.ToString();

                            }
                        
                    }

                }

            }

        }
        private async void CleanClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //String IP = this.IPaddres.Text;
                String requestURL = "CLEAN";
                HttpClient z = new HttpClient();
                z.BaseAddress = new Uri(IP);
                HttpResponseMessage response = await z.GetAsync(requestURL);
                this.StatusBox.Text = "CLEANING...";
            }
            catch (HttpRequestException error)
            {

                // this.StatusBox.Text = error.Message.ToString();

            }
        }
        private async void DockClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //String IP = this.IPaddres.Text;
                String requestURL = "DOCK";
                HttpClient z = new HttpClient();
                z.BaseAddress = new Uri(IP);
                HttpResponseMessage response = await z.GetAsync(requestURL);
                this.StatusBox.Text = "DOCKING...";
            }
            catch (HttpRequestException error)
            {

                // this.StatusBox.Text = error.Message;

            }
        }
        
        private void AutoSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AUTO = !AUTO;
            
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ScheduledList.RemoveAt(this.listBox.SelectedIndex);
            XmlSerializer xml = new XmlSerializer(ScheduledList.GetType());
            StringWriter textWriter = new StringWriter();
            xml.Serialize(textWriter, ScheduledList);
            String serialized = textWriter.ToString();
            ApplicationData.Current.LocalSettings.Values["ScheduledList"] = serialized;
            this.listBox.Items.Clear();
            foreach (SEvent even in ScheduledList)
            {
                this.listBox.Items.Add(dayslookup[even.Day] + " " + even.hour + ":" + even.minutes);

            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            

            ContentDialog add = new Add();
            
            ContentDialogResult res = await add.ShowAsync();
            if (res.ToString().Equals("Primary"))
            {
                ScheduledList.Add(new SEvent() { Day = Int32.Parse(ApplicationData.Current.LocalSettings.Values["NewDay"].ToString()), hour = Int32.Parse(ApplicationData.Current.LocalSettings.Values["NewHour"].ToString()), minutes = Int32.Parse(ApplicationData.Current.LocalSettings.Values["NewMinute"].ToString()) });
                XmlSerializer xml = new XmlSerializer(ScheduledList.GetType());
                StringWriter textWriter = new StringWriter();
                xml.Serialize(textWriter, ScheduledList);
                String serialized = textWriter.ToString();
                ApplicationData.Current.LocalSettings.Values["ScheduledList"] = serialized;
                this.listBox.Items.Clear();
                foreach (SEvent even in ScheduledList)
                {
                    this.listBox.Items.Add(dayslookup[even.Day] + " " + even.hour + ":" + even.minutes);

                }
                
            }
        }
    }
}
