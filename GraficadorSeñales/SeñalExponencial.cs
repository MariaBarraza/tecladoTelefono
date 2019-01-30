using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficadorSeñales
{
    class SeñalExponencial : Señal
    {
        public double Alpha { get; set; }
        



        public SeñalExponencial()
        {
            Alpha = 1.0;
            Muestras = new List<Muestra>();
        }


        public SeñalExponencial(double alpha)
        {
            Alpha = alpha;
            
            Muestras = new List<Muestra>();
            
        }

        override public double evaluar(double tiempo)
        {
            double resultado;
            resultado = Math.Exp( Alpha * tiempo);
            return resultado;
        }
    }
}
