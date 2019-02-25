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
                for (int x = 2; x <= Pizza.Rows; x++)
                {
                    Debug.WriteLine("Submatriceando con X : " + x + " y Y : " + y);
                    FragmentarPizza(x, y, Pizza.PizzaCells);
                }
            }
            Debugger.Break();
        }

        private static void ReFragmentar(char[,] vMatriz)
        {
            for (int y = 1; y <= vMatriz.GetLength(1); y++)
            {
                for (int x = 2; x <= vMatriz.GetLength(0); x++)
                {
                    Debug.WriteLine("Subsubmatriceando con X : " + x + " y Y : " + y);
                    FragmentarPizza(x, y, vMatriz);
                }
            }
        }

        private static void FragmentarPizza(int CantFilas, int CantidadCol, char[,] vMatriz)
        {
            if ((CantFilas) * (CantidadCol) <= Pizza.MaxCellsPerSlide)
            {                
                var aux = ViewAsNByM2(vMatriz, CantFilas, CantidadCol);
                var puntaje = 0;
                foreach (var items in aux)
                {
                    var vQuery = from char item in items where item != '\0' select item;
                    puntaje += vQuery.Count();             
                    foreach (var cells in items)
                    {
                        if (cells != '\0')
                        {
                            var vIndex = CoordinatesOf(items, cells);
                            items[vIndex.Item1, vIndex.Item2] = '\0';
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
                    var vNewMatrix = new char[height, width];
                    for (int i = 0; i < n; i++)
                    {
                        Array.ConstrainedCopy(
                            values,
                            (row * n + i) * width + column * m + vDesde,
                            matrix,
                            (row * n + i) * width + column * m + vDesde,
                            m);

                    }
                    if (ValidarSlide(matrix))
                    {
                   //     vNewMatrix = RestarMatrices(values, matrix);
                 //       ReFragmentar(vNewMatrix);
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

        private static char[,] RestarMatrices<T>(this T[,] vM1, T[,] vM2)
        {
            int height = vM1.GetLength(0), width = vM1.GetLength(1);
            var vReturn = new char[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    vReturn[i, j] = (char)(Convert.ToInt32(vM1[i, j]) - Convert.ToInt32(vM2[i, j]));
                }           
            }
            return vReturn;
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
