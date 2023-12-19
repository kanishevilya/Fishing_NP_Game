using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace classWork_9_10
{
    public class Director : Employee, IManager
    {
        private bool isWorkin = true;
        public List<IWorker> Workers { get; set; }

        public Director(string firstName, string secondName, DateTime birthDay, string position, int salary) : base(position, salary, firstName, secondName, birthDay)
        {
            Workers = new List<IWorker>();
        }


        public void Control()
        {
            Console.WriteLine("Контролирую работу");
        }

        public void MakeBudget()
        {
            Console.WriteLine("Формирую бюджет");
        }

        public void Organize()
        {
            Console.WriteLine("Организую работу");
        }
    }
    public class Seller: Employee, IWorker
    {
        private bool isWorkin = true;

        public Seller(string firstName, string secondName, DateTime birthDay, string position, int salary) : base(position, salary, firstName, secondName, birthDay)
        {
        }

        public bool IsWorkin { get => isWorkin; }

        public event EventHandler WorkEnded;

        public string Work()
        {
            return "Подаю товары";
        }
    }
    public class Cashier : Employee, IWorker
    {
        private bool isWorkin = true;

        public Cashier(string firstName, string secondName, DateTime birthDay, string position, int salary) : base(position, salary, firstName, secondName, birthDay)
        {
            isWorkin = false;
        }

        public bool IsWorkin { get => isWorkin; }

        public event EventHandler WorkEnded;

        public string Work()
        {
            return "Принимаю оплату";
        }
        public void goToHome()
        {
            Console.WriteLine("Иду Домой");
        }
    }
    public class StoreKeeper : Employee, IWorker
    {
        private bool isWorkin = true;

        public StoreKeeper(string firstName, string secondName, DateTime birthDay, string position, int salary) : base(position, salary, firstName, secondName, birthDay)
        {
        }

        public bool IsWorkin { 
            get 
            {
                if (FirstName == "Bob")
                {
                    return isWorkin;
                }
                return false;
            } 
        }

        public event EventHandler WorkEnded;

        public string Work()
        {
            return "Учитываю товар";
        }
        
    }
}
