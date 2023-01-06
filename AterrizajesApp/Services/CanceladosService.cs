using AterrizajesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace AterrizajesApp.Services
{
    public class CanceladosService
    {

        public CanceladosService(Partidas cancelado)
        {

            Thread hiloRecibir = new Thread(new ParameterizedThreadStart(eliminarcancelado));
            hiloRecibir.IsBackground = true;
            hiloRecibir.Start(cancelado);


        }


        public DispatcherTimer Temporizador = new DispatcherTimer();
        public void eliminarcancelado(object? p)
        {

            Temporizador.Interval = TimeSpan.FromSeconds(30);
            Temporizador.Tick += (object s, EventArgs a) => timer_Tick(s, a, p);
            Temporizador.Start();
        }

         public void timer_Tick(object sender, EventArgs e, object p)
        {
           Eliminar((Partidas)p);

           
           
        }

        public event Action<Partidas>? BorrarVueo;
        public void Eliminar(Partidas cancelado)
        {
            Temporizador.Stop();
            BorrarVueo?.Invoke(cancelado);

        }
    }
}
