using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RestSharp;
using static TuyaDevelopment.DeviceInfoProp;

namespace TuyaDevelopment
{
    /// <summary>
    /// Set Countdown for your switch
    /// </summary>
    public partial class CountDownInsert : UserControl
    {
        public CountDownInsert()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            string tip = "";

            if(tbSure.Text.Length == 0)
                MessageBox.Show("Enter Countdown");

            int _time = Convert.ToInt32(tbSure.Text);
            int time = 0;

            if ((BtnSn.IsChecked == true))
            {
                time = _time;
                tip = "Second";
            }
            else if (BtnDk.IsChecked == true)
            {
                time = _time * 60;
                tip = "Minute";
            }
            else if (BtnSaat.IsChecked == true)
            {
                time = _time * 3600;
                tip = "Hour";
            }
            else
            {
                MessageBox.Show("You didnt choose time !");
            }

            

            string token = DeviceInfoProp.Login();
            string t = MillisecondsTimestamp(DateTime.Now);
            string hash = Hash(DeviceInfoProp.client_id + token + t, DeviceInfoProp.secret);
            var client = new RestSharp.RestClient("https://openapi.tuyaeu.com/v1.0/devices/" + DeviceInfoProp.device_id + "/commands");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("client_id", DeviceInfoProp.client_id);
            request.AddHeader("access_token", token);
            request.AddHeader("sign", hash);
            request.AddHeader("t", t);
            request.AddHeader("sign_method", "HMAC-SHA256");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\t\"commands\":[\n\t\t{\n\t\t\t\"code\": \"countdown_1\",\n\t\t\t\"value\":"+time+"\n\t\t}\n\t]\n}", ParameterType.RequestBody);

            client.Execute(request);

            bool lastState = CheckLastState(DeviceInfoProp.device_id);

            if (lastState)
                m_notifyIcon.BalloonTipText = "Light will turn off after "+ _time + " "+tip;
            else
                m_notifyIcon.BalloonTipText = "Light will turn on after " + _time + " "+tip;

            m_notifyIcon.BalloonTipTitle = "The Light App";
            m_notifyIcon.ShowBalloonTip(1000);

            Window.GetWindow(this).Close();
        }

        private void SadeceRakamVeVirgul(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2
                || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9
                || e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5
                || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 
                || e.Key == Key.Right || e.Key == Key.Left)
            {

                e.Handled = false;
            }
            else
            {

                e.Handled = true;
            }
        }
    }
}
