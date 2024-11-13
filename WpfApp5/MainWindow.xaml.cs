using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace WpfApp5
{
    public partial class MainWindow : Window
    {
        private List<Button> _buttons;
        private List<string> _cards;
        private Button _firstSelectedCard;
        private Button _secondSelectedCard;
        private int _moves;
        private DispatcherTimer _timer;
        private int _seconds;
        private bool _gameOver;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            _buttons = new List<Button>();
            _cards = new List<string>();
            _moves = 0;
            _seconds = 0;
            _gameOver = false;
            TimeLabel.Text = "Idő: 0 s";
            MovesLabel.Text = "Lépések: 0";

            _cards.AddRange(new List<string> { "A", "A", "B", "B", "C", "C", "D", "D", "E", "E", "F", "F", "G", "G", "H", "H" });
            ShuffleCards();

            CardGrid.Children.Clear();
            for (int i = 0; i < 16; i++)
            {
                var button = new Button
                {
                    Name = $"Card{i}",
                    Content = "?",
                    FontSize = 24,
                    Width = 80,
                    Height = 80,
                    Margin = new Thickness(5),
                    Background = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180)),
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(2),
                    FontFamily = new FontFamily("Arial"),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                button.Click += Card_Click;
                _buttons.Add(button);
                CardGrid.Children.Add(button);
            }

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_gameOver)
            {
                _seconds++;
                TimeLabel.Text = $"Idő: {_seconds} s";
            }
        }

        private void ShuffleCards()
        {
            var rand = new Random();
            _cards = _cards.OrderBy(x => rand.Next()).ToList();
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (_gameOver) return;

            var button = sender as Button;

            if (button.Content.ToString() != "?") return;

            int index = _buttons.IndexOf(button);
            button.Content = _cards[index];
            button.Background = new SolidColorBrush(Colors.White);

            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = button;
            }
            else if (_secondSelectedCard == null)
            {
                _secondSelectedCard = button;
                _moves++;
                MovesLabel.Text = $"Lépések: {_moves}";

                if (_firstSelectedCard.Content.ToString() == _secondSelectedCard.Content.ToString())
                {
                    _firstSelectedCard = null;
                    _secondSelectedCard = null;
                    CheckForGameOver();
                }
                else
                {
                    Task.Delay(500).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            _firstSelectedCard.Content = "?";
                            _secondSelectedCard.Content = "?";
                            _firstSelectedCard.Background = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
                            _secondSelectedCard.Background = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
                            _firstSelectedCard = null;
                            _secondSelectedCard = null;
                        });
                    });
                }
            }
        }

        private void CheckForGameOver()
        {
            if (_buttons.All(button => button.Content.ToString() != "?"))
            {
                _gameOver = true;
                _timer.Stop();
                MessageBox.Show($"Gratulálok! Készen vagy! Idő: {_seconds} s, Lépések: {_moves}");
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }
    }
}
