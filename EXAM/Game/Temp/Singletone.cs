using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp
{
    public class Singletone<T>
    {
        private static bool onInit;
        
        public static T Instance;

        
        //public bool InInit { get; private set; }
        public static void Initialize(T instance) {
            if (!onInit)
            {
                onInit = true;
                Instance = instance;

                //AfterInitialize();
            }
        }
        //protected virtual static void AfterInitialize() { }

    }
}
