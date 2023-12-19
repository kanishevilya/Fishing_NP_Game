using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp
{
    public class Player : Character
    {
        public Player(string name, int age, List<Item> items) : base(name, age, items) { }
    }
}
