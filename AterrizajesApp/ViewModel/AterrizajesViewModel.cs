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
        public ObservableCollection<Partidas> ListaAterrizajes { get; set; }=new ObservableCollection<Partidas>();
        public ObservableCollection<Partidas> ListaAterrizajesFiltrada { get; set; }

        public List<string> cancelados = new List<string>() ;

        public List<Partidas>    Cancelados { get; set; }
        
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
            Cancelados = new List<Partidas>();

            FechaFiltro = DateTime.Now.Date;
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            CargarAterrizajes();
        }

       public async void CargarAterrizajes()
        {
            //ListaAterrizajes.Clear();
            ListaAterrizajes = new ObservableCollection<Partidas>(await serviceAterrizaje.GetAll());

            ListaAterrizajesFiltrada = new ObservableCollection<Partidas>(ListaAterrizajes.Select(x => x).Where(x => x.Tiempo.Date == FechaFiltro.Date));


            DateTime fechaactual = DateTime.Now;



            foreach (var item in ListaAterrizajes)
            {
                if (item.Status == "Cancelado"&& !cancelados.Contains(item.Vuelo))
                {
                    cancelados.Add(item.Vuelo);
                    cancelservice = new CanceladosService(item);
                    cancelservice.BorrarVueo += Cancelservice_BorrarVueo;

                }

                if (item.Status == "En vuelo" && fechaactual.TimeOfDay.Subtract(item.Tiempo.TimeOfDay).TotalSeconds>30)
                {

                    await serviceAterrizaje.Delete(item);
                }


                if (item.Tiempo.Date == fechaactual.Date && (item.Status.ToLower() == "programado"|| item.Status.ToLower() == "abordando"))
            {
                    var diferencia= (item.Tiempo.TimeOfDay - fechaactual.TimeOfDay).TotalMinutes;
                if (diferencia < 10 && diferencia >0)
                {
                    item.Status = "Abordando";
                    await serviceAterrizaje.Update(item);
                }
                else if (diferencia<0)
                    {
                        item.Status = "En vuelo";
                        await serviceAterrizaje.Update(item);
                    }



            }
            else if (item.Tiempo.Date < fechaactual.Date && (item.Status.ToLower() == "programado" || item.Status.ToLower() == "en vuelo"))
            {
                item.Status = "Concluido";
                await serviceAterrizaje.Update(item);
            }



        }

            ListaAterrizajesFiltrada = new ObservableCollection<Partidas>(ListaAterrizajes.Select(x => x).Where(x => x.Tiempo.Date == FechaFiltro.Date));
        Actualizar(nameof(ListaAterrizajesFiltrada));
            Actualizar(nameof(FechaFiltro));


        }

        private void Cancelservice_BorrarVueo(Partidas obj)
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
