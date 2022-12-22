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
        public async Task<bool> Update(Aterrizajes p)
        {
            //Validar

            var json = JsonConvert.SerializeObject(p);
            var response = await client.PutAsync("api/Aviones", new StringContent(json, Encoding.UTF8,
                "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) //BadRequest
            {
                var errores = await response.Content.ReadAsStringAsync();
                LanzarErrorJson(errores);
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                LanzarError("No se encontro el producto");
            }
            return true;
        }

        public event Action<List<string>> Error;
        void LanzarError(string mensaje)
        {
            Error?.Invoke(new List<string> { mensaje });
        }
        void LanzarErrorJson(string json)
        {
            List<string> obj = JsonConvert.DeserializeObject<List<string>>(json);
            if (obj != null)
            {
                Error?.Invoke(obj);
            }
        }


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

