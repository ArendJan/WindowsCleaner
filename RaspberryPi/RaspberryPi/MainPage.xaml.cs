using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Net.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspberryPi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.IPaddres.Text = "http://192.168.2.9/"; // fill in your default.
            connect_Click(new object(), new RoutedEventArgs());
            
        }
        


        private async void connect_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                this.HelloMessage.Text = "CHECKING...";
                String IP = this.IPaddres.Text;
                String requestURL = "UP";
                HttpClient z = new HttpClient();
                z.BaseAddress = new Uri(IP);
                HttpResponseMessage response = await z.GetAsync(requestURL);
                
                if (response.IsSuccessStatusCode)
                {
                    String UP = await response.Content.ReadAsStringAsync();
                    UP = UP.Replace(System.Environment.NewLine, "");
                    this.HelloMessage.Text = UP.Length.ToString();

                    //this.HelloMessage.Text = String.Equals('y', UP).ToString();
                    if (String.Equals("y", UP))
                    {
                        this.HelloMessage.Text = "Connected!";

                       Frame rootFrame = Window.Current.Content as Frame;
                        
                        rootFrame.Navigate(typeof(data), IP);
                        
                    }
                    else
                    {
                        HelloMessage.Text = "Not an Arduino running the (correct) code(confirmation is not received)";
                    }
                }
            }
            catch (HttpRequestException error)
            {

                this.HelloMessage.Text = "Arduino is not up / IP not correct / no internet connection";

            }
            catch(UriFormatException error)
            {
                this.HelloMessage.Text = "Not a correct IP addres.";
            }
        }
    }

}
