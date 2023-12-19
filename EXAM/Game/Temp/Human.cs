using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace classWork_9_10
{
    public abstract class Human: IHuman
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime BirthDay { get; set; }

        protected Human(string firstName, string secondName, DateTime birthDay)
        {
            FirstName = firstName;
            SecondName = secondName;
            BirthDay = birthDay;
        }

        public override string ToString()
        {
            return $"{FirstName} {SecondName}";
        }

        public void goToHome()
        {
            throw new NotImplementedException();
        }
    }
    public abstract class Employee : IHuman
    {
        public string Position { get; set; }
        public int Salary { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime BirthDay { get; set; }

        protected Employee(string position, int salary, string firstName, string secondName, DateTime birthDay) 
        {

            Position = position;
            Salary = salary;
            FirstName = firstName;
            SecondName = secondName;
            BirthDay = birthDay;
        }

        public override string ToString()
        {
            return base.ToString() + $"{Position} | {Salary}";
        }

        public void goToHome()
        {
            Console.WriteLine("Еду домой на автобусе");
        }

        public void goToHome(bool isNight)
        {
            Console.WriteLine("Еду домой на такси");
        }

        public int Compare(object? x, object? y)
        {
            if(x is Human && y is Human)
            {
                Human first = x as Human;
                Human second = y as Human;

                return first.FirstName.CompareTo(second.FirstName);
            }
            return 0;
        }
    }
}
