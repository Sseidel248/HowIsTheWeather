using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using HowsTheWeather.JSON_Response;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;


namespace HowsTheWeather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string apiKey;

        private string requestURL = "https://api.openweathermap.org/data/2.5/weather";

        public MainWindow()
        {
            InitializeComponent();

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            if (!LoadApiKey(filePath))
            {
                Environment.Exit(0);
            }

            UpdateData("Zepernick");
        }

        private void UpdateWeatherUI(object sender, RoutedEventArgs e)
        {
            string city = editCity.Text;
            UpdateData(city);
        }

        public void UpdateData(string city)
        {
            WeatherResponse weatherResponse = GetWeatherResponse(city);

            SetBkgImg(weatherResponse);
            if (weatherResponse.weather != null) { 
                labelTemp.Content = $"{weatherResponse.main.temp:F1}°C";
                labelDescr.Content = weatherResponse.weather[0].main;
            }
            else {
                labelTemp.Content = $"?°C";
                labelDescr.Content = "Unknown City";
            }

        }

        private void SetBkgImg(WeatherResponse weatherResponse)
        {
            string finalImg = "sun.png";
            if (weatherResponse.weather == null) { 
               finalImg = "Unknown_City.png"; 
            } 
            else {
                string currentWeather = weatherResponse.weather[0].main.ToLower();
                if (currentWeather.Contains("cloud"))
                {
                    finalImg = "Cloud.png";
                } else if (currentWeather.Contains("rain"))
                {
                    finalImg = "Rain.png";
                } else if (currentWeather.Contains("snow"))
                {
                    finalImg = "Snow.png";
                }
            }
            
            backgroungImg.ImageSource = new BitmapImage(new Uri($"Images/{finalImg}", UriKind.Relative));
        }

        private bool LoadApiKey(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    string content = File.ReadAllText(filename);
                    Setting appSetting = JsonConvert.DeserializeObject<Setting>(content);
                    if (appSetting != null) {
                        apiKey = appSetting.apiKey;
                    }
                }
                catch (JsonReaderException)
                {
                    MessageBox.Show($"Falsches Format oder fehlender API-Key innerhalb der Datei \"{filename}\"");
                    return false;
                }            
                return true;
            } else
            {
                MessageBox.Show($"Datei: \"{filename}\" existiert nicht.");
                return false;
            }

        }

        public WeatherResponse GetWeatherResponse(string city)
        {
            // Für die Http Request
            HttpClient client = new HttpClient();

            string finalURL = $"{requestURL}?q={city}&appid={apiKey}&units=metric";

            // Anfrage vom Web-Server abfragen und warten
            HttpResponseMessage response = client.GetAsync(finalURL).Result;
            
            // Anwort als String auslesen 
            string content = response.Content.ReadAsStringAsync().Result;

            // JSON String in einem Objekt konvertieren (NewtonSoft_JSON)
            WeatherResponse weatherReponse = JsonConvert.DeserializeObject<WeatherResponse>(content);

            return weatherReponse;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set Mouse Hover Color
            UpdateBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBEE6FD"));
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            // Set Button Color
            UpdateBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF0F0F0"));
        }

        private void editCity_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) { 
                string city = editCity.Text;
                UpdateData(city);
            }
        }
    }
}