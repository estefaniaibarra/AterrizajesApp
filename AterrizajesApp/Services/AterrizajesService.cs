using AterrizajesApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AterrizajesApp.Services
{
    public class AterrizajesService
    {
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("https://avionesaf.sistemas19.com/")
        };
        
        public async Task<List<Aterrizajes>> GetAll()
        {
            List<Aterrizajes>? aterizajes = null;
            var response = await client.GetAsync("api/Aviones");
            if (response.IsSuccessStatusCode)
            {

                var json = await response.Content.ReadAsStringAsync();
                aterizajes = JsonConvert.DeserializeObject<List<Aterrizajes>?>(json);
            }
            if (aterizajes == null)
            {
                return new List<Aterrizajes>();
            }
            else
            {
                return aterizajes;
            }
        }
    }
}

