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
        readonly AterrizajesService serviceAterrizaje = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        public AterrizajesViewModel()
        {
            CargarAterrizajes();
        }
        void Actualizar(string? name=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
        
        async void CargarAterrizajes()
        {
            
            var datos = await serviceAterrizaje.GetAll();
            ListaAterrizajes.Clear();
            datos.ForEach(x=>ListaAterrizajes.Add(x));
            Actualizar();
        }
    }
}
