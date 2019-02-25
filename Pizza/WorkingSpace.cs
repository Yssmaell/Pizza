using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza
{
    static class WorkingSpace
    {
        public static void InputData(string vFilePath)
        {            
            if (File.Exists(vFilePath))
            {
                var vFirst = true;
                foreach (var line in File.ReadAllLines(vFilePath).Where(x=> !string.IsNullOrEmpty(x)))
                {
                    if (vFirst)
                    {
                        Pizza.TamañoPizza(line);
                        vFirst = false;
                        continue;
                    }
                    Pizza.ArmarPizza(line);
                }
            }
        }

        private static bool ValidarSlide<T>(this T[,] vSlide)
        {
            var vTomates = 0;
            var vMorrones = 0;
            foreach (var vCells in vSlide)
            {
                if ((Pizza.Ingredientes)(char)(object)vCells == Pizza.Ingredientes.Tomates)
                    vTomates++;
                if ((Pizza.Ingredientes)(char)(object)vCells == Pizza.Ingredientes.Morrones)
                    vMorrones++;
            }
            return vTomates >= Pizza.MinIngredientPerSlide && vMorrones >= Pizza.MinIngredientPerSlide;
        }
        public static void CutPizza()
        {            
            for (int y = 1; y <= Pizza.Colls; y++)
            {
                for (int x = 1; x <= Pizza.Rows; x++)
                {
                    if ((x) * (y) <= Pizza.MaxCellsPerSlide)
                    {
                        Debug.WriteLine("Submatriceando con X : " + x + " y Y : " + y);
                        var aux = ViewAsNByM2(Pizza.PizzaCells, x , y);
                        var puntaje = 0;
                        foreach (var items in aux)
                        {
                            var vQuery = from char item in items where item != '\0' select item;
                            puntaje += vQuery.Count();
                            //Debug.WriteLine("Send to debug output.");                            
                            foreach (var cells in items)
                            {                                
                                if (cells != '\0')
                                {
                                    var vIndex = CoordinatesOf(items, cells);
                                    items[vIndex.Item1, vIndex.Item2] = '\0';
                                    //Debug.WriteLine("Send to debug output.");
                                    Debug.WriteLine("Pizza comienza en: " + (vIndex.Item1) + " y " + (vIndex.Item2));
                                }                                
                            }
                            Debug.WriteLine("");
                        }
                        Debug.WriteLine("Score: " + puntaje);
                        Debug.WriteLine("");
                        Debug.WriteLine("");
                    }
                }
            }
            Debugger.Break();
        }

        public static IEnumerable<T[,]> ViewAsNByM2<T>(this T[,] values, int n, int m)
        //public static char[,] ViewAsNByM2<T>(this T[,] values, int n, int m)
        {
            int height = values.GetLength(0), width = values.GetLength(1);
            int rows = height / n, columns = width / m;

            for (int row = 0; row < rows; row++)
            {
                var vDesde = 0;
                for (int column = 0; column < columns; column++)
                {
                    var matrix = new T[height, width];                    
                    for (int i = 0; i < n; i++)
                    {
                        Array.ConstrainedCopy(
                            values,
                            (row * n + i) * width + column * m + vDesde,
                            matrix,
                            //i * m,
                            (row * n + i) * width + column * m + vDesde,
                            m); 
                        
                    }
                    if (ValidarSlide(matrix))
                    {
                        yield return matrix;
                    }
                    else
                    {
                        if ((decimal)columns < (decimal)width / m && vDesde == 0)
                        {
                            column--;
                            vDesde++;
                        }
                        else
                        {
                            vDesde = 0;
                        }
                    }
                }
            }
        }

        public static Tuple<int, int> CoordinatesOf<T>(this T[,] matrix, T value)
        {
            int w = matrix.GetLength(0); // width
            int h = matrix.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrix[x, y].Equals(value))
                        return Tuple.Create(x, y);
                    
                }
            }

            return Tuple.Create(-1, -1);
        }

    }    
}
