using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using methods;

namespace homework7_8.Problem_01
{
    public class Circle : GeometricFigure
    {
        private float radius;

        public Circle(float radius)
        {
            this.radius = radius;
        }
        public Circle(bool isRand = false)
        {
            if (isRand)
            {
                this.radius = alg.RAND(2, 12);
            }
            else
            {
                this.radius = 0;
            }
        }
        public override void calcPerimeter()
        {
            figurePerimeter = 2 * (float)Math.PI * radius;
        }

        public override void calcSquare()
        {
            figureSquare = (float)Math.PI*radius*radius;
        }

        public override void Input()
        {
            Console.WriteLine("(Circle)");
            while (radius <= 0) { radius = alg.toFloatTryParse("Enter a radius for Circle: "); }
        }

        public override void Print()
        {
            Console.WriteLine("(Circle)");
            PrintSides();
            base.Print();
        }
        public override void PrintSides()
        {
            Console.WriteLine($"Radius = {radius}");
        }

    }
}
