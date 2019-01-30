using System.Linq;
using System.Windows;
using Microsoft.Win32; //aqui se tienen cosas exclusivas de la interfaz de windows
using NAudio.Wave;

namespace GraficadorSeñales
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        double amplitudMaxima = 1;
        Señal señal;
        Señal señalResultado;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGraficar_Click(object sender, RoutedEventArgs e)
        {
            var reader = new AudioFileReader(txtRutaArchivo.Text); //si se pone var al inicio se toma el tipo de dato que se establece despues, en este caso toma el tipo audiofilereader
            double tiempoInicial = 0; //es 0 porque todos los archivos de audio empiezan en 0
            double tiempoFinal = reader.TotalTime.TotalSeconds; // aqui revisa cuantos segundos dura el archivo de audio
            double frecuenciaMuestreo = reader.WaveFormat.SampleRate; //dice la frecuencia de muestreo

            txtFrecuenciaMuestreo.Text = frecuenciaMuestreo.ToString();
            txtTiempoInicial.Text = "0";
            txtTiempoFinal.Text = tiempoFinal.ToString();

            //inicializar la señal
            señal = new SeñalPersonalizada();

            señal.TiempoInicial = tiempoInicial;
            señal.TiempoFinal = tiempoFinal;
            señal.FrecuenciaMuestreo = frecuenciaMuestreo;


            //construir nuestra señal a traves del archivo de audio
            //segun el numero de canales son los float que se ocupan mono=1 estereo=2 reader.WaveFormat.Channels esto dice cuantos canales son
            var bufferLectura = new float[reader.WaveFormat.Channels];//se declara un buffer de lectura que es un arreglo de bites, audiofile reader maneja muestras de 32 bits y aqui se van a guardar las muestras y se van a leer poco a poco

            int muestrasLeidas = 1; //nos dice cuantas muestras lee con el buffer
            double instanteActual = 0;
            double intervaloMuestra = 1.0 / frecuenciaMuestreo; //es el tiempo que transcurre entre muestra y muestra

            do
            {
                //se leen tantas muestras como canales se tienen 
                muestrasLeidas = reader.Read(bufferLectura, 0, reader.WaveFormat.Channels); //esta funcion lee las muestras, necesita varios parametros buffer, offset (cuantas muestras va a dejar pasar) y count cuanto va a leer
                if (muestrasLeidas > 0)
                {
                    double max = bufferLectura.Take(muestrasLeidas).Max(); //aqui dice que tome todas las muestras leidas y que saque el maximo
                    señal.Muestras.Add(new Muestra(instanteActual, max));
                }

                instanteActual += intervaloMuestra;

            } while (muestrasLeidas > 0);

            señal.actualizarAmplitudMaxima();
           
           
                amplitudMaxima = señal.AmplitudMaxima;
            
            plnGrafica.Points.Clear();
            

            lblAmplitudMaximaY.Text = señal.AmplitudMaxima.ToString("F");
            lblAmplitudMaximaNegativaY.Text = "-" + amplitudMaxima.ToString("F");
            if (señal != null)
            {
                //Recorrer una  coleccion o arreglo
                foreach (Muestra muestra in señal.Muestras)
                {

                    plnGrafica.Points.Add(new Point((muestra.X - tiempoInicial) * scrContenedor.Width, ((muestra.Y / amplitudMaxima) * ((scrContenedor.Height / 2.0) - 30) * -1)
                    + (scrContenedor.Height / 2)));
                }
               
            }
          

            plnEjeX.Points.Clear();
            //Punto del Principio
            plnEjeX.Points.Add(new Point(0, (scrContenedor.Height / 2)));
            //Punto del Fin
            plnEjeX.Points.Add(new Point(((tiempoFinal - tiempoInicial) * scrContenedor.Width), (scrContenedor.Height / 2)));

            //Punto del Principio
            plnEjeY.Points.Add(new Point(0 - tiempoInicial * scrContenedor.Width, scrContenedor.Height));
            //Punto del Fin
            plnEjeY.Points.Add(new Point(0-tiempoInicial*scrContenedor.Width,scrContenedor.Height*-1));

        }

        private void btnGraficarRampa_Click(object sender, RoutedEventArgs e)
        {
            //todas las señales ocupan estas 3
            double tiempoInicial =
                double.Parse(txtTiempoInicial.Text);
            double tiempoFinal =
                double.Parse(txtTiempoFinal.Text);
            double frecuenciaMuestreo =
                double.Parse(txtFrecuenciaMuestreo.Text);

            SeñalRampa señal =
                new SeñalRampa();

            double periodoMuestreo = 1 / frecuenciaMuestreo;

            plnGrafica.Points.Clear();

            for (double i = tiempoInicial; i <= tiempoFinal; i += periodoMuestreo)
            {
                double valorMuestra = señal.evaluar(i);

                señal.Muestras.Add(new Muestra(i, valorMuestra));
                //Recorrer una  coleccion o arreglo Aqui se agregan los puntos
                
            }
            //Recorrer una  coleccion o arreglo Aqui se agregan los puntos
            foreach (Muestra muestra in señal.Muestras)
            {
                plnGrafica.Points.Add(new Point(muestra.X * scrContenedor.Width, (muestra.Y * ((scrContenedor.Height / 2.0) - 30) * -1)
                + (scrContenedor.Height / 2)));
            }
        }
        


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Señal transformada = Señal.transformar(señal);

            transformada.actualizarAmplitudMaxima();

            plnGraficaResultado.Points.Clear();

            double res;

            lblAmplitudMaximaY_Resultado.Text = transformada.AmplitudMaxima.ToString("F");
            lblAmplitudMaximaNegativaY_Resultado.Text = "-" + transformada.AmplitudMaxima.ToString("F");
            


            if (transformada != null)
            {
                //Recorrer una  coleccion o arreglo
                foreach (Muestra muestra in transformada.Muestras)
                {

                    plnGraficaResultado.Points.Add(new Point((muestra.X - transformada.TiempoInicial) * scrContenedor_Resultado.Width, ((muestra.Y / transformada.AmplitudMaxima) * ((scrContenedor_Resultado.Height / 2.0) - 30) * -1)
                    + (scrContenedor_Resultado.Height / 2)));
                }

                double valorMaximo = 0;
                double segundoValorMaximo = 0;
                int indiceMaximo = 0;
                int indiceActual = 0;
                int segundoIndiceMaximo = 0;
                foreach (Muestra muestra in transformada.Muestras)
                {
                    if(muestra.Y > valorMaximo)
                    {
                        valorMaximo = muestra.Y;
                        indiceMaximo = indiceActual;
                    }
                    else if(muestra.Y > segundoValorMaximo && muestra.Y < valorMaximo) 
                    {
                        segundoValorMaximo = muestra.Y;
                        segundoIndiceMaximo = indiceActual;
                    }
                    indiceActual++;
                    if(indiceActual > (double)transformada.Muestras.Count / 2.0)
                    {
                        break;
                    }
                }


                //calcular frecuencia fundamental
                double frecuenciaFundamental = (double)indiceMaximo * señal.FrecuenciaMuestreo / (double)transformada.Muestras.Count;
                lblFrecuenciaFundamental.Text = frecuenciaFundamental.ToString() + " Hz";

                double segundaFrecuenciaFundamental = (double)segundoIndiceMaximo * señal.FrecuenciaMuestreo / (double)transformada.Muestras.Count;
                lblSegundaFrecuenciaFundamental.Text = segundaFrecuenciaFundamental.ToString() + " Hz";
            }

            plnEjeXResultado.Points.Clear();
            //Punto del Principio
            plnEjeXResultado.Points.Add(new Point(0, (scrContenedor_Resultado.Height / 2)));
            //Punto del Fin
            plnEjeXResultado.Points.Add(new Point(((transformada.TiempoFinal - transformada.TiempoInicial) * scrContenedor_Resultado.Width), (scrContenedor_Resultado.Height / 2)));

            //Punto del Principio
            plnEjeYResultado.Points.Add(new Point(0 - transformada.TiempoInicial * scrContenedor_Resultado.Width, scrContenedor_Resultado.Height));
            //Punto del Fin
            plnEjeYResultado.Points.Add(new Point(0 - transformada.TiempoInicial * scrContenedor_Resultado.Width, scrContenedor_Resultado.Height * -1));

        }

        private void btnExaminar_Click(object sender, RoutedEventArgs e)
        {
            //Abre cuadro de dialogo para seleccionar archivos para importar
            OpenFileDialog fileDialog = new OpenFileDialog();

            if ((bool)fileDialog.ShowDialog()) //se hace casting a bool para que en vez de 3 (true,false y null) opciones tenga 2 (true,false)
            {
                txtRutaArchivo.Text = fileDialog.FileName; // el nombre de la ruta seleccionada se queda guardada en el filedialog
            }
            
        }
    }
}
