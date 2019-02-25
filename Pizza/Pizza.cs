using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza
{
    public class Pizza
    {

        private static int vColls = 0;
        private static int vRows = 0;
        
        public static int Rows { get; set; }
        public static int Colls { get; set; }
        public static int MinIngredientPerSlide { get; set; }
        public static int MaxCellsPerSlide { get; set; }
        public static char[,] PizzaCells { get; set; }

        private static Pizza _Pizza;
        public static Pizza TamañoPizza(string vLine) => _Pizza ?? (_Pizza = new Pizza(vLine));       

        public Pizza(string vLine)
        {
            Rows = Convert.ToInt32(vLine.Split(' ')[0]);
            Colls = Convert.ToInt32(vLine.Split(' ')[1]);
            MinIngredientPerSlide = Convert.ToInt32(vLine.Split(' ')[2]);
            MaxCellsPerSlide = Convert.ToInt32(vLine.Split(' ')[3]);
            PizzaCells = new char[Rows, Colls];
        }
        public static void ArmarPizza(string vLine)
        {
            foreach (var vChar in vLine.ToCharArray())
            {
                PizzaCells[vRows, vColls++] = vChar;
            }
            vColls = 0;
            vRows++; 
        }

        public enum Ingredientes
        {
            Tomates = 'T',
            Morrones = 'M'
        }
    }
}
