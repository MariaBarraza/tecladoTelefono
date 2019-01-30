using System;
using System.Collections.Generic;
using System.Numerics;

namespace GraficadorSeñales
{
    abstract class Señal
    {
        public List<Muestra> Muestras { get; set; }
        public double AmplitudMaxima { get; set; }
        public double TiempoInicial { get; set; }
        public double TiempoFinal { get; set; }
        public double FrecuenciaMuestreo { get; set; }

        public abstract double evaluar(double tiempo);

        public void construirSeñalDigital()
        {
            double periodoMuestreo = 1 / FrecuenciaMuestreo;

            for (double i = TiempoInicial; i <= TiempoFinal; i += periodoMuestreo)
            {
                double valorMuestra = evaluar(i);

                if (Math.Abs(valorMuestra) > AmplitudMaxima)
                {
                    AmplitudMaxima = Math.Abs(valorMuestra);
                }

                Muestras.Add(new Muestra(i, valorMuestra));

            }
        }
        public void escalar(double factor)
        {
            foreach (Muestra muestra in Muestras)
            {
                muestra.Y *= factor;
            }
        }

        public void desplazarY(double factor)
        {
            foreach (Muestra muestra in Muestras)
            {
                muestra.Y += factor;
            }
        }


        public void actualizarAmplitudMaxima()
        {
            AmplitudMaxima = 0;
            foreach(Muestra muestra in Muestras)
            {
                if (Math.Abs(muestra.Y) > AmplitudMaxima)
                {
                    AmplitudMaxima = Math.Abs(muestra.Y);
                }
            }
        }


        //
        public void truncar(float umbral)
        {
            foreach(Muestra muestra in Muestras)
            {
                if (muestra.Y > umbral)
                {
                    muestra.Y = umbral;
                }
                else if (muestra.Y < -umbral) 
                {
                    muestra.Y = -umbral;
                }
            }
        }
        //
        public static  Señal sumar(Señal sumando1, Señal sumando2)
        {
            SeñalPersonalizada resultado = new SeñalPersonalizada();
            resultado.TiempoInicial = sumando1.TiempoInicial;
            resultado.TiempoFinal = sumando1.TiempoFinal;
            resultado.FrecuenciaMuestreo = sumando1.FrecuenciaMuestreo;

            int indice = 0;
            foreach(Muestra muestra in sumando1.Muestras)
            {
                Muestra muestraResultado = new Muestra();
                muestraResultado.X = muestra.X;
                muestraResultado.Y = muestra.Y + sumando2.Muestras[indice].Y;
                indice++;
                resultado.Muestras.Add(muestraResultado);
            }

            return resultado;
        }

        public static Señal multiplicar(Señal factor1, Señal factor2)
        {
            SeñalPersonalizada resultado = new SeñalPersonalizada();
            resultado.TiempoInicial = factor1.TiempoInicial;
            resultado.TiempoFinal = factor1.TiempoFinal;
            resultado.FrecuenciaMuestreo = factor1.FrecuenciaMuestreo;

            int indice = 0;
            foreach (Muestra muestra in factor1.Muestras)
            {
                Muestra muestraResultado = new Muestra();
                muestraResultado.X = muestra.X;
                muestraResultado.Y = muestra.Y * factor2.Muestras[indice].Y;
                indice++;
                resultado.Muestras.Add(muestraResultado);
            }

            return resultado;
        }

        public static Señal convolucionar(Señal operando1, Señal operando2)
        {
            SeñalPersonalizada resultado = new SeñalPersonalizada();
            resultado.TiempoInicial = operando1.TiempoInicial + operando2.TiempoInicial;
            resultado.TiempoFinal = operando1.TiempoFinal + operando2.TiempoFinal;

            resultado.FrecuenciaMuestreo = operando1.FrecuenciaMuestreo;

            double duracionSeñal = resultado.TiempoFinal - resultado.TiempoInicial;

            double cantidadMuestrasResultado = duracionSeñal * resultado.FrecuenciaMuestreo;

            double periodoMuestreo = 1 / resultado.FrecuenciaMuestreo;

            double instanteActual = resultado.TiempoInicial;
            for (int n = 0; n < cantidadMuestrasResultado; n++)
            {
                double valorMuestraY = 0;
                for (int k=0; k < operando2.Muestras.Count; k++)
                {
                    if((n-k) >= 0 && (n-k) <  operando2.Muestras.Count)
                    {
                        valorMuestraY += operando1.Muestras[k].Y * operando2.Muestras[n - k].Y;
                    }
                }
                //nuevo
                valorMuestraY /= resultado.FrecuenciaMuestreo;
                //nuevo
                Muestra muestra = new Muestra(instanteActual, valorMuestraY);
                resultado.Muestras.Add(muestra);
                instanteActual += periodoMuestreo;
            }
            return resultado;
        }

        public static Señal transformar(Señal señal)
        {
            SeñalPersonalizada transformada = new SeñalPersonalizada();

            transformada.TiempoInicial = señal.TiempoInicial;
            transformada.TiempoFinal = señal.TiempoFinal;
            transformada.FrecuenciaMuestreo = señal.FrecuenciaMuestreo;

            for(int k=0; k < señal.Muestras.Count; k++)
            {
                // si se pone solo la parte real la parte imaginaria se toma como 0 
                Complex muestra = 0;
                //sumatoria
                //imaginaryOne le pone 0 a la parte real y 1 a la parte imaginaria
                for(int n=0; n<señal.Muestras.Count;n++)
                {
                    muestra += señal.Muestras[n].Y * Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne * k * n/señal.Muestras.Count);
                }
                // para obtener los numeros reales de un numero complejo se utiliza Magnitude
                transformada.Muestras.Add(new Muestra((double)k/(double)señal.Muestras.Count, muestra.Magnitude));
            }

            return transformada;
        }

    }
}
