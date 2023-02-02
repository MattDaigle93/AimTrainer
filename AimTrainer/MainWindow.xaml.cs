using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace AimTrainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();

        List<Ellipse> removeThis = new List<Ellipse>();

        int spawnRate = 60;
        int currentRate;
        int lastScore = 0;
        int health = 350;
        int posX;
        int posY;
        int score = 0;
        int misses = 0;

        double growthRate = 0.6;

        Random rand = new Random();

        MediaPlayer playClickSound = new MediaPlayer();
        MediaPlayer playerPopSound = new MediaPlayer();
        MediaPlayer playMissSound = new MediaPlayer();

        Uri ClickedSound;
        Uri PoppedSound;
        Uri MissSound;

        Brush brush;
        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            currentRate = spawnRate;

            ClickedSound = new Uri("pack://siteoforigin:,,,/sound/clickedpop.mp3");
            PoppedSound = new Uri("pack://siteoforigin:,,,/sound/pop.mp3");
            MissSound = new Uri("pack://siteoforigin:,,,/sound/miss.mp3");
        }

        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;
            txtLastScore.Content = "Last Score: " + lastScore;
            txtMisses.Content = "Misses: " + misses;

            currentRate -= 2;

            if (currentRate < 1)
            {
                currentRate = spawnRate;

                posX = rand.Next(15, 700);
                posY = rand.Next(50, 350);

                /* for bubble colors to be white */
                brush = new SolidColorBrush(Color.FromRgb(255,255,255));

                /* for bubble colors to be random */
                //brush = new SolidColorBrush(Color.FromRgb((byte)rand.Next(1, 255), (byte)rand.Next(1, 255), (byte)rand.Next(1, 255)));

                Ellipse circle = new Ellipse
                {
                    Tag = "circle",
                    Height = 70,
                    Width = 70,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = brush
                };

                Canvas.SetLeft(circle, posX);
                Canvas.SetTop(circle, posY);

                MyCanvas.Children.Add(circle);
            }

            foreach (var x in MyCanvas.Children.OfType<Ellipse>())
            {
                x.Height -= growthRate;
                x.Width -= growthRate;
                x.RenderTransformOrigin = new Point(0.5, 0.5);

                if (x.Width < 10)
                {
                    removeThis.Add(x);
                    health -= 15;
                    playerPopSound.Open(PoppedSound);
                    playerPopSound.Play();
                }
            }

            if (health > 1)
            {
                healthBar.Width = health;
            }
            else
            {
                GameOverFunction();
            }

            foreach (Ellipse i in removeThis)
            {
                MyCanvas.Children.Remove(i);
            }

            if (score > 5)
            {
                spawnRate = 25;
            }

            if (score > 50)
            {
                spawnRate = 15;
                growthRate = 1.5;
            }
        }

        private void ClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse)
            {
                Ellipse circle = (Ellipse)e.OriginalSource;

                MyCanvas.Children.Remove(circle);

                score++;

                playClickSound.Open(ClickedSound);
                playClickSound.Play();
            }
            else
            {
                misses++;
                health -= 15;

                playMissSound.Open(MissSound);
                playMissSound.Play();
            }
        }

        private void GameOverFunction()
        {
            gameTimer.Stop();

            MessageBox.Show("Game Over" + Environment.NewLine + "You Scored: " + score + Environment.NewLine + "Misses: " + misses + Environment.NewLine + "Click Ok to play again!", "Aim Trainer");

            foreach (var y in MyCanvas.Children.OfType<Ellipse>())
            {
                removeThis.Add(y);
            }
            foreach (Ellipse i in removeThis)
            {
                MyCanvas.Children.Remove(i);
            }

            growthRate = 0.6;
            spawnRate = 60;
            lastScore = score;
            score = 0;
            misses = 0;
            currentRate = 5;
            health = 350;
            removeThis.Clear();
            gameTimer.Start();
        }
    }
}
