using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace ArkanoidGame
{
    public static class Tools
    {
        private static Random rnd = new Random();
        public static string info;
        public static int PointsAtLevel = 0;
        public static int NumberOfBricks = 0;
        [XmlRoot("XMLBricks")]
        public class ListBricks
        {
            public ListBricks() { Bricks = new List<XMLBrick>(); }

            [XmlElement("XMLBrick")]
            public List<XMLBrick> Bricks { get; set; }
        }

        public static ListBricks listBricks = new ListBricks();
        public static Brick[,] ReadLvl(int lvl) => JustReadLvl("../../Levels", "lvl_" + lvl.ToString());
        private static Brick[,] JustReadLvl(string path, string lvl)
        {
            Brick[,] Bricks = new Brick[13, 21];
            int levelPoints = 0;
            info = "";  
            var serializer = new XmlSerializer(typeof(ListBricks));
            using (var reader = XmlReader.Create($@"{path}\{lvl}.xml"))
            {
                listBricks = (ListBricks)serializer.Deserialize(reader);
                reader.Close();
                reader.Dispose();
            }
            foreach (var Brick in listBricks.Bricks)
            {
                levelPoints += Brick.Value;
                info += "Brick: x" + Brick.PosX + ", y" + Brick.PosY + " ttb:> " + Brick.TimesToBreak + "\n";
                switch (Brick.Type)
                {
                    case 1:
                        Bricks[Brick.PosX, Brick.PosY] = new SimpleBrick(Brick.Color, Brick.Value);
                        break;
                    case 2:
                        Bricks[Brick.PosX, Brick.PosY] = new SilverBrick(Brick.Value, Brick.TimesToBreak);
                        break;
                    case 3:
                        Bricks[Brick.PosX, Brick.PosY] = new GoldBrick(Brick.Value, Brick.TimesToBreak);
                        break;
                    default: break;
                }
            }
            PointsAtLevel = levelPoints;
            return Bricks;
        }
        public static void SpawnBall(ref Canvas myCanvas, ref List<Ball> balls, Rectangle rectangle)
        {
            Ellipse ballEclipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000ff")),
                Tag = "ballEclipse",
            };

            myCanvas.Children.Add(ballEclipse);
            Canvas.SetTop(ballEclipse, Canvas.GetTop(rectangle) - ballEclipse.Height);
            Canvas.SetLeft(ballEclipse, Canvas.GetLeft(rectangle) + (rectangle.Width / 2) - (ballEclipse.Width / 2));

            Ball ball = new Ball();
            ball.InitBall(ballEclipse, 0);
            ball.top = true;
            balls.Add(ball);
        }
        public static int RundomNumber(int from, int to)
        {
            return rnd.Next(from, to + 1);
        }
        public static bool CalculateTrajectory(Rect blockHitBox, Rect ballEclipseHitBox, Rectangle x, Ellipse ball, ref List<Ball> balls, int index, int tickRate) // куда шарик отскочит
        {
            if (ballEclipseHitBox.IntersectsWith(blockHitBox))
            {
                if (Canvas.GetLeft(x) < Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 1;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 2 > Canvas.GetLeft(ball) + ball.Width / 2)
                {                   
                    balls[index].top = true;
                    balls[index].left = true;                
                    balls[index].position = 2;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 2 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 3 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;                    
                    balls[index].position = 3;
                    
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 3 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 4 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;                 
                    balls[index].position = 4;
                    
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 4 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 5 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;                  
                    balls[index].position = 5;
                    
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 5 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 6 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 6;
                    
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 6 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 7 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;                   
                    balls[index].position = 7;
                
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 7 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 8 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 8;
              
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 8 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 9 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;          
                    balls[index].position = 9;
             
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 9 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 10 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 10;

                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 10 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 11 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 11;

                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 11 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 12 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 12;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 12 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 13 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 13;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 13 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 14 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 14;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 14 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 15 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 15;

                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 15 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 16 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 16;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 16 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 17 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 17;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 17 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 18 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 18;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 18 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 19 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 19;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 19 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 20 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = true;
                    balls[index].position = 19;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 20 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 21 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 19;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 21 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 22 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 19;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 22 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 23 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 18;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 23 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 24 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 17;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 24 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 25 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 16;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 25 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 26 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 15;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 26 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 27 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 14;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 27 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 28 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 13;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 28 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 29 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 12;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 29 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 30 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 11;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 30 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 31 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 10;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 31 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 32 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 9;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 32 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 33 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 8;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 33 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 34 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 7;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 34 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 35 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 6;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 35 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 36 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 5;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 36 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 37 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 4;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 37 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 38 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 3;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 38 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 39 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 2;
                }
                else if (Canvas.GetLeft(x) + (x.Width / 40) * 39 <= Canvas.GetLeft(ball) + ball.Width / 2 && Canvas.GetLeft(x) + (x.Width / 40) * 40 > Canvas.GetLeft(ball) + ball.Width / 2)
                {
                    balls[index].top = true;
                    balls[index].left = false;
                    balls[index].position = 1;
                }
            }
            return true;
        }
       
    }
}