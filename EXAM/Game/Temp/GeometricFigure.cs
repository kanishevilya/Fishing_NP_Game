using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using methods;

//Задание 1.
//Разработать абстрактный класс «Геометрическая Фигура» с методами «Площадь Фигуры» и «Периметр Фигуры»
//Разработать классы-наследники: Треугольник, Квадрат,
//Ромб, Прямоугольник, Параллелограмм, Трапеция, Круг,
//Эллипс Реализовать конструкторы, которые однозначно
//определяют объекты данных классов
//Реализовать класс «Составная Фигура», который
//может состоять из любого количества «Геометрических
//Фигур» Для данного класса определить метод нахождения
//площади фигуры Создать диаграмму взаимоотношений
//классов

namespace homework7_8.Problem_01
{

    public abstract class GeometricFigure
    {
        protected float figureSquare;
        protected float figurePerimeter;
 
        public float SquareGet { get { return figureSquare; } }
        public float PerimeterGet { get { return figurePerimeter; } }
        public abstract void calcSquare();
        public abstract void calcPerimeter();


        public abstract void Input();
        public virtual void Print()
        {
            Console.WriteLine($"Square   : {figureSquare}");
            Console.WriteLine($"Perimeter: {figurePerimeter}");
        }
        public abstract void PrintSides();
        public void AllActions()
        {
            Input();
            calcPerimeter();
            calcSquare();
            //Print();
        }
    }
}
