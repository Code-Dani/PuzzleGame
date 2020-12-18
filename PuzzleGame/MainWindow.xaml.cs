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
        private String gridSize = "4x4"; //default size if not modified
        private List<Image> cardPool;
        private Image back_of_card;
        private MediaPlayer player;
        private Timer timer;
        private Stopwatch _stopwatch;
        private List<Image> cardSet;
        private Image previousImage;
        private int scorePoints = 0;
        private int endCount = 1;

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
            //Declarations
            Grid playfield = G_playfield;
            

            //creating field            TEMP - WILL BE MOVED INTO A NON SO CALLED METHOD
            int rows = int.Parse(gridSize.Split('x')[0]), columns = int.Parse(gridSize.Split('x')[1]), counter = 0;
            int _cardsNeeded = (rows * columns) / 2;

            for (int i = 0; i < columns; i++)
            {
                var GRIDcolumns = new ColumnDefinition();
                GRIDcolumns.Width = new GridLength(150);
                playfield.ColumnDefinitions.Add(GRIDcolumns);
            }
            for (int j = 0; j < rows; j++)
            {
                var GRIDrows = new RowDefinition();
                GRIDrows.Height = new GridLength(150);
                playfield.RowDefinitions.Add(GRIDrows);
            }
            //initialize new card set made for this game
            cardSet = new List<Image>();
            int tmp = _cardsNeeded - 1;
            for (int i = 0; i < _cardsNeeded; i++)
            {
                //I'll simply add the firsts evertime since they get scrambled anyway at the start of the code
                cardPool[i].Tag = i;
                cardSet.Add(cardPool[i]);
                //now come the harder part... adding the double.. the thing is that C# binds every object, so if you try to simply add one more of that specific object it'll crash when adding it to the grid so we have to make a new object and trasfer the proprieties :(
                var tmpIm = new Image()
                {
                    Source = cardPool[i].Source,
                    Width = cardPool[i].Width,
                    Height = cardPool[i].Height,
                    Stretch = cardPool[i].Stretch,
                    Margin = cardPool[i].Margin,
                    Name = "test" + (tmp + 20),
                    Tag = cardPool[i].Tag
                };
                cardSet.Add(tmpIm);  //obviously this way they'll always end up one next to the other, but to make it simplier we'll just scramble it after
            }

            cardSet = cardSet.OrderBy(a => Guid.NewGuid()).ToList();

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
                    cardSet[counter].Tag = counter.ToString();

                    playfield.Children.Add(back);
                    Grid.SetRow(back, i);
                    Grid.SetColumn(back, j);
                    counter++;
                }
            }

            //Setting up field
            switchGridSettings(true, G_contenitore);
            switchGridSettings(false, G_mm);
        }
        private void Back_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            bool hit = false;

            foreach(var item in cardSet)
            {
                if(img.Tag.ToString() == item.Tag.ToString())
                {
                    if (img.Name.Contains("back"))
                    {
                        img.Source = item.Source;
                        img.Name = img.Name.Replace("back", "front");
                        hit = false;
                        if (previousImage != null)
                        {
                            if (img.Source == previousImage.Source)
                            {
                                img.Name = img.Name.Replace("front", "a");
                                previousImage.Name = previousImage.Name.Replace("front", "a");
                                previousImage = null;
                                scorePoints = scorePoints + 15;
                                T_score.Text = "Score: " + string.Format("{0:0}", ((scorePoints * 10) / _stopwatch.Elapsed.TotalSeconds));
                                if(((scorePoints * 10) / _stopwatch.Elapsed.TotalSeconds) >= 100)
                                {
                                    T_score.Foreground = Brushes.Gold;
                                }
                                if(((scorePoints * 10) / _stopwatch.Elapsed.TotalSeconds) >= 60 && ((scorePoints * 10) / _stopwatch.Elapsed.TotalSeconds) < 100)
                                {
                                    T_score.Foreground = Brushes.Green;
                                }
                                if (((scorePoints * 10) / _stopwatch.Elapsed.TotalSeconds) >= 30 && ((scorePoints * 10) / _stopwatch.Elapsed.TotalSeconds) < 60)
                                {
                                    T_score.Foreground = Brushes.Yellow;
                                }
                                if (((scorePoints * 10) / _stopwatch.Elapsed.TotalSeconds) >= 0 && ((scorePoints * 10) / _stopwatch.Elapsed.TotalSeconds) < 30)
                                {
                                    T_score.Foreground = Brushes.Red;
                                }

                                hit = true;
                                
                                if(endCount == cardSet.Count/2)
                                {
                                    _stopwatch.Stop();
                                    timer.Stop();
                                }
                                endCount++;
                            }
                            else
                            {
                                img.Source = back_of_card.Source;
                                img.Name = img.Name.Replace("front", "back");
                                previousImage.Source = back_of_card.Source;
                                previousImage.Name = previousImage.Name.Replace("front", "back");
                                previousImage = null;
                                hit = true;
                            }
                        }
                    }
                }
            }
            if (hit == false)
            {
                previousImage = img;
            }
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
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                T_timer.Text = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
            });
        }
        private void BT_settings_Click(object sender, RoutedEventArgs e)
        {
            switchGridSettings(true, G_settings);
            switchGridSettings(false, G_mm);
        }
        private void BT_gameModeSelection(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            if (bt.Content.Equals(BT_mode.Content))
            {
                switchGridSettings(false, G_mm);
                switchGridSettings(true, G_gamemode);
            }
            if (bt.Content.Equals(BT_apply_gamemode.Content))
            {
                switchGridSettings(false, G_gamemode);
                switchGridSettings(true, G_mm);
            }
            if (!bt.Content.Equals(BT_mode.Content) && !bt.Content.Equals(BT_apply_gamemode.Content))
            {
                switch (bt.Content)
                {
                    case "2x2":
                        gridSize = "2x2";
                        break;
                    case "3x4":
                        gridSize = "3x4";
                        break;
                    case "4x4":
                        gridSize = "4x4";
                        break;
                    default:
                        MessageBox.Show("Error while processing the request");
                        break;
                }
            }
        }
        public void switchGridSettings(bool isEnabled, Grid grid)
        {
            switch (isEnabled)
            {
                case true:
                    grid.Visibility = Visibility.Visible;
                    grid.IsEnabled = true;
                    break;
                case false:
                    grid.Visibility = Visibility.Hidden;
                    grid.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }
        private void volumeBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = volumeBar.Value / 100;
            player.Volume = value;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
            _stopwatch.Stop();
        }
    }
}
