using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using methods;

namespace classwork7_8
{
    public class Employee: Human
    {
        private double salary;

        public Employee(int id, string frstname, string lstname, double salary) : base(id, frstname, lstname)
        {
            this.salary = salary;
            
        }
        public override void DoAction()
        {
            Console.WriteLine("Action employee");
        }
        public override void Print()
        {
            base.Print();
            Console.WriteLine($"Salary = {salary}");
        }
        public override string ToString()
        {
            return $"Salary = {salary}";
        }
    }
}
