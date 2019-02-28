using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pizza
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkingSpace.InputData(@"C:\a_example.in");
            WorkingSpace.CutPizza();
            //WorkingSpace.InputData(@"C:\b_small.in");
            //WorkingSpace.CutPizza();
            //WorkingSpace.InputData(@"C:\c_medium.in");
            //WorkingSpace.CutPizza();

            Debugger.Break();
        }
    }
}
