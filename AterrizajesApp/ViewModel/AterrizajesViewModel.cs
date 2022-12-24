using AterrizajesApp.Models;
using AterrizajesApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AterrizajesApp.ViewModel
{
    public class AterrizajesViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<Aterrizajes> ListaAterrizajes { get; set; }=new ObservableCollection<Aterrizajes>();
        
              public List<Aterrizajes>    Cancelados { get; set; }
        
        readonly AterrizajesService serviceAterrizaje = new();

        public CanceladosService cancelservice;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DispatcherTimer Temporizador;
        public AterrizajesViewModel()
        {
            CargarAterrizajes();
     
                    Temporizador = new DispatcherTimer();
            Temporizador.Interval = TimeSpan.FromSeconds(12);
            Temporizador.Tick += timer_Tick;
            Temporizador.Start();
            Cancelados = new List<Aterrizajes>();
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            CargarAterrizajes();
        }

        async void CargarAterrizajes()
        {
            //ListaAterrizajes.Clear();
            ListaAterrizajes = new ObservableCollection<Aterrizajes>(await serviceAterrizaje.GetAll());

           

            DateTime fechaactual = DateTime.Now;
          


            foreach (var item in ListaAterrizajes)
            {
                if (item.Status == "On Boarding")
                {
                    cancelservice = new CanceladosService(item);
                    cancelservice.BorrarVueo += Cancelservice_BorrarVueo;

                }

                if (item.Tiempo.Date == fechaactual && item.Status.ToLower() == "on time" )
                {
                    if (((item.Tiempo.TimeOfDay - fechaactual.TimeOfDay).TotalMinutes) < 10)
                    {
                        item.Status = "On Boarding";
                        await serviceAterrizaje.Update(item);
                    }



                }
                else if (item.Tiempo.Date < fechaactual && item.Status.ToLower() == "on time")
                {
                    item.Status = "On Boarding";
                    await serviceAterrizaje.Update(item);
                }



            }
            Actualizar(nameof(ListaAterrizajes));


        }

        private void Cancelservice_BorrarVueo(Aterrizajes obj)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
               obj.Status="Eliminado";
                                   
                serviceAterrizaje.Update(obj);

            });
        }

        public void Actualizar(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
