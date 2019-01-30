using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficadorSeñales
{
    class SeñalRampa :Señal
    {


        
        

        public SeñalRampa()
        {
            Muestras = new List<Muestra>();
        }


        override public double evaluar(double tiempo)
        {
            double resultado;
            if (tiempo >= 0) { resultado = tiempo; }
            else { resultado = 0; }
            return resultado;
        }
    }
}
