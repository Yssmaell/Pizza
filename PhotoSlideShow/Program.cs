using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSlideShow
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataImputcs.InputData(@"C:\a_example.txt");
            DataImputcs.InputData(@"C:\b_lovely_landscapes.txt");
            DataImputcs.Combinar(DataImputcs.dPhotoTags);
            
            Debug.WriteLine(DataImputcs.dPhotoTagsAux.Values.Count);
            foreach (var item in DataImputcs.dPhotoTagsAux)
            {
                Debug.WriteLine(item.Value.Split('-')[1]);
            }
        }
    }
}
