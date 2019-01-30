using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficadorSeñales
{
    class Muestra
    {
        //el instante de tiempo en que fue tomada la muestra
        public double X { get; set; }
        //el valor de la muestra en ese instante
        public double Y { get; set; }

        //Constructor que inicializa valores
        public Muestra(double x, double y)
        {
            X = x;
            Y = y;
        }

        //constructor sin parametros
        public Muestra()
        {
            X = 0.0;
            Y = 0.0;
        }

        
    }
}
