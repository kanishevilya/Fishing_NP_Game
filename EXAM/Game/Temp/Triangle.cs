using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using methods;

namespace homework7_8.Problem_01
{
    public class Triangle : GeometricFigure
    {
        private float aSide, bSide, cSide;

        public Triangle(float aSide, float bSide, float cSide)
        {
            this.aSide = aSide;
            this.bSide = bSide;
            this.cSide = cSide;
        }
        public Triangle(bool isRand = false)
        {
            if (isRand)
            {
                this.aSide = alg.RAND(2, 12);
                this.bSide = alg.RAND(2, 12);
                this.cSide = alg.RAND(2, 12);
            }
            else
            {
                this.aSide = 0;
                this.bSide = 0;
                this.cSide = 0;
            }
        }
        public override void calcPerimeter()
        {
            figurePerimeter = aSide+bSide+cSide;
        }

        public override void calcSquare()
        {
            float p = (aSide + bSide + cSide) / 2;
            figureSquare = (float)Math.Sqrt(p*(p-aSide)*(p-bSide)*(p-cSide));
        }

        public override void Input()
        {
            Console.WriteLine("(Triangle)");
            while (aSide <= 0) { aSide = alg.toFloatTryParse("Enter a first side for Triangle: "); }
            while (bSide <= 0) { bSide = alg.toFloatTryParse("Enter a second side for Triangle: "); }
            while (cSide <= 0) { cSide = alg.toFloatTryParse("Enter a third side for Triangle: "); }
            if (aSide >= bSide + cSide || bSide >= aSide + cSide || cSide >= bSide + aSide)
            {
                Console.WriteLine("Одна сторона больше или равна сумме двух других!!!");
                aSide = 0;
                bSide = 0;
                cSide = 0;
                this.Input();
            }
        }

        public override void Print()
        {
            Console.WriteLine("(Triangle)");
            PrintSides();
            base.Print();
        }
        public override void PrintSides()
        {
            Console.Write($"aSide = {aSide} | ");
            Console.Write($"bSide = {bSide} | ");
            Console.WriteLine($"cSide = {cSide}");
        }

    }
}
