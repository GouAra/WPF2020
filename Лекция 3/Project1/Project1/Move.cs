using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace Move
{
    class Movement
    {
        private const double g = 9.8;
        private double speedX; //X-компонента скорости
        private double speedY; //Y-компонента скорости
        private double T; //общее время движения
        private double angle; //начальный угол (радианы)

        private List<List<double>> cXYT = new List<List<double>>();

        public Movement(double s, double a)
        {
            this.angle = a * Math.PI / 180.0;
            speedX = s * Math.Cos(angle);
            speedY = s * Math.Sin(angle);
            T = 2 * s * Math.Sin(angle) / g;
        }

        public void InitList()
        {
            this.cXYT.Add(new List<double>());
            this.cXYT.Add(new List<double>());
            this.cXYT.Add(new List<double>());
        }

        public void Calculate(double step)
        {
            double x = 0, y = 0;
            double t = 0;

            StreamWriter f = new StreamWriter("..\\..\\test.txt");
            
            do {
                InitList();
                x = speedX * t;
                y = speedY * t - g * Math.Pow(t, 2) / 2;
                if (y < 0 || t > T)
                {
                    cXYT[0].Add(x);
                    cXYT[1].Add(0);
                    cXYT[2].Add(T);
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
            while (t <= T);

            Console.WriteLine("Полёт окончен. Время полёта: {0} с. Дистанция: {1} м",
                    cXYT[2].Last(), cXYT[0].Last());
            f.Close();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            double s, a, st; //начальная скорость, начальный угол (градусы), шаг (с)
            s = Convert.ToDouble(Console.ReadLine());
            a = Convert.ToDouble(Console.ReadLine());
            st = Convert.ToDouble(Console.ReadLine());
            Movement m = new Movement(s, a);
            m.Calculate(st);
        }
    }
}