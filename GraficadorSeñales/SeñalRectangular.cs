using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficadorSeñales
{
    class SeñalRectangular : Señal
    {


        public SeñalRectangular()
        {
            Muestras = new List<Muestra>();
        }


        override public double evaluar(double tiempo)
        {
            double resultado;
            if (tiempo > .5) { resultado = 0; }
            else if (tiempo < .5) { resultado = 1; }
            else { resultado = .5; }
            return resultado;
        }
    }
}
