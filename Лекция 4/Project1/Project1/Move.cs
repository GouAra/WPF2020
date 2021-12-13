using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
namespace Move
{
    class Movement
    {
        private const double g = 9.8;
        private double speedX; //X-компонента скорости
        private double speedY; //Y-компонента скорости
        private double angle; //начальный угол (радианы)

        private List<List<double>> cXYT = new List<List<double>>();
        private List<double> wind = new List<double>();

        public Movement(double s, double a)
        {
            this.angle = a * Math.PI / 180.0;
            speedX = s * Math.Cos(angle);
            speedY = s * Math.Sin(angle);
        }

        public void InitList()
        {
            this.cXYT.Add(new List<double>());
            this.cXYT.Add(new List<double>());
            this.cXYT.Add(new List<double>());
        }

        public double WindX(double a)
        {
            return Math.Sin(a) * 0.1;
        }

        public double WindY(double a)
        {
            return Math.Cos(a) * 0.1;
        }

        public void Calculate(double s, double a, double step, double m)
        {
            double x = 0, y = 0;
            
            StreamWriter f = new StreamWriter("..\\..\\test.txt");
            f.WriteLine("speed = {0} m/s, angle = {1} °, step = {2} s, mass = {3} kg", s, a, step, m);
            Console.WriteLine("speed = {0} m/s, angle = {1} °, step = {2} s, mass = {3} kg", s, a, step, m);
            for (double t = 0; true; t += step)
            {
                InitList();

                speedX = speedX - speedX * WindX(t) * t / m;
                speedY = speedY - (speedY * WindY(t) / m + g) * t;
                x = x + speedX * t;
                y = y + speedY * t;

                if (y < 0)
                {
                    cXYT[0].Add(x);
                    cXYT[1].Add(0);
                    cXYT[2].Add(t);
                    f.WriteLine("x = {0}, y = {1}, t = {2}", cXYT[0].Last(), cXYT[1].Last(), cXYT[2].Last());
                    f.Close();
                    break;
                }

                cXYT[0].Add(x);
                cXYT[1].Add(y);
                cXYT[2].Add(t);

                f.WriteLine("x = {0}, y = {1}, t = {2}", cXYT[0].Last(), cXYT[1].Last(), cXYT[2].Last());
                Console.WriteLine("x = {0}, y = {1}, t = {2}", cXYT[0].Last(), cXYT[1].Last(), cXYT[2].Last());
                t += step;
            }
                
            Console.WriteLine("Полёт окончен. Время полёта: {0} с. Дистанция: {1} м",
                    cXYT[2].Last(), cXYT[0].Last());
            f.Close();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            double s, a, st, m; //начальная скорость, начальный угол (градусы), шаг (с), масса снаряда
            s = Convert.ToDouble(Console.ReadLine());
            a = Convert.ToDouble(Console.ReadLine());
            st = Convert.ToDouble(Console.ReadLine());
            m = Convert.ToDouble(Console.ReadLine());
            Movement n = new Movement(s, a);
            n.Calculate(s, a, st, m);
            Thread.Sleep(50000);
        }
    }
}