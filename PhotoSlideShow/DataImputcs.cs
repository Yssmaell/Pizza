using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSlideShow
{
    public static class DataImputcs
    {
        public static Dictionary<List<string>, string > dPhotoTags = new Dictionary<List<string> , string>();
        public static Dictionary<List<string>, string> dPhotoTagsAux = new Dictionary<List<string>, string>();
        public static Dictionary<string, Dictionary<List<string>, string>> dSlide = new Dictionary<string, Dictionary<List<string>, string>>();

        public static void InputData(string vFilePath)
        {
            if (File.Exists(vFilePath))
            {
                var vFirst = true;
                var i = 0;
                foreach (var line in File.ReadAllLines(vFilePath).Where(x => !string.IsNullOrEmpty(x)))
                {
                    if (vFirst)
                    {
                        vFirst = false;
                        continue;
                    }
                    var lTags = new List<string>();
                    lTags.AddRange(line.Split(' '));
                    var vKey = lTags[0] + "-" + lTags[1];
                    lTags.RemoveRange(0, 2);
                    dPhotoTags.Add(lTags, vKey);                                        
                }
            }
        }


        public static void Combinar(Dictionary<List<string>, string> dThis, bool vBuscar = false)
        {
            Dictionary<List<string>, string> dNuevo = new Dictionary<List<string>, string>();
            List<string> lIzquierda, lDerecha, lIntersect, lVerticales = new List<string>();
            var score = "";

            dThis = dThis.OrderByDescending(x => x.Key.Count).ToDictionary(k => k.Key,
                    k => k.Value);

            for (int i = 0; i < dThis.Count; i++)
            {
                if (dPhotoTagsAux.Any(x => x.Value.Split('-')[1].Split(' ').Any(K => Convert.ToInt64(K)  == Convert.ToInt64(dThis.ElementAt(i).Value.Split('-')[1])))) continue;
                if (!dThis.ElementAt(i).Value.Contains("V"))
                for (int j = i + 1; j < dThis.Count; j++)
                {
                    lIzquierda = dThis.ElementAt(i).Key.Except(dThis.ElementAt(j).Key).ToList();
                    lDerecha = dThis.ElementAt(j).Key.Except(dThis.ElementAt(i).Key).ToList();
                    lIntersect = dThis.ElementAt(j).Key.Intersect(dThis.ElementAt(i).Key).ToList();
                    score = lIzquierda.Count + "-" + lIntersect.Count + "-" + lDerecha.Count;
                    if (lIntersect.Any())
                    {
                        if (dThis.ElementAt(i).Value.Contains("H"))
                        {
                            dNuevo.Add(lIntersect, score);
                            if (!dPhotoTagsAux.Any(x => x.Key == dThis.ElementAt(i).Key && x.Value == "H-" + i))
                                dPhotoTagsAux.Add(dThis.ElementAt(i).Key, "H-" +i);
                            if (!dPhotoTagsAux.Any(x => x.Key == dThis.ElementAt(j).Key && x.Value == "H-" + j))
                                dPhotoTagsAux.Add(dThis.ElementAt(j).Key, "H-" +j);
                            break;
                        }
                        else
                        {
                            for (int k = j + 1; k < dThis.Count; k++)
                            {
                                if (dThis.ElementAt(k).Value.Contains("V"))
                                {
                                    lIzquierda = dThis.ElementAt(j).Key.Except(dThis.ElementAt(k).Key).ToList();
                                    lDerecha = dThis.ElementAt(k).Key.Except(dThis.ElementAt(j).Key).ToList();
                                    lIntersect = dThis.ElementAt(k).Key.Intersect(dThis.ElementAt(j).Key).ToList();
                                    if (lIntersect.Any())
                                    {
                                        lVerticales.AddRange(lIzquierda);
                                        lVerticales.AddRange(lDerecha);
                                        lVerticales.AddRange(lIntersect);
                                        if (!dPhotoTagsAux.Any(x => x.Key == lVerticales && x.Value == "H-" + j + " " + k))
                                        {
                                            dPhotoTagsAux.Add(lVerticales, "H-" + j + " " + k);
                                            break;
                                        }
                                    }
                                }
                            }
                        }                   
                    }                    
                }
                else
                {
                    for (int k = i + 1; k < dThis.Count; k++)
                    {
                        if (dThis.ElementAt(k).Value.Contains("V"))
                        {
                            lIzquierda = dThis.ElementAt(i).Key.Except(dThis.ElementAt(k).Key).ToList();
                            lDerecha = dThis.ElementAt(k).Key.Except(dThis.ElementAt(i).Key).ToList();
                            lIntersect = dThis.ElementAt(k).Key.Intersect(dThis.ElementAt(i).Key).ToList();
                            if (lIntersect.Any())
                            {
                                lVerticales.AddRange(lIzquierda);
                                lVerticales.AddRange(lDerecha);
                                lVerticales.AddRange(lIntersect);
                                if (!dPhotoTagsAux.Any(x => x.Key == lVerticales && x.Value == "H-" + i + " " + k))
                                    dPhotoTagsAux.Add(lVerticales,"H-" + i + " " + k);
                                break;
                            }
                        }
                    }
                }
            }
            if(dPhotoTagsAux.Any(x=> x.Value.Contains("V")))
                Combinar(dPhotoTagsAux);
        }

    }
}
