using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using methods;

namespace homework7_8.Problem_01
{
    public class Square : GeometricFigure
    {
        private float aSide, bSide;

        public Square(float aSide, float bSide)
        {
            this.aSide = aSide;
            this.bSide = bSide;
        }
        public Square(bool isRand=false)
        {
            if (isRand)
            {
                this.aSide = alg.RAND(2, 12);
                this.bSide = alg.RAND(2, 12);
            }
            else
            {
                this.aSide = 0;
                this.bSide = 0;
            }
        }
        public override void calcPerimeter()
        {
            figurePerimeter=2*(aSide+bSide);
        }

        public override void calcSquare()
        {
            figureSquare = aSide * bSide;
        }

        public override void Input()
        {
            Console.WriteLine("(Square)");
            while(aSide<=0){aSide = alg.toFloatTryParse("Enter a first side for Square: ");}
            while (bSide <=0) { bSide = alg.toFloatTryParse("Enter a second side for Square: "); }
        }

        public override void Print()
        {
            Console.WriteLine("(Square)");
            PrintSides();
            base.Print();
        }
        public override void PrintSides()
        {
            Console.Write($"aSide = {aSide} | ");
            Console.WriteLine($"bSide = {bSide}");
        }

    }
}
