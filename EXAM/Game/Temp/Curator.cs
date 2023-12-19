using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace classwork7_8
{
    public class Curator : Employee
    {
        public Curator(int id, string frstname, string lstname, double salary) : base(id, frstname, lstname, salary)
        {

        }

        public override void Print()
        {
            Console.WriteLine("I'm Cutor!");
        }
    }
}
