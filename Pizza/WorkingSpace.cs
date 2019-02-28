using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pizza
{
    static class WorkingSpace
    {
        public static List<char[,]> lMatricesValidaz = new List<char[,]>();
        public static List<string> lCombinaciones = new List<string>();
        public static List<char[,]> lMatricesValidazAux = new List<char[,]>();
        public static Dictionary<List<char[,]>, int> dPosiblesListas = new Dictionary<List<char[,]>, int>();

        public static Dictionary<List<char[,]>, int> dFinal  = new Dictionary<List<char[,]>, int>();

        private static int vMaxValor = 0;
        public static void InputData(string vFilePath)
        {
            if (File.Exists(vFilePath))
            {
                var vFirst = true;
                foreach (var line in File.ReadAllLines(vFilePath).Where(x => !string.IsNullOrEmpty(x)))
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
                if ((char)(object)vCells == '\0') continue;
                if ((Pizza.Ingredientes)(char)(object)vCells == Pizza.Ingredientes.Tomates)
                    vTomates++;
                if ((Pizza.Ingredientes)(char)(object)vCells == Pizza.Ingredientes.Morrones)
                    vMorrones++;
            }
            return vTomates >= Pizza.MinIngredientPerSlide && vMorrones >= Pizza.MinIngredientPerSlide;
        }
        public static void CutPizza()
        {
            dFinal.Add(new List<char[,]>(), 0);
            for (int y = Pizza.Colls; 1 <= y; y--)
            {
                for (int x = Pizza.Rows; 1 <= x; x--)
                {
                    if (x == 1 && y == 1) continue;
                    dPosiblesListas = new Dictionary<List<char[,]>, int>();
                    Debug.WriteLine("Submatriceando con X : " + x + " y Y : " + y);
                    //Debug.WriteLine("Score: " + FragmentarPizza(x, y, Pizza.PizzaCells));
                    Debug.WriteLine("Score: " + NewFragmentar(x, y, Pizza.PizzaCells));
                    Debug.WriteLine("");
                    Debug.WriteLine("");
                }
            }
            Debug.WriteLine("");
            ImprimirDatos(dFinal);

        }

        public static string MatrizSalidaValid(List<char[,]> vM1 , int vNoIndex = -1)
        {            
            int height = vM1[0].GetLength(0), width = vM1[0].GetLength(1);
            var vReturn = new char[height, width];
            string vCombinaciones = "";
            bool Invalid = false;

            for (int index = 0; index < vM1.Count; index++)
            {
                if (index == vNoIndex) continue;
                Invalid = false;
                for (int i = 0; i < height; i++)
                {                    
                    for (int j = 0; j < width; j++)
                    {
                        if (vReturn[i, j] != vM1[index][i, j] && vM1[index][i, j] != '\0')
                            vReturn[i, j] += vM1[index][i, j];
                        else if (vM1[index][i, j] != '\0')
                        {
                            vReturn[i, j] += vM1[index][i, j];
                            Invalid = true;
                        }                            
                    }                    
                }
                if (Invalid)
                    vReturn = RestarMatrices(vReturn, vM1[index]);
                else
                    vCombinaciones += index + ",";
                
            }
            lCombinaciones.Add(vCombinaciones);
            if (-1 != vNoIndex) return vCombinaciones;
                foreach (var item in vCombinaciones.Split(',').Where(x=>!string.IsNullOrWhiteSpace(x)).OrderByDescending(x=> Convert.ToInt32(x)))
            {
                if (item == "0") break;
                MatrizSalidaValid(vM1, Convert.ToInt32(item));
            }

            return vCombinaciones;
        }

        public static void FusionarM(List<char[,]> lM)
        {
            lMatricesValidaz.Clear();
            bool vNew = false;
            for (int i = 0; i < lM.Count; i++)
            {
                vNew = false;
                char[,] vGroup;
                char[,] vGroupReadOnly = new char[lM[0].GetLength(0), lM[0].GetLength(1)];
                for (int j = i + 1; j < lM.Count; j++)
                {
                    vGroup = AgruparMatrices(lM[i], lM[j]);
                    if (!CompararMatrices(vGroup, vGroupReadOnly) && !vNew)
                    {
                        vNew = true;
                        //lM[i] = vGroup;
                        if (!lMatricesValidaz.Any(x => x == vGroup))
                            lMatricesValidaz.Add(vGroup);
                    }
                }
                if (!vNew)
                    if (!lMatricesValidaz.Any(x => x == lM[i]))
                        lMatricesValidaz.Add(lM[i]);
            }
            //lMatricesValidaz.AddRange(lM);
            if (lMatricesValidaz.Any())
                MatrizSalidaValid(lMatricesValidaz);
        }

        public static char[,] AgruparMatrices(char[,] vM1, char[,] vM2)
        {
            int height = vM1.GetLength(0), width = vM1.GetLength(1);
            var vReturn = new char[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if(vM1[i, j] == vM2[i, j])
                        vReturn[i, j] = vM1[i, j];
                }
            }

            if(CompararMatrices(vM1, vReturn))
                return vM2;
            else if(CompararMatrices(vM2, vReturn))
                return vM1;

            return new char[height, width];

        }

        private static void ReFragmentar(char[,] vMatriz)
        {
            for (int y = 1; y <= vMatriz.GetLength(1); y++)
            {
                //for (int y = 1; y <= vMatriz.GetLength(1); y++)
                for (int x = 1; x <= vMatriz.GetLength(0); x++)
                {
                    if (x == 1 && y == 1) continue;
                    if ((x) * (y) <= Pizza.MaxCellsPerSlide)
                    {
                        //var aux = ViewAsNByM2(vMatriz, x, y);
                        //Debug.WriteLine("Comienzo del comenzado anterior");
                        ViewAsNByM(vMatriz, x, y);
                        //foreach (var items in aux)
                        //{
                        //    Debug.WriteLine("Subsubmatriceando con X : " + x + " y Y : " + y);
                        //    var vQuery = from char item in items where item != '\0' select item;
                        //    puntaje += vQuery.Count();
                        //    foreach (var cells in items)
                        //    {
                        //        if (cells != '\0')
                        //        {
                        //            var vIndex = CoordinatesOf(items, cells);
                        //            items[vIndex.Item1, vIndex.Item2] = '\0';
                        //            Debug.WriteLine("Pizza comienza en: " + (vIndex.Item1) + " y " + (vIndex.Item2));
                        //        }
                        //    }
                        //    Debug.WriteLine(puntaje);
                        //}

                    }
                }
            }
        }

        private static int NewFragmentar(int CantFilas, int CantidadCol, char[,] vMatriz)
        {
            var puntaje = 0;
            if ((CantFilas) * (CantidadCol) <= Pizza.MaxCellsPerSlide)
            {
                //if (CantFilas == 3 && CantidadCol == 2) Debugger.Break();
                Debug.WriteLine("Comienzo nuevo");
                ViewAsNByM(vMatriz, CantFilas, CantidadCol);
                
                for (int i = 0; i < CantFilas; i++)
                {
                    lCombinaciones.Clear();
                    FusionarM(lMatricesValidazAux);
                    lMatricesValidazAux.Clear();
                    lMatricesValidazAux.AddRange(lMatricesValidaz);
                    foreach (var itemCombinacion in lCombinaciones)
                    {
                        var lForAdd = new List<char[,]>();
                        foreach (var item in itemCombinacion.Split(',').Where(x=> !string.IsNullOrWhiteSpace(x)))
                        {
                            for (int index = 0; index < lMatricesValidaz.Count; index++)
                            {
                                if (Convert.ToInt32(item) == index)
                                {
                                    lForAdd.Add(lMatricesValidaz[index]);
                                    break;
                                } 
                            }
                        }
                        dPosiblesListas.Add(new List<char[,]>(lForAdd), Puntaje(lForAdd));
                    }
                }
                if (dFinal.Any(x => x.Value <= vMaxValor))
                {                                   
                    foreach (var item in dPosiblesListas.Where(x => x.Value == vMaxValor).ToDictionary(k => k.Key,
                    k => k.Value))
                    {
                        dFinal.Clear();
                        dFinal.Add(item.Key, item.Value);
                    }
                    vMaxValor = 0;
                }

                ImprimirDatos(dPosiblesListas);

            }
            lMatricesValidaz.Clear();
            lMatricesValidazAux.Clear();
            return puntaje;
        }

        private static void ImprimirDatos(Dictionary<List<char[,]>, int> dLista)
        {
            foreach (var items in dLista)
            {
                Debug.WriteLine("Combinancion Posibles ");
                foreach (var item in items.Key)
                {
                    char[,] vTrabajo = new char[item.GetLength(0), item.GetLength(1)];
                    vTrabajo = (char[,])item.Clone();
                    foreach (var cells in vTrabajo)
                    {
                        if (cells != '\0')
                        {
                            var vIndex = CoordinatesOf(vTrabajo, cells);
                            vTrabajo[vIndex.Item1, vIndex.Item2] = 'X';
                            Debug.WriteLine("Pizza comienza en: " + (vIndex.Item1) + " y " + (vIndex.Item2));
                        }
                    }
                    Debug.WriteLine("");
                }
                Debug.WriteLine("Puntaje : " + items.Value);
            }
        }

        private static int Puntaje(List<char[,]> lM)
        {
            var puntaje = 0;
            foreach (var items in lM)
            {
                var vQuery = from char item in items where item != '\0' select item;
                puntaje += vQuery.Count();
            }
            if (vMaxValor < puntaje) vMaxValor = puntaje;
            return puntaje;
        }

        private static int FragmentarPizza(int CantFilas, int CantidadCol, char[,] vMatriz)
        {
            var puntaje = 0;
            if ((CantFilas) * (CantidadCol) <= Pizza.MaxCellsPerSlide)
            {
                var aux = ViewAsNByM2(vMatriz, CantFilas, CantidadCol);

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

            }
            return puntaje;
        }

        public static IEnumerable<T[,]> ViewAsNByM2<T>(this T[,] values, int n, int m)
        {
            int height = values.GetLength(0), width = values.GetLength(1);
            int rows = height / n, columns = width / m;
            //if (m == 5) Debugger.Break();
            for (int row = 0; row < rows; row++)
            {
                var vDesde = 0;
                for (int column = 0; column < columns; column++)
                {
                    var matrix = new T[height, width];
                    var matrixPrueba = new T[n, m];

                    for (int kk = 0; kk < n; kk++)
                    {
                        for (int jj = 0; jj < m; jj++)
                        {
                            matrixPrueba.SetValue('F', kk, jj);
                        }

                    }

                    var vNewMatrix = new char[height, width];
                    for (int i = 0; i < n; i++)
                    {
                        Array.ConstrainedCopy(
                            values,
                            (row * n + i) * width + column * m + vDesde,
                            matrixPrueba,
                            i * m,
                            m);
                        Array.ConstrainedCopy(
                            values,
                            (row * n + i) * width + column * m + vDesde,
                            matrix,
                            (row * n + i) * width + column * m + vDesde,
                            m);

                    }
                    if (ValidarSlide(matrix) && !MatrizWithNull(matrixPrueba))
                    {
                        if (!lMatricesValidazAux.Any(x => CompararMatrices(x, (char[,])(object)matrix)))
                            lMatricesValidazAux.Add((char[,])(object)matrix);
                        else
                            continue;

                        //vNewMatrix = RestarMatrices(values, matrix);
                        //new Thread(() =>ReFragmentar(vNewMatrix)).Start();
                        //ReFragmentar(vNewMatrix);
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

        public static void ViewAsNByM<T>(this T[,] values, int n, int m)
        {
            int height = values.GetLength(0), width = values.GetLength(1);
            int rows = height, columns = width ;
            //int rows = height / n, columns = width / m;

            
            for (int row = 0; row < rows; row++)
            {
                var vDesde = 0;
                for (int column = 0; column < columns; column++)
                {
                    var matrix = new T[height, width];
                    var matrixPrueba = new T[n, m];

                    for (int kk = 0; kk < n; kk++)
                    {
                        for (int jj = 0; jj < m; jj++)
                        {
                            matrixPrueba.SetValue('F', kk, jj);
                        }

                    }

                    var vNewMatrix = new char[height, width];
                    var vDesdeDatos = 0;
                    for (int i = 0; i < n; i++)
                    {
                        vDesdeDatos = (row * n + i) * width + column * m + vDesde;
                        if (vDesdeDatos >= width* height) break;
                        if (vDesdeDatos + m > width * height) break;
                        Array.ConstrainedCopy(
                            values,
                            vDesdeDatos,
                            matrixPrueba,
                            i * m,
                            m);
                        Array.ConstrainedCopy(
                            values,
                            vDesdeDatos,
                            matrix,
                            vDesdeDatos,
                            m);

                    }                    
                    if (ValidarSlide(matrix) && !MatrizWithNull(matrixPrueba) )
                    {                        
                        if (!lMatricesValidazAux.Any(x => CompararMatrices(x, (char[,])(object)matrix)))
                            lMatricesValidazAux.Add((char[,])(object)matrix);
                        else
                            continue;

                        vNewMatrix = RestarMatrices(values, matrix);
                        ReFragmentar(vNewMatrix);
                        return;
                        //               yield return matrix;
                    }
                    else
                    {
                        if ((decimal)columns < (decimal)width && vDesde == 0)
                        {
                            column--;
                            vDesde++;
                        }
                        else
                        {
                            column--;
                            vDesde++;
                        }
                    }
                }

            }
        }
        private static bool MatrizWithNull<T>(this T[,] vM1)
        {
            int height = vM1.GetLength(0), width = vM1.GetLength(1);
            var vReturn = new char[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if ((char)(Convert.ToInt32(vM1[i, j])) == '\0') return true;
                }
            }
            return false;
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

        private static bool CompararMatrices(char[,] vM1, char[,] vM2)
        {
            int height = vM1.GetLength(0), width = vM1.GetLength(1);
            var vReturn = new char[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (vM1[i, j] != vM2[i, j]) return false;
                }
            }
            return true;
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
 