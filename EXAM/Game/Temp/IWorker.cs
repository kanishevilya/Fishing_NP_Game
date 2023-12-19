using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace classWork_9_10
{
    public interface IWorker : IHuman
    {
        event EventHandler WorkEnded;

        bool IsWorkin { get; }

        string Work();

        public void goToHome(bool isNight);
    }
    public interface IHuman
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime BirthDay { get; set; }

        public void goToHome();
    }
}
