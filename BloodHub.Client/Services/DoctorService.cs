using Blazored.LocalStorage;
using BloodHub.Client.Helpers;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Respones;

namespace BloodHub.Client.Services
{
    public class DoctorService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILocalStorageService _localStorage = localStorage;

        public async Task<ServiceResponse<List<Doctor>>> GetAllDoctors()
        {
            return await HttpClientHelper.SendRequest<List<Doctor>>(_httpClient, _localStorage, HttpMethod.Get, "api/doctor");
        }

        public async Task<ServiceResponse<Doctor>> GetDoctorById(int id)
        {
            return await HttpClientHelper.SendRequest<Doctor>(_httpClient, _localStorage, HttpMethod.Get, $"api/doctor/getbyid/{id}");
        }

        public async Task<ServiceResponse<Doctor>> CreateDoctor(Doctor doctor)
        {
            return await HttpClientHelper.SendRequest<Doctor>(_httpClient, _localStorage, HttpMethod.Post, "api/doctor/create", doctor);
        }

        public async Task<ServiceResponse<Doctor>> UpdateDoctor(int id, Doctor doctor)
        {
            return await HttpClientHelper.SendRequest<Doctor>(_httpClient, _localStorage, HttpMethod.Put, $"api/doctor/update/{id}", doctor);
        }

        public async Task<ServiceResponse<bool>> DeleteDoctor(int id)
        {
            return await HttpClientHelper.SendRequest<bool>(_httpClient, _localStorage, HttpMethod.Delete, $"api/doctor/delete/{id}");
        }
    }
}
