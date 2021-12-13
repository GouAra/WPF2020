using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Game
{
    public class MainW : Window
    {
        const double g = 9.8;
        const double step = 0.1;

        Point[] coordinates;
        TextBlock Start1, Exit, OK; //текст для кнопок
        TextBox Angle, Velocity, X0, Y0, Mass; //начальные условия: угол, скорость, координаты, масса
        StackPanel Stack1, Stack2;
        Canvas Canvas1, Canvas2, Canvas3;
        Polyline trajectory;
        PointCollection points;
        double xStart = 10, yStart = 300;

        bool subcondition1(string v)
        {
            int check = 0;
            if (v.Length == 0) return false; //должно быть заполнено
            for (int i = 0; i < v.Length; i++)
            {
                if (check > 1) return false; //должна быть только одна точка в записи
                if (v[i] == ' ') return false; //не должно быть пробелов в записи 
                if (!(v[i] >= '0' && v[i] <= '9'))
                    if (v[i] == '.') check++;
                    else return false;
            }//for
            return true;
        }//subcondition1

        bool subcondition2(string v)
        {
            int check = 0;
            if (v.Length == 0) return false; //должно быть заполнено
            if (v[0] == '0' && v.Length == 1) return false; //масса должна быть
            for (int i = 0; i < v.Length; i++)
            {
                if (check > 1) return false; //должна быть только одна точка в записи
                if (v[i] == ' ') return false; //не должно быть пробелов в записи 
                if (!(v[i] >= '0' && v[i] <= '9'))
                    if (v[i] == '.') check++;
                    else return false;
            }//for
            return true;
        }//subcondition1

        bool condition(string v, string a, string x, string y, string m) //обработка входных данных
        {
            if (subcondition1(v) && subcondition1(a) && subcondition1(x) && subcondition1(y) && subcondition2(m)) return true;
            else return false;
        }//condition

        double SpeedX(double v0, double angle) //скорость x
        {
            angle *= Math.PI / 180.0; //перевод в радианы
            return v0 * Math.Cos(angle);
        }//SpeedX

        double SpeedY(double v0, double angle) //скорость y
        {
            angle *= Math.PI / 180.0; //перевод в радианы
            return v0 * Math.Sin(angle);
        }//SpeedY

        double WindX(double angle) //скорость x ветра
        {
            angle *= Math.PI / 180.0;
            return Math.Sin(angle) * 0.1;
        }//WindX

        double WindY(double angle)//скорость y ветра
        {
            angle *= Math.PI / 180.0;
            return Math.Cos(angle) * 0.1;
        }//WindY

        double CalculateX(double v0, double angle, double x0, double m, double CurrentTime) //вычисление координаты Y
        {
            angle *= Math.PI / 180.0;
            double vx = SpeedX(v0, angle);
            vx = vx - vx * WindX(CurrentTime) * CurrentTime / m;
            return x0 + vx * CurrentTime;
        }//CalculateX

        double CalculateY(double v0, double angle, double y0, double m, double CurrentTime) //вычисление координаты Y
        {
            angle *= Math.PI / 180.0;
            double vy = SpeedY(v0, angle);
            vy = vy - (vy * WindY(CurrentTime) / m + g) * CurrentTime;
            return y0 + vy * CurrentTime;
        }//CalculateY

        int Quantity(double v0, double angle, double x0, double y0, double m) //вычисление количества координат для
        {
            int q = 0;
            double CurrentTime = 0;
            for (int i = 0; i < i + 1; i++)
            {
                if (CalculateY(v0, angle, y0, m, CurrentTime) <= 0) break; //снаряд упал на землю
                q++;
                CurrentTime += step;
            }//for
            return q;
        }//Quantity

        [STAThread]

        public static void Main()
        {
            Application app = new Application();
            app.Run(new MainW());
        }//Main

        public MainW()
        {
            Title = "Движение тела, брошенного под углом к горизонту";
            
            //размеры окна
            Width = 450;
            Height = 180;
            
            this.Background = new SolidColorBrush(Color.FromRgb(182, 225, 252)); //цвет фона
            
            Button Start1Btn = new Button(); //объявление и параметры кнопки Start на первом экране
            Start1 = new TextBlock();
            Start1.FontSize = 24;
            Start1.TextAlignment = TextAlignment.Center;
            Start1.Foreground = Brushes.Black;
            Start1.Inlines.Add(new Run("Начать"));
            Start1Btn.Content = Start1;
            Start1Btn.Margin = new Thickness(25);
            Start1Btn.Height = 50;
            Start1Btn.Width = 150;

            Button ExitBtn = new Button(); //объявление и параметры кнопки Exit на первом экране
            Exit = new TextBlock();
            Exit.FontSize = 24;
            Exit.TextAlignment = TextAlignment.Center;
            Exit.Foreground = Brushes.Black;
            Exit.Inlines.Add(new Run("Выйти"));
            ExitBtn.Content = Exit;
            ExitBtn.Margin = new Thickness(25);
            ExitBtn.Height = 50;
            ExitBtn.Width = 150;

            Stack1 = new StackPanel(); //объявление StackPanel на первом экране
            Stack1.HorizontalAlignment = HorizontalAlignment.Left;
            Stack1.VerticalAlignment = VerticalAlignment.Top;

            Grid W1Grid = new Grid(); //объявление и параметры Grid на первом экране
            W1Grid.Margin = new Thickness(25, 25, 25, 25);

            W1Grid.RowDefinitions.Add(new RowDefinition());
            W1Grid.ColumnDefinitions.Add(new ColumnDefinition());
            W1Grid.ColumnDefinitions.Add(new ColumnDefinition());

            W1Grid.Children.Add(Start1Btn);
            Grid.SetRow(Start1Btn, 0);
            Grid.SetColumn(Start1Btn, 0);

            W1Grid.Children.Add(ExitBtn);
            Grid.SetRow(ExitBtn, 0);
            Grid.SetColumn(ExitBtn, 1);

            Stack1.Children.Add(W1Grid);

            Canvas1 = new Canvas(); //объявление и параметры Canvas на первом экране
            Canvas1.Height = SystemParameters.VirtualScreenHeight;
            Canvas1.Width = SystemParameters.VirtualScreenWidth;
            Canvas1.Children.Add(Stack1);
            Content = Canvas1;

            ExitBtn.PreviewMouseLeftButtonDown += ExitBtnClicked;  //клик по кнопке Exit на первом экране
            void ExitBtnClicked(object sender, MouseButtonEventArgs e)
            {
                Close();
            }//ExitBtnClicked

            Start1Btn.PreviewMouseLeftButtonDown += Start1BtnClicked; //клик по кнопке Start на первом экране
            void Start1BtnClicked(object sender, MouseButtonEventArgs e)
            {
                Canvas1.Children.Clear(); //очистить экран

                //размеры окна
                Width = 350;
                Height = 275;

                this.Background = new ImageBrush(new BitmapImage(new Uri("https://b.radikal.ru/b41/1905/da/1871fa9b4329.jpg"))); //фон изображением

                Button OKBtn = new Button(); //объявление и параметры кнопки Пуск на втором экране
                OK = new TextBlock();
                OK.FontSize = 20;
                OK.Foreground = Brushes.Black;
                OK.TextAlignment = TextAlignment.Center;
                OK.Inlines.Add(new Run("OK"));
                OKBtn.Content = OK;
                OKBtn.Margin = new Thickness(2.5);
                OKBtn.Height = 30;
                OKBtn.Width = 100;

                TextBlock velocityText = new TextBlock(); //текст для TextBox для скорости на втором экране
                velocityText.Text = "V0";
                velocityText.HorizontalAlignment = HorizontalAlignment.Center;
                velocityText.FontSize = 16;
                velocityText.FontWeight = FontWeights.Bold;
                velocityText.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                velocityText.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                Velocity = new TextBox(); //объявление и параметры TextBox для скорости на втором экране
                Velocity.FontSize = 16;
                Velocity.Foreground = Brushes.Red;
                Velocity.TextAlignment = TextAlignment.Center;
                Velocity.Height = 30;
                Velocity.Width = 100;
                Velocity.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                TextBlock angleText = new TextBlock(); //текст для TextBox для угла на втором экране
                angleText.Text = "Angle";
                angleText.HorizontalAlignment = HorizontalAlignment.Center;
                angleText.FontSize = 16;
                angleText.FontWeight = FontWeights.Bold;
                angleText.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                angleText.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                Angle = new TextBox(); //объявление и параметры TextBox для угла на втором экране
                Angle.FontSize = 16;
                Angle.Foreground = Brushes.Red;
                Angle.TextAlignment = TextAlignment.Center;
                Angle.Height = 30;
                Angle.Width = 100;
                Angle.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                TextBlock x0Text = new TextBlock(); //текст для TextBox для X0 на втором экране
                x0Text.Text = "x0";
                x0Text.HorizontalAlignment = HorizontalAlignment.Center;
                x0Text.FontSize = 18;
                x0Text.FontWeight = FontWeights.Bold;
                x0Text.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                x0Text.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                X0 = new TextBox(); //объявление и параметры TextBox для X0 на втором экране
                X0.FontSize = 16;
                X0.Foreground = Brushes.Red;
                X0.TextAlignment = TextAlignment.Center;
                X0.Height = 30;
                X0.Width = 100;
                X0.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                TextBlock y0Text = new TextBlock(); //текст для TextBox для Y0 на втором экране
                y0Text.Text = "y0";
                y0Text.HorizontalAlignment = HorizontalAlignment.Center;
                y0Text.FontSize = 16;
                y0Text.FontWeight = FontWeights.Bold;
                y0Text.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                y0Text.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                Y0 = new TextBox(); //объявление и параметры TextBox для Y0 на втором экране
                Y0.FontSize = 16;
                Y0.Foreground = Brushes.Red;
                Y0.TextAlignment = TextAlignment.Center;
                Y0.Height = 30;
                Y0.Width = 100;
                Y0.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                TextBlock massText = new TextBlock(); //текст для TextBox для массы на втором экране
                massText.Text = "Mass";
                massText.HorizontalAlignment = HorizontalAlignment.Center;
                massText.FontSize = 16;
                massText.FontWeight = FontWeights.Bold;
                massText.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                massText.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                Mass = new TextBox(); //объявление и параметры TextBox для массы на втором экране
                Mass.FontSize = 16;
                Mass.Foreground = Brushes.Red;
                Mass.TextAlignment = TextAlignment.Center;
                Mass.Height = 30;
                Mass.Width = 100;
                Mass.Margin = new Thickness(2.5, 2.5, 2.5, 2.5);

                Stack2 = new StackPanel(); //объявление StackPanel на втором экране

                Grid W2Grid = new Grid(); //объявление и параметры Grid на втором экране
                W2Grid.Margin = new Thickness(25, 25, 25, 25);

                W2Grid.RowDefinitions.Add(new RowDefinition());
                W2Grid.RowDefinitions.Add(new RowDefinition());
                W2Grid.RowDefinitions.Add(new RowDefinition());
                W2Grid.RowDefinitions.Add(new RowDefinition());
                W2Grid.RowDefinitions.Add(new RowDefinition());
                W2Grid.ColumnDefinitions.Add(new ColumnDefinition());
                W2Grid.ColumnDefinitions.Add(new ColumnDefinition());
                W2Grid.ColumnDefinitions.Add(new ColumnDefinition());

                W2Grid.Children.Add(velocityText);
                Grid.SetRow(velocityText, 0);
                Grid.SetColumn(velocityText, 0);

                W2Grid.Children.Add(Velocity);
                Grid.SetRow(Velocity, 0);
                Grid.SetColumn(Velocity, 1);

                W2Grid.Children.Add(angleText);
                Grid.SetRow(angleText, 1);
                Grid.SetColumn(angleText, 0);

                W2Grid.Children.Add(Angle);
                Grid.SetRow(Angle, 1);
                Grid.SetColumn(Angle, 1);

                W2Grid.Children.Add(x0Text);
                Grid.SetRow(x0Text, 2);
                Grid.SetColumn(x0Text, 0);

                W2Grid.Children.Add(X0);
                Grid.SetRow(X0, 2);
                Grid.SetColumn(X0, 1);

                W2Grid.Children.Add(y0Text);
                Grid.SetRow(y0Text, 3);
                Grid.SetColumn(y0Text, 0);

                W2Grid.Children.Add(Y0);
                Grid.SetRow(Y0, 3);
                Grid.SetColumn(Y0, 1);

                W2Grid.Children.Add(massText);
                Grid.SetRow(massText, 4);
                Grid.SetColumn(massText, 0);

                W2Grid.Children.Add(Mass);
                Grid.SetRow(Mass, 4);
                Grid.SetColumn(Mass, 1);

                W2Grid.Children.Add(OKBtn);
                Grid.SetRow(OKBtn, 2);
                Grid.SetColumn(OKBtn, 2);

                Stack2.Children.Add(W2Grid);

                Canvas2 = new Canvas(); //объявление и параметры Canvas на втором экране
                Canvas2.Height = SystemParameters.VirtualScreenHeight;
                Canvas2.Width = SystemParameters.VirtualScreenWidth;
                Canvas2.Children.Add(Stack2);
                Content = Canvas2;
                MessageBox.Show("Введите начальную скорость, угол и начальные координаты (x, y)", "Примечание");

                OKBtn.PreviewMouseLeftButtonDown += OKBtnClicked; //клик по кнопке OK на втором экране

                void OKBtnClicked(object sender2, MouseButtonEventArgs e2)
                {
                    if (condition(Velocity.Text, Angle.Text, X0.Text, Y0.Text, Mass.Text) == true)//обработка входных данных
                    {
                        Canvas2.Children.Clear();

                        this.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); //цвет фона

                        double maxHeight = 0, maxLength = 0, maxTime = 0;

                        double InitialVelocity = System.Convert.ToDouble(Velocity.Text);
                        double InitialAngle = System.Convert.ToDouble(Angle.Text);
                        double InitialX0 = System.Convert.ToDouble(X0.Text);
                        double InitialY0 = System.Convert.ToDouble(Y0.Text);
                        double InitialMass = System.Convert.ToDouble(Mass.Text);

                        coordinates = new Point[Quantity(InitialVelocity, InitialAngle, InitialX0, InitialY0, InitialMass) + 1]; //объявление и заполнение массива координат
                        double CurrentTimeForPoints = 0;

                        for (int i = 0; i < coordinates.Length; i++)
                        {
                            if (i + 1 == coordinates.Length)
                            {
                                coordinates[i].X = CalculateX(InitialVelocity, InitialAngle, InitialX0, InitialY0, CurrentTimeForPoints);
                                coordinates[i].Y = 0;
                            }
                            else
                            {
                                coordinates[i].X = CalculateX(InitialVelocity, InitialAngle, InitialX0, InitialY0, CurrentTimeForPoints);
                                coordinates[i].Y = CalculateY(InitialVelocity, InitialAngle, InitialX0, InitialY0, CurrentTimeForPoints);
                            }//if

                            if (coordinates[i].X > maxLength) maxLength = coordinates[i].X;
                            if (coordinates[i].Y > maxHeight) maxHeight = coordinates[i].Y;
                            
                            CurrentTimeForPoints += step;
                        }//for

                        maxTime = CurrentTimeForPoints;

                        Width = 1000;
                        Height = 500;

                        this.Margin = new Thickness(20);
                        
                        NameScope.SetNameScope(this, new NameScope());
                        
                        Ellipse projectile = new Ellipse();
                        projectile.Width = 35;
                        projectile.Height = 30;
                        projectile.Fill = Brushes.DarkGray;
                        
                        TranslateTransform animatedTranslateTransform = new TranslateTransform();
                        
                        this.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);

                        projectile.RenderTransform = animatedTranslateTransform;
                        
                        Canvas3 = new Canvas();
                        Canvas3.Width = 1000;
                        Canvas3.Height = 500;
                        Canvas3.Children.Add(projectile);
                        Content = Canvas3;
                        
                        PathGeometry animationPath = new PathGeometry();
                        PathFigure pFigure = new PathFigure();
                        pFigure.StartPoint = new Point(xStart, yStart);
                        PolyBezierSegment pBezierSegment = new PolyBezierSegment();
                        for(int i = 0; i < coordinates.Length; i++)
                            pBezierSegment.Points.Add(new Point(xStart + coordinates[i].X * 50, yStart - coordinates[i].Y * 50));
                        pFigure.Segments.Add(pBezierSegment);
                        animationPath.Figures.Add(pFigure);
                        
                        animationPath.Freeze();
                        
                        DoubleAnimationUsingPath translateXAnimation = new DoubleAnimationUsingPath();
                        translateXAnimation.PathGeometry = animationPath;
                        translateXAnimation.Duration = TimeSpan.FromSeconds(maxTime);
                        
                        translateXAnimation.Source = PathAnimationSource.X;
                        
                        Storyboard.SetTargetName(translateXAnimation, "AnimatedTranslateTransform");
                        Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath(TranslateTransform.XProperty));
                        
                        DoubleAnimationUsingPath translateYAnimation = new DoubleAnimationUsingPath();
                        translateYAnimation.PathGeometry = animationPath;
                        translateYAnimation.Duration = TimeSpan.FromSeconds(maxTime);
                        
                        translateYAnimation.Source = PathAnimationSource.Y;
                        
                        Storyboard.SetTargetName(translateYAnimation, "AnimatedTranslateTransform");
                        Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath(TranslateTransform.YProperty));
                        
                        Storyboard pathAnimationStoryboard = new Storyboard();
                        pathAnimationStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                        pathAnimationStoryboard.Children.Add(translateXAnimation);
                        pathAnimationStoryboard.Children.Add(translateYAnimation);
                        
                        projectile.Loaded += delegate (object sender3, RoutedEventArgs e3)
                        {
                            pathAnimationStoryboard.Begin(this);
                        };
                    }
                    else
                    {
                        MessageBox.Show("Ошибка", "Данные неверны");
                    }//if
                }//OKBtnClicked
            }//StartBtnClicked
        }//MainW
    }//class MainW
}//Game