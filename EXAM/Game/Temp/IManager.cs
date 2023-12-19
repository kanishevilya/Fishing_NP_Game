using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace classWork_9_10
{
    public interface IManager
    {
        List<IWorker> Workers { get; }
        void Organize();
        void MakeBudget();
        void Control();
    }
}
