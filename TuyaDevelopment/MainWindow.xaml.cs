using System;
using System.Windows;
using System.Windows.Media.Imaging;
using RestSharp;
using static TuyaDevelopment.DeviceInfoProp;

namespace TuyaDevelopment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            Hide();

            contextMenu1 = new System.Windows.Forms.ContextMenu();
            menuItem1 = new System.Windows.Forms.MenuItem();
            menuItem2 = new System.Windows.Forms.MenuItem();
            menuItem3 = new System.Windows.Forms.MenuItem();
            contextMenu1.MenuItems.AddRange(
                new System.Windows.Forms.MenuItem[] { menuItem1, menuItem2, menuItem3 });

            menuItem1.Index = 0;
            menuItem1.Text = "Switch";
            menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

            menuItem2.Index = 1;
            menuItem2.Text = "CountDown";
            menuItem2.Click += new System.EventHandler(this.menuItem3_Click);

            menuItem3.Index = 2;
            menuItem3.Text = "Exit";
            menuItem3.Click += new System.EventHandler(this.menuItem2_Click);

            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "You can control your light here";
            m_notifyIcon.BalloonTipTitle = "The Light App";
            m_notifyIcon.Text = "The Light";
            m_notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\bulb_off.ico");
            m_notifyIcon.DoubleClick += new EventHandler(m_notifyIcon_Click);
            m_notifyIcon.ContextMenu = contextMenu1;


            if (m_notifyIcon != null)
                m_notifyIcon.Visible = true;

            m_notifyIcon.ShowBalloonTip(1000);


            CheckLastState(DeviceInfoProp.device_id);
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            string token = Login();
            string t = MillisecondsTimestamp(DateTime.Now);
            string hash = Hash(client_id + token + t, secret);
            var client = new RestSharp.RestClient("https://openapi.tuyaeu.com/v1.0/devices/" + device_id + "/commands");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("client_id", client_id);
            request.AddHeader("access_token", token);
            request.AddHeader("sign", hash);
            request.AddHeader("t", t);
            request.AddHeader("sign_method", "HMAC-SHA256");
            request.AddHeader("Content-Type", "application/json");

            bool lastState = CheckLastState(DeviceInfoProp.device_id);

            if (lastState)
            {
                request.AddParameter("application/json", "{\n\t\"commands\":[\n\t\t{\n\t\t\t\"code\": \"switch_1\",\n\t\t\t\"value\":false\n\t\t}\n\t]\n}", ParameterType.RequestBody);
            }
            else
            {
                request.AddParameter("application/json", "{\n\t\"commands\":[\n\t\t{\n\t\t\t\"code\": \"switch_1\",\n\t\t\t\"value\":true\n\t\t}\n\t]\n}", ParameterType.RequestBody);
            }

            client.Execute(request);

            if (!lastState)
                m_notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\bulb_on.ico");
            else
            {
                m_notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\bulb_off.ico");
            }
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;

            Window.GetWindow(this).Close();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {

            Window window = new Window
            {
                Title = "Set CountDown",
                Content = new CountDownInsert()
            };
            Uri iconUri = new Uri("pack://application:,,,/Images/Bulb.ico", UriKind.RelativeOrAbsolute);
           
            window.Icon = BitmapFrame.Create(iconUri);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowInTaskbar = false;
            //window.WindowStyle = WindowStyle.None;
            window.Height = 110;
            window.Width = 180;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowDialog();
            return;
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            string token = Login();
            string t = MillisecondsTimestamp(DateTime.Now);
            string hash = Hash(client_id + token + t, secret);
            var client = new RestSharp.RestClient("https://openapi.tuyaeu.com/v1.0/devices/" + device_id + "/commands");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("client_id", client_id);
            request.AddHeader("access_token", token);
            request.AddHeader("sign", hash);
            request.AddHeader("t", t);
            request.AddHeader("sign_method", "HMAC-SHA256");
            request.AddHeader("Content-Type", "application/json");

            bool lastState = CheckLastState(DeviceInfoProp.device_id);

            if (lastState)
            {
                request.AddParameter("application/json", "{\n\t\"commands\":[\n\t\t{\n\t\t\t\"code\": \"switch_1\",\n\t\t\t\"value\":false\n\t\t}\n\t]\n}", ParameterType.RequestBody);
            }
            else
            {
                request.AddParameter("application/json", "{\n\t\"commands\":[\n\t\t{\n\t\t\t\"code\": \"switch_1\",\n\t\t\t\"value\":true\n\t\t}\n\t]\n}", ParameterType.RequestBody);
            }

            client.Execute(request);

            if (!lastState)
                m_notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\bulb_on.ico");
            else
            {
                m_notifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\bulb_off.ico");
            }
        }
    }
}
