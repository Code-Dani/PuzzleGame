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

namespace PuzzleGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String gridSize = "4x4";
        List<Image> cardPool;
        Image back_of_card;
        MediaPlayer player;
        DispatcherTimer timer;
        int seconds = 0;
        int minutes = 0;

        public MainWindow()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);

            player = new MediaPlayer();
            player.Open(new Uri(@"..\..\Music\track.wav", UriKind.Relative));

            defineVariables();
            InitializeComponent();
            player.Play();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds++;
            Console.WriteLine("entro");
            T_timer.Text = seconds.ToString();
            //T_timer.Text = TimeSpan.FromSeconds(seconds).ToString("mm\\:ss");
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
            playfield.Visibility = Visibility.Visible;
            playfield.IsEnabled = true;
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

        private void BT_apply_Click(object sender, RoutedEventArgs e)
        {
            Grid settings = G_settings;
            Grid main = G_mm;

            settings.Visibility = Visibility.Hidden;
            settings.IsEnabled = false;

            main.Visibility = Visibility.Visible;
            main.IsEnabled = true;
        }
    }
}
