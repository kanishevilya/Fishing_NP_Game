using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp
{
    public abstract class Character
    {
        private string name;
        private int age;

        private List<Item> items;
        private List<TypeActionsPlayer> actions;

        public string GetName { get => name; }
        public int GetAge { get => age; }

        public List<TypeActionsPlayer> GetActions { get => new List<TypeActionsPlayer>(actions); }
        public List<Item> GetItems { get => new List<Item>(items); }
       
        public Character(string name, int age, List<Item> items)
        {
            this.name = name;
            this.age = age;
            this.items = items;

            actions = new List<TypeActionsPlayer>();
            actions.Add(TypeActionsPlayer.ViewItems);
            actions.Add(TypeActionsPlayer.LookAround);
            Console.WriteLine($"I player name {name} age {age} items {items.Count} ");
        }
        public virtual string ToString()
        {
            return $"| {name} | {age} |";
        }
    }
}
