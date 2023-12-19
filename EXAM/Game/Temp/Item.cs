using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp
{
    public class Item
    {
        public string Name { get; private set; }
        public TypeItem Type { get; private set; }

        public Item(string name, TypeItem type)
        {
            Name = name;
            Type = type;
        }
    }

    public enum TypeItem
    {
        Clothe,
        Food,
        Potions,
    }
}
