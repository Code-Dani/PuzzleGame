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
using System.Media;
using WMPLib;
using System.Windows.Threading;
using System.Timers;

namespace PuzzleGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String gridSize = "4x4";
        private List<Image> cardPool;
        private Image back_of_card;
        private MediaPlayer player;
        private Timer timer;
        private Stopwatch _stopwatch;
        

        public MainWindow()
        {
            timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            _stopwatch = new Stopwatch();

            player = new MediaPlayer();
            player.Open(new Uri(@"..\..\Music\track.wav", UriKind.Relative));

            defineVariables();
            InitializeComponent();
            player.Play();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                T_timer.Text = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
            });
        }

        public void defineVariables()
        {
            back_of_card = new Image
            {
                Source = new BitmapImage(new Uri("/Images/back.png", UriKind.Relative)),
                Width = 160,
                Height = 160,
                Stretch = Stretch.Fill,
                Margin = new Thickness(1, 1, 1, 1)
            };
            cardPool = new List<Image>();
            int count = 0;
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory() + "../../../Images/cards"))
            {
                cardPool.Add(new Image
                {
                    Source = new BitmapImage(new Uri("/Images/cards/"+ System.IO.Path.GetFileName(file), UriKind.Relative)), 
                    Width = 160,
                    Height = 160,
                    Stretch = Stretch.Fill,
                    Margin = new Thickness(1, 1, 1, 1),
                    Name = "test" + count.ToString(),
                    Tag = "dwokd"
                });
                count++;
            }
        }
        private void DSlink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://discord.gg/cHx4kNXpVa");
        }
        private void BT_play_Click(object sender, RoutedEventArgs e)
        {
            _stopwatch.Start();
            timer.Start();
            cardPool = cardPool.OrderBy(a => Guid.NewGuid()).ToList();  //scramble pool list
            cardPool = cardPool.OrderBy(a => Guid.NewGuid()).ToList();  //scramble pool list
            //Declarations
            Grid playfield = G_playfield;
            

            //creating field            TEMP - WILL BE MOVED INTO A NON SO CALLED METHOD
            int rows = int.Parse(gridSize.Split('x')[0]), columns = int.Parse(gridSize.Split('x')[1]), counter = 0;

            for (int i = 0; i < rows; i++)
            {
                var GRIDcolumns = new ColumnDefinition();
                GRIDcolumns.Width = new GridLength(150);
                playfield.ColumnDefinitions.Add(GRIDcolumns);
            }
            for (int j = 0; j < columns; j++)
            {
                var GRIDrows = new RowDefinition();
                GRIDrows.Height = new GridLength(150);
                playfield.RowDefinitions.Add(GRIDrows);
            }
            
            counter = 0; //counter reset

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var back = new Image
                    {
                        Source = new BitmapImage(new Uri("/Images/back.png", UriKind.Relative)),
                        Width = 160,
                        Height = 160,
                        Stretch = Stretch.Fill,
                        Margin = new Thickness(1, 1, 1, 1),
                        Name = "back" + counter,
                        Tag = counter.ToString()
                    };
                    back.MouseUp += Back_MouseUp;
                    cardPool[counter].Tag = counter.ToString();

                    playfield.Children.Add(back);
                    Grid.SetRow(back, i);
                    Grid.SetColumn(back, j);
                    counter++;
                }
            }

            //Setting up field
            G_contenitore.Visibility = Visibility.Visible;
            G_contenitore.IsEnabled = true;
            G_mm.Visibility = Visibility.Hidden;
            G_mm.IsEnabled = false;
            
        }
        private void Back_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            foreach(var item in cardPool)
            {
                Console.WriteLine(item.Tag);
            }

            foreach(var item in cardPool)
            {
                if(img.Tag.ToString() == item.Tag.ToString())
                {
                    if (img.Name.Contains("back"))
                    {
                        img.Source = item.Source;
                        img.Name = img.Name.Replace("back", "front");
                    }
                    else
                    {
                        img.Source = back_of_card.Source;
                        img.Name = img.Name.Replace("front", "back");
                    }
                }
            }
        }
        private void volumeBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = volumeBar.Value/100;
            player.Volume = value;
        }

        private void BT_settings_Click(object sender, RoutedEventArgs e)
        {
            Grid settings = G_settings;
            Grid main = G_mm;

            settings.Visibility = Visibility.Visible;
            settings.IsEnabled = true;

            main.Visibility = Visibility.Hidden;
            main.IsEnabled = false;
        }

        #region //Apply button
        private void BT_apply_Click(object sender, RoutedEventArgs e)
        {
            Grid settings = G_settings;
            Grid main = G_mm;

            settings.Visibility = Visibility.Hidden;
            settings.IsEnabled = false;

            main.Visibility = Visibility.Visible;
            main.IsEnabled = true;
        }
        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
            _stopwatch.Stop();
        }
    }
}
