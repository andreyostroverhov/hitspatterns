using Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Services
{
    public class Converter
    {
        private const string ApiKey = "73ef1999001b2d2f3ba42653";
        private readonly HttpClient client = new HttpClient();

        public async Task<decimal> Convert(Currency from, Currency to, decimal amount)
        {
            if (from == to)
            {
                return amount;
            }

            var response = await client.GetAsync(
                 $"https://v6.exchangerate-api.com/v6/{ApiKey}/pair/{from}/{to}/{amount}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка конвертации валюты");

            var content = await response.Content.ReadFromJsonAsync<ExchangeRateApiResponse>();

            return content.ConversionResult;
        }
    }


    public class ExchangeRateApiResponse
    {
        public decimal ConversionResult { get; set; }
    }
}
