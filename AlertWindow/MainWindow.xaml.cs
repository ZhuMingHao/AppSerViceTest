using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace AlertWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        private AppServiceConnection connection = null;
        public MainWindow()
        {
            InitializeComponent();
            InitializeAppServiceConnection();
        }
        private async void InitializeAppServiceConnection()
        {
            connection = new AppServiceConnection();
            connection.AppServiceName = "SampleInteropService";
            connection.PackageFamilyName = Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            AppServiceConnectionStatus status = await connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                MessageBox.Show(status.ToString());
                this.IsEnabled = false;
            }
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Application.Current.Shutdown();
            }));
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            // retrive the reg key name from the ValueSet in the request
            string key = args.Request.Message["KEY"] as string;

            if (key.Length > 0)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    InfoBlock.Text = key;

                }));
                ValueSet response = new ValueSet();
                response.Add("OK", "SEND SUCCESS");
                await args.Request.SendResponseAsync(response);
            }
            else
            {
                ValueSet response = new ValueSet();
                response.Add("ERROR", "INVALID REQUEST");
                await args.Request.SendResponseAsync(response);
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ValueSet response = new ValueSet();
            response.Add("OK", "AlerWindow Message");
            await connection.SendMessageAsync(response);
        }

    }
}
