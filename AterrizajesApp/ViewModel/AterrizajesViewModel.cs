using AterrizajesApp.Models;
using AterrizajesApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AterrizajesApp.ViewModel
{
    public class AterrizajesViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<Aterrizajes> ListaAterrizajes { get; set; }=new ObservableCollection<Aterrizajes>();
        public ObservableCollection<Aterrizajes> ListaAterrizajesFiltrada { get; set; }

        public List<string> cancelados = new List<string>() ;

        public List<Aterrizajes>    Cancelados { get; set; }
        
        readonly AterrizajesService serviceAterrizaje = new();

        public CanceladosService cancelservice;

        public DateTime FechaFiltro { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public DispatcherTimer Temporizador;
        public AterrizajesViewModel()
        {
            CargarAterrizajes();
     
                    Temporizador = new DispatcherTimer();
            Temporizador.Interval = TimeSpan.FromSeconds(10);
            Temporizador.Tick += timer_Tick;
            Temporizador.Start();
            Cancelados = new List<Aterrizajes>();

            FechaFiltro = DateTime.Now.Date;
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            CargarAterrizajes();
        }

       public async void CargarAterrizajes()
        {
            //ListaAterrizajes.Clear();
            ListaAterrizajes = new ObservableCollection<Aterrizajes>(await serviceAterrizaje.GetAll());



            DateTime fechaactual = DateTime.Now;



            foreach (var item in ListaAterrizajes)
            {
                if (item.Status == "Cancelado"&& !cancelados.Contains(item.Vuelo))
                {
                    cancelados.Add(item.Vuelo);
                    cancelservice = new CanceladosService(item);
                    cancelservice.BorrarVueo += Cancelservice_BorrarVueo;

                }
            

            if (item.Tiempo.Date == fechaactual.Date && item.Status.ToLower() == "on time")
            {
                if (((item.Tiempo.TimeOfDay - fechaactual.TimeOfDay).TotalMinutes) < 10)
                {
                    item.Status = "On Boarding";
                    await serviceAterrizaje.Update(item);
                }



            }
            else if (item.Tiempo.Date < fechaactual.Date && item.Status.ToLower() == "on time")
            {
                item.Status = "On Boarding";
                await serviceAterrizaje.Update(item);
            }



        }

            ListaAterrizajesFiltrada = new ObservableCollection<Aterrizajes>(ListaAterrizajes.Select(x => x).Where(x => x.Tiempo.Date == FechaFiltro.Date));
        Actualizar(nameof(ListaAterrizajesFiltrada));
            Actualizar(nameof(FechaFiltro));


        }

        private void Cancelservice_BorrarVueo(Aterrizajes obj)
        {
           
               obj.Status="Eliminado";
                                   
                serviceAterrizaje.Delete(obj);
            cancelados.Remove(obj.Vuelo);


        }

        public void Actualizar(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
