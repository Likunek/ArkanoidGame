using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArkanoidGame
{
    public partial class GamePage : Page
    {
        string maincolor = "violet"; // основной цвет кнопок
        bool playerGoLeft = false;
        bool playerGoRight = false;
        bool gamePlay = true;

        public int levelnum = 1; // счетчик уровеней
        public int allPoints = 0;
        public int pointsLeft = 0;
        public int points = 0;
        public int complication = 0;

        public int hearts = 3; // количество жизней

        public Brick[,] bricks = new Brick[13, 21];

        const int tickRate = 10;
        Physics Physics = new Physics(tickRate);
        CartesianPosition CurrentPosition;

        DispatcherTimer gameTimer = new DispatcherTimer();
        Booster booster = new Booster();
        List<Ball> balls = new List<Ball>();

        // размеры игрового поля
        int height;
        int width;

        public GamePage() // запуск начального уровня
        {
            InitializeComponent();
            bricks = Tools.ReadLvl(levelnum);
            levelTB.Text = "Level " + levelnum;
            Game();
        }
        public GamePage(int level, int allpkt) // запуск следующего уровня
        {
            InitializeComponent();
            levelnum = level;
            allPoints = allpkt;
            levelTB.Text = "Level " + levelnum;
            points = 0;
            bricks = Tools.ReadLvl(levelnum);
            Game();
        }
        private void Game() // запуск самой игры
        {
            height = (int)myCanvas.Height;
            width = (int)myCanvas.Width;

            foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(x => x.Tag.ToString() == "ballEclipse"))
            {
                Ball ball = new Ball();
                ball.InitBall(x, 0);
                balls.Add(ball);
            }

            Brick.GenerateElements(ref myCanvas, ref bricks, width, height);
            myCanvas.Focus();
           
            pointsLabel.Content = "" + allPoints;
            heartsTextBlock.Text = "" + hearts;
            pointsLeft = Tools.PointsAtLevel;
            
            gameTimer.Interval = TimeSpan.FromMilliseconds(30);
            gameTimer.Tick += new EventHandler(GameTimerEvent);
            gameTimer.Start();
        }
       
        public void Next_Level() // запуск следующего уровня
        {
            gameTimer.Stop();
            levelnum++;
            if (levelnum%2 == 0) {

                complication++;
            }
            if (levelnum == 10)
            {
                MessageBox.Show("Вы выиграли!");
                NavigationService.Navigate(new MenuPage());
            }
            else
                NavigationService.Navigate(new GamePage(levelnum, allPoints));
        }

        public void RespawnBoost(int indexOfBall) // с вероятностью 10% выпадет бустер
        {
             if (Tools.RundomNumber(1, 10) == 5)
            {
                if (myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster").Count() == 0)
                    booster = new Booster(balls[indexOfBall], ref myCanvas, booster);
            }
        }

        public void SetBoost() // активировать бустер
        {
            if (booster.GetPower() != Power.None)
                powerIcon.Visibility = Visibility.Visible;  

            switch (booster.GetPower())
            {
                case Power.PlayerLenght:
                    booster.SetBoostPlayerLenght(ref player);
                    powerTextBlock.Text = "Player lenght";
                    powerIcon.Source = new BitmapImage(new Uri("/Images/boost-player-length.png", UriKind.Relative));
                    break;
                case Power.NewBall:
                    booster.NewBallSetBoost(ref myCanvas, ref balls);
                    powerTextBlock.Text = "New ball";
                    powerIcon.Source = new BitmapImage(new Uri("/Images/boost-add-ball.png", UriKind.Relative));
                    break;
                case Power.StrongerHit:
                    booster.SetPower(Power.StrongerHit);
                    powerTextBlock.Text = "Stronger hit";
                    powerIcon.Source = new BitmapImage(new Uri("/Images/boost-stronger-hit.png", UriKind.Relative));
                    break;
                case Power.None:
                    powerTextBlock.Text = "";
                    break;
                default:
                    break;
            }
        }
        public void StopBoost() // отключить бустер
        {
            powerIcon.Visibility = Visibility.Collapsed;

            if (booster.GetPower() != Power.None)
                powerIcon.Visibility = Visibility.Visible;

            switch (booster.GetPower())
            {
                case Power.PlayerLenght:
                    booster.StopBoostPlayerLenght(ref player);
                    break;
                case Power.NewBall:
                    break;
                case Power.StrongerHit:
                    booster.SetPower(Power.None);
                    break;               
                case Power.None:
                    break;
                default:
                    break;
            }
        }
        public bool PlayerCaughtABoost(Rect rect) // если платформа коснулась бустера, то активируем его
        {
            foreach (var g in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
            {
                Rect boosterEclipseHitBox = new Rect(Canvas.GetLeft(g), Canvas.GetTop(g), g.Width, g.Height);
                if (boosterEclipseHitBox.IntersectsWith(rect))
                {
                    myCanvas.Children.Remove(g);
                    StopBoost();
                    booster.RandomPower();
                    SetBoost();
                    return true;
                }
            }
            return false;
        }
        private void ChangeBallDirection(int index) // направление шарика
        {
            if (balls[index].posY <= 0)
            {
                balls[index].top = false;
            }
            if (balls[index].posX <= 0) balls[index].left = false; // лево
            if (balls[index].posX >= width - (balls[index].rad * 2)) balls[index].left = true;// право
        }
        public void HitBlock(int posX, int posY, Rectangle rectangle, int indexOfBall) // действия при касании блока
        {
            
            if (bricks[posX, posY].TimesToBreak < 2)
            {
                    myCanvas.Children.Remove(rectangle);

                    RespawnBoost(indexOfBall);

                    points += bricks[posX, posY].Value;
                    allPoints += bricks[posX, posY].Value;
                    pointsLabel.Content = "" + allPoints;

                    if (points == Tools.PointsAtLevel) Next_Level();
            }
            else
            {
                    bricks[posX, posY].TimesToBreak--;
                    return;
            }
        }
        private void BallMovement(int index, int goLeft = 0) // передвижение шарика
        {
            
            balls[index].posX = Canvas.GetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
            balls[index].posY = Canvas.GetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index));
            CurrentPosition = Physics.ExtractValue(balls[index].left, balls[index].top, balls[index].position, complication);

            if (balls[index].stop != true)
            {
                balls[index].posX += CurrentPosition.HorizontalPosition;
                balls[index].posY += CurrentPosition.VerticalPosition;
            }
            else 
                balls[index].posX += goLeft;

            Canvas.SetLeft(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posX);
            Canvas.SetTop(myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").ElementAt(index), balls[index].posY);

            ChangeBallDirection(index);
        }
        private void PlayerMovement(bool direction, int playerSpeed = 2) // передвижение игрока
        {
            for (int i = 0; i < playerSpeed; i++)
            {
                if (Canvas.GetLeft(player) + (player.Width) < width && direction)
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) + 1);
                    for (int j = 0; j < balls.Count; j++)
                    {
                        if (balls[j].stop == true)
                            BallMovement(j, 1);
                    }
                }
                if (Canvas.GetLeft(player) > 0 && !direction)
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) - 1);
                    for (int j = 0; j < balls.Count; j++)
                    {
                        if (balls[j].stop == true)
                            BallMovement(j, -1);
                    }
                }
            }
        }
        
        private void BoostMovement() // передвижение бустера
        {
            foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
            {
                booster.posX = (int)Canvas.GetLeft(x);
                booster.posY = (int)Canvas.GetTop(x);

                booster.posY += 1;

                Canvas.SetLeft(x, booster.posX);
                Canvas.SetTop(x, booster.posY);
            }
        }
        private void GameTimerEvent(object sender, EventArgs e)
        {
            try
            {
                OnLoseAllBalls();
  
                for (int i = 0; i < tickRate; i++)
                {
                    bool isTheSameBrick = false;
                    foreach (var x in myCanvas.Children.OfType<Rectangle>()) // столкновение с блоком
                    {
                        bool leave = false;
                        if (!isTheSameBrick && x.Name != "player") //если элемент блок
                        {
                            int posX = (int)Canvas.GetLeft(x) / (width / 13); 
                            int posY = (int)Canvas.GetTop(x) / (height / 26); 
                            Rect BlockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect ballEclipseHitBox;

                            foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index)))
                            {
                                leave = false;
                                ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);

                                if (!isTheSameBrick && ballEclipseHitBox.IntersectsWith(BlockHitBox))
                                {
                                    // верх блока
                                    if (balls[index].posY + balls[index].rad < Canvas.GetTop(x))
                                    {
                                        if (booster.GetPower() != Power.StrongerHit)
                                        {
                                            balls[index].top = true;
                                        }
                                       
                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // низ блока
                                    else if (balls[index].posY + balls[index].rad > Canvas.GetTop(x) + x.Height)
                                    {
                                        if (booster.GetPower() != Power.StrongerHit)
                                        {
                                            balls[index].top = false;
                                        }
                                        
                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // лево блока
                                    else if (balls[index].posX + balls[index].rad < Canvas.GetLeft(x))
                                    {
                                        if (booster.GetPower() != Power.StrongerHit)
                                        {
                                            balls[index].left = true;
                                        }  
                                        
                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }

                                    // право блока
                                    else if (balls[index].posX + balls[index].rad > Canvas.GetLeft(x) + x.Width)
                                    {
                                        if (booster.GetPower() != Power.StrongerHit)
                                        {
                                            balls[index].left = false;
                                        }                                       

                                        HitBlock(posX, posY, x, index);
                                        isTheSameBrick = true;
                                        leave = true;
                                    }                               
                                }
                                if (leave || isTheSameBrick)
                                    break;
                            }
                            if (leave)
                                break;
                        }
                        if (x.Name == "player") // если игрок, то отскок
                        {
                            Rect blockHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect ballEclipseHitBox;

                            foreach (var (ball, index) in myCanvas.Children.OfType<Ellipse>().Where(ball => ball.Tag.ToString() == "ballEclipse").Select((ball, index) => (ball, index)))
                            {
                                bool gotHit = false;
                                ballEclipseHitBox = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                                gotHit = Tools.CalculateTrajectory(blockHitBox, ballEclipseHitBox, x, ball, ref balls, index, tickRate);
                                if (!gotHit)
                                {
                                    for (int b = 0; b < myCanvas.Children.OfType<Ellipse>().Where(deletedBall => deletedBall.Tag.ToString() == "ballEclipse").Count(); b++)
                                    {
                                        balls.RemoveAt(b);
                                        myCanvas.Children.Remove(myCanvas.Children.OfType<Ellipse>().Where(deletedBall => deletedBall.Tag.ToString() == "ballEclipse").ElementAt(b));
                                    }
                                    return;

                                }
                            }
                            
                            if (PlayerCaughtABoost(blockHitBox)) { break; }
                        }
                        
                        if (leave)
                            break;
                    }

                    if (playerGoRight && !playerGoLeft)
                          PlayerMovement(true);
                    if (playerGoLeft && !playerGoRight)
                          PlayerMovement(false);

                    for (int j = 0; j < myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").Count(); j++)
                    {
                        BallMovement(j); 
                    }               

                    BoostMovement();

                    foreach (var (element, index) in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "ballEclipse").Select((element, index) => (element, index)))
                    {
                        if (Canvas.GetTop(element) > Canvas.GetTop(player))
                        {
                            myCanvas.Children.Remove(element);
                            balls.RemoveAt(index);
                            if (balls.Count == 0) return;
                            break;
                        }
                    }

                    foreach (var x in myCanvas.Children.OfType<Ellipse>().Where(element => element.Tag.ToString() == "Booster"))
                    {
                        if (Canvas.GetTop(x) > Canvas.GetTop(player))
                        {
                            myCanvas.Children.Remove(x);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "" + ex.StackTrace);
            }

        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }
        private void myCanvas_KeyDown(object sender, KeyEventArgs e) // при зажатой кнопке двигаемся
        {
            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = true;
                    break;
                case Key.Right:
                    e.Handled = true;
                    break;
                case Key.Up:
                    e.Handled = true;
                    break;
                case Key.Down:
                    e.Handled = true;
                    break;
                case Key.Tab:
                    e.Handled = true;
                    break;
                default:
                    break;
            }

            if (e.Key == Key.D) playerGoRight = true;

            if (e.Key == Key.A) playerGoLeft = true;

            if (e.Key == Key.Space)
                for (int i = 0; i < balls.Count; i++)
                    balls[i].stop = false;
        }
        private void myCanvas_KeyUp(object sender, KeyEventArgs e) // при разжатой нет
        {
            if (e.Key == Key.D) playerGoRight = false;
            if (e.Key == Key.A) playerGoLeft = false;
        }
        private void Button_MouseEvent(object sender, MouseEventArgs e) // при наведении на кнопку меняем ее на картинку другого цвета
        {
            (Application.Current.MainWindow as MainWindow).changeColors(sender as Button, maincolor);
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Image img = (Image)btn.Content;

            if (gamePlay)
            {
                gameTimer.Stop();
                gamePlay = false;
                myCanvas.Focus();
                img.Source = new BitmapImage(new Uri("Images/button-play-white.png", UriKind.Relative));
            }
            else
            {
                gameTimer.Start();
                gamePlay = true;
                myCanvas.Focus();
                img.Source = new BitmapImage(new Uri("Images/button-pause-white.png", UriKind.Relative));
            }
        }
       
        public void OnLoseAllBalls() // при потере шарика
        {
            heartsTextBlock.Text = "" + hearts;

            if (balls.Count == 0 && hearts == 1)
            {
                hearts--;
                heartsTextBlock.Text = "" + hearts;
                MessageBox.Show("Вы проиграли :(");
                gameTimer.Stop();
                NavigationService.Navigate(new MenuPage());
                return;
            }
            else if (balls.Count == 0 && hearts > 1)
            {
                Tools.SpawnBall(ref myCanvas, ref balls, player);
                hearts--;
            }
        }
    }
}
