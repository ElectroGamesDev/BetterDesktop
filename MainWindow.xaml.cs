using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;
using Octokit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BetterDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string owner = "ElectroGamesYT";
        string repo = "BetterDesktopImages";
        string branch = "main";
        bool started = false;
        string settingsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), ".settings");
        public MainWindow()
        {
            InitializeComponent();
            CreateSettings();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string? category = GetSetting("Category").ToString();
            if (category != null)
            {
                Button? button = FindName(category.ToLower() + "Button") as Button;
                if (button != null && category != null && category != "None")
                {
                    HighlightSelectedCategory(button, true);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void homeBtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void homeBtnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            started = !started;

            if (started)
            {
                // popup messaging saying "Downloading images". When done say "You may now exit Better Desktop"

                

                //GithubDownloadImages(1);
            }
            else
            {
                // Popup button saying "Click here to reset desktop background"
            }
        }

        private void btnAnimalsCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(animalsButton);
            SetSetting("Category", "Animals");
        }

        private void btnNatureCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(natureButton);
            SetSetting("Category", "Nature");
        }

        private void btnTravelCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(travelButton);
            SetSetting("Category", "Travel");
        }

        private void btnAbstractCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(abstractButton);
            SetSetting("Category", "Abstract");
        }

        private void btnTechnologyCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(technologyButton);
            SetSetting("Category", "Technology");
        }

        private void btnMinimalCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(minimalButton);
            SetSetting("Category", "Minimal");
        }

        private void btnArtCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(artButton);
            SetSetting("Category", "Art");
        }

        private void btnInspirationalCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(inspirationalButton);
            SetSetting("Category", "Inspirational");
        }

        private void btnCustomCategory_Click(object sender, RoutedEventArgs e)
        {
            HighlightSelectedCategory(customButton);
            SetSetting("Category", "Custom");
        }

        private async void GithubDownloadImages(string type, int amount)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("BetterDesktop"));
            IReadOnlyList<RepositoryContent> contents = await client.Repository.Content.GetAllContents(owner, repo, type);
            string backgroundsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Backgrounds");

            if (contents != null)
            {
                Random random = new Random();
                List<int> allNumbers = Enumerable.Range(0, contents.Count).ToList();
                List<int> uniqueNumbers = new List<int>();

                for (int i = 0; i < amount; i++)
                {
                    int index = random.Next(0, allNumbers.Count);
                    allNumbers.RemoveAt(index);
                    uniqueNumbers.Add(allNumbers[index]);
                }

                foreach (int number in uniqueNumbers)
                {
                    byte[] imageContent = await client.Repository.Content.GetRawContent(owner, repo, contents[number].Path);
                    if (!Directory.Exists(backgroundsPath)) Directory.CreateDirectory(backgroundsPath);
                    File.WriteAllBytes(System.IO.Path.Combine(backgroundsPath, contents[number].Name), imageContent);
                }
            }
            else
            {
                // Category cannot be found or there is an issue with the internet connection
            }
        }

        private void HighlightSelectedCategory(Button button, bool highlightingCurrent = false)
        {
            string? currentCategory = GetSetting("Category").ToString();
            if (!highlightingCurrent && currentCategory != null && currentCategory != "None")
            {
                Button? btn = FindName(currentCategory.ToLower() + "Button") as Button;
                if (btn != null)
                {
                    Ellipse? eli = btn.Template.FindName("ellipse", btn) as Ellipse;
                    if (eli != null)
                    {
                        eli.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF962484"));
                        eli.StrokeThickness = 1.5f;
                    }
                }
            }
            Ellipse? ellipse = button.Template.FindName("ellipse", button) as Ellipse;
            if (ellipse != null)
            {
                ellipse.Stroke = new SolidColorBrush(Colors.Yellow);
                ellipse.StrokeThickness = 2.5f;
            }
        }

        private object? GetSetting(string key)
        {
            CreateSettings();
            Dictionary<string, object> deserializedData = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Deserialize<Dictionary<string, object>>(File.ReadAllText(settingsPath));
            if (deserializedData.TryGetValue(key, out object? value)) return value;
            else return null;
        }

        private void SetSetting(string key, object value)
        {
            CreateSettings();
            Dictionary<string, object> deserializedData = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Deserialize<Dictionary<string, object>>(File.ReadAllText(settingsPath));
            deserializedData[key] = value;
            string yaml = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Serialize(deserializedData);
            File.WriteAllText(settingsPath, yaml);
        }

        private void CreateSettings()
        {
            if (File.Exists(settingsPath)) return;
            //File.Create(settingsPath);

            Dictionary<string, object> settingsData = new Dictionary<string, object>
            {
                { "OldBackgroundPath", "None" },
                { "Category", "None" },
                { "CustomPath", "None" },
                { "Started", false },
                { "UpdateType", 0 }
            };

            string yaml = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Serialize(settingsData);
            File.WriteAllText(settingsPath, yaml);
        }

        private void SaveCurrentBackground()
        {

        }

        private string? GetCurrentBackground()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "Wallpaper", null) as string;
        }

        private string? SetBackground()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "Wallpaper", null) as string;
        }
    }
}
