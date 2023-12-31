﻿using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenWeatherMap.Configuration;
using OpenWeatherMap.Models;

namespace OpenWeatherMap.Services
{
    public class Service
    {
        private readonly Config _config;
        private readonly HttpClient _httpClient;
        private bool _disposed;

        public Service(Config config, Action<HttpClientHandler> httpClientHandler = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "Config cannot be null.");

            var handler = new HttpClientHandler();
            httpClientHandler?.Invoke(handler);

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_config.BaseUrl)
            };
        }
    // -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_ Current -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_
        public async Task<CurrentWeatherModel> GetCurrentWeatherByCityNameAsync(
            string city,
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentNullException(nameof(city), "City name cannot be null or empty.");

            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("weather?q=");
                sBuilder.Append(city);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                        throw new Exception("City not found.");
                    throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                }

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{city} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                // Print the JSON response string to the console
                //Console.WriteLine(responseContent);
                return JsonSerializer.Deserialize<CurrentWeatherModel>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                // Catch other HttpRequestException instances not handled by the above catch blocks
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Invalid JSON response received.", ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("The city name cannot be null or empty.", ex);
            }
            catch (Exception ex)
            {
                // General catch block to catch any other exceptions
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<CurrentWeatherModel> GetCurrentWeatherByGeoCoordinatesAsync(
            double latitude,
            double longitude,
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("weather?lat=");
                sBuilder.Append(latitude);
                sBuilder.Append("&lon=");
                sBuilder.Append(longitude);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{latitude},{longitude} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<CurrentWeatherModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }

        public async Task<CurrentWeatherModel> GetCurrentWeatherByCityIdAsync(
            int cityId, 
            string lang = "en",
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("weather?id=");
                sBuilder.Append(cityId);
                sBuilder.Append("&lang=");
                sBuilder.Append(lang);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{cityId} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<CurrentWeatherModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }

    // -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_ ForeCast -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_
    
    public async Task<WeatherForecastModel> GetForecastByZipCodeAsync(
            string zipCode,
            string stateCode = "",
            string countryCode = "",
            int cnt = 2,
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                throw new ArgumentNullException(nameof(zipCode), "ZIP code cannot be null or empty.");
            }

            // Check if the state code and country code is given
            var query = zipCode;
            if (!string.IsNullOrEmpty(stateCode))
                query = $"{query},{stateCode}";
            if (!string.IsNullOrEmpty(countryCode))
                query = $"{query},{countryCode}";

            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast?zip=");
                sBuilder.Append(query);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{zipCode} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }
    
    public async Task<WeatherForecastModel> Paid_GetFourDayHourlyWeatherForecastByGeoLocationAsync(
            double latitude,
            double longitude,
            int cnt = 2,
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/hourly?lat=");
                sBuilder.Append(latitude);
                sBuilder.Append("&lon=");
                sBuilder.Append(longitude);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{latitude},{longitude} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }

    public async Task<WeatherForecastModel> Paid_GetFourDayHourlyWeatherForecastByCityNameAsync(
        string city,
        string stateCode = "",
        string countryCode = "",
        int cnt = 2,
        string units = "",
        Action<HttpClientHandler> httpClientHandler = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentNullException(nameof(city), "City name cannot be null or empty.");

        try
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentNullException(nameof(city), "City name cannot be null or empty.");

                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/hourly?q=");
                sBuilder.Append(city);

                if (!string.IsNullOrWhiteSpace(stateCode))
                {
                    sBuilder.Append($",{stateCode}");
                }

                if (!string.IsNullOrWhiteSpace(countryCode))
                {
                    sBuilder.Append($",{countryCode}");
                }

                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                        throw new Exception("City not found.");
                    throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                }

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{city} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                // Print the JSON response string to the console
                //Console.WriteLine(responseContent);
                return JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                // Catch other HttpRequestException instances not handled by the above catch blocks
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Invalid JSON response received.", ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception("The city name cannot be null or empty.", ex);
            }
            catch (Exception ex)
            {
                // General catch block to catch any other exceptions
                throw new Exception("An unexpected error occurred.", ex);
            }
    }
    
    public async Task<WeatherForecastModel> Paid_GetFourDayHourlyWeatherForecastByCityIdAsync(
            int cityId, 
            string lang = "en",
            int cnt = 2,
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/hourly?id=");
                sBuilder.Append(cityId);
                sBuilder.Append("&lang=");
                sBuilder.Append(lang);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{cityId} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }
    
    public async Task<WeatherForecastModel> Paid_GetFourDayHourlyWeatherForecastByZipCodeAsync(
            string zipCode,
            string countryCode = "",
            int cnt = 2,
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                throw new ArgumentNullException(nameof(zipCode), "ZIP code cannot be null or empty.");
            }

            // Check if the state code and country code is given
            var query = zipCode;
            if (!string.IsNullOrEmpty(countryCode))
                query = $"{query},{countryCode}";

            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/hourly?zip=");
                sBuilder.Append(query);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{zipCode} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }

    public async Task<WeatherForecastModel> Paid_GetSixteenDayDailyWeatherForecastByGeoLocationAsync(
            double latitude,
            double longitude,
            int cnt = 2,
            string units = "",
            string mode = "",
            string lang = "en",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/daily?lat=");
                sBuilder.Append(latitude);
                sBuilder.Append("&lon=");
                sBuilder.Append(longitude);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Return JSON or XML - By default JSON
                if(!string.IsNullOrEmpty(mode))
                    sBuilder.Append($"&mode={mode}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");
                if(!string.IsNullOrEmpty(lang))
                    sBuilder.Append($"&lang={lang}");

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{latitude},{longitude} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }

    public async Task<WeatherForecastModel> Paid_GetSixteenDayDailyWeatherForecastByCityNameAsync(
        string city,
        string stateCode = "",
        string countryCode = "",
        int cnt = 2,
        string mode ="",
        string units = "",
        string lang = "en",
        Action<HttpClientHandler> httpClientHandler = null)
    {

        try
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentNullException(nameof(city), "City name cannot be null or empty.");

            var sBuilder = new StringBuilder();
            sBuilder.Append("forecast/daily?q=");
            sBuilder.Append(city);

            if (!string.IsNullOrWhiteSpace(stateCode))
                sBuilder.Append($",{stateCode}");

            if (!string.IsNullOrWhiteSpace(countryCode))
                sBuilder.Append($",{countryCode}");
            sBuilder.Append("&appid=");
            sBuilder.Append(_config.ApiKey);
            // Number of days returned
            sBuilder.Append($"&cnt={cnt}");
            // Return JSON or XML - By default JSON
            if (!string.IsNullOrEmpty(mode))
                sBuilder.Append($"&mode={mode}");
            // Imperial or metric
            if (!string.IsNullOrEmpty(units))
                sBuilder.Append($"&units={units}");
            if(!string.IsNullOrEmpty(lang))
                sBuilder.Append($"&lang={lang}");

            var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
            httpResponse.EnsureSuccessStatusCode();

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("City not found.");
                throw new Exception($"API returned a {httpResponse.StatusCode} status.");
            }

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new Exception($"{city} not found.");
                case HttpStatusCode.Unauthorized:
                    throw new Exception("Invalid API key or unauthorized access.");
                case HttpStatusCode.BadRequest:
                    throw new Exception("Bad request. Please check the request parameters.");
                default:
                {
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                    }

                    break;
                }
            }

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            // Print the JSON response string to the console
            //Console.WriteLine(responseContent);
            return JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);
        }
        catch (HttpRequestException ex)
        {
            // Catch other HttpRequestException instances not handled by the above catch blocks
            throw new Exception("There was a problem with the HTTP request.", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception("Invalid JSON response received.", ex);
        }
        catch (ArgumentNullException ex)
        {
            throw new Exception("The city name cannot be null or empty.", ex);
        }
        catch (Exception ex)
        {
            // General catch block to catch any other exceptions
            throw new Exception("An unexpected error occurred");
        }
    }
    
    public async Task<WeatherForecastModel> Paid_GetSixteenDayDailyWeatherForecastByCityIdAsync(
            int cityId, 
            string lang = "en",
            int cnt = 2,
            string mode = "",
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/daily?id=");
                sBuilder.Append(cityId);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Return JSON or XML - By default JSON
                if(!string.IsNullOrEmpty(mode))
                    sBuilder.Append($"&mode={mode}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");
                sBuilder.Append("&lang=");
                sBuilder.Append(lang);

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{cityId} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }
    
    public async Task<WeatherForecastModel> Paid_GetSixteenDayDailyWeatherForecastByZipCode(
            string zipCode,
            string countryCode = "",
            int cnt = 2,
            string mode = "",
            string units = "",
            string lang = "en",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                throw new ArgumentNullException(nameof(zipCode), "ZIP code cannot be null or empty.");
            }


            try
            {
                var query = zipCode;
                if (!string.IsNullOrEmpty(countryCode))
                    query = $"{query},{countryCode}";

                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/daily?zip=");
                sBuilder.Append(query);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Return JSON or XML - By default JSON
                if(!string.IsNullOrEmpty(mode))
                    sBuilder.Append($"&mode={mode}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");
                sBuilder.Append("&lang=");
                sBuilder.Append(lang);

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{zipCode} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
        }
    
    public async Task<WeatherForecastModel> Paid_GetThirtyDayClimateForecastByGeoLocationAsync(
            double latitude,
            double longitude,
            int cnt = 2,
            string units = "",
            string mode = "",
            string lang = "en",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/climate?lat=");
                sBuilder.Append(latitude);
                sBuilder.Append("&lon=");
                sBuilder.Append(longitude);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Return JSON or XML - By default JSON
                if(!string.IsNullOrEmpty(mode))
                    sBuilder.Append($"&mode={mode}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");
                sBuilder.Append("&lang=");
                sBuilder.Append(lang);

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{latitude},{longitude} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    case HttpStatusCode.Forbidden:
                        throw new Exception("API access is forbidden.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }
                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }

        }

    public async Task<WeatherForecastModel> Paid_GetThirtyDayClimateForecastByCityNameAsync(
        string city,
        string stateCode = "",
        string countryCode = "",
        int cnt = 2,
        string mode = "",
        string units = "",
        string lang = "en",
        Action<HttpClientHandler> httpClientHandler = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentNullException(nameof(city), "City name cannot be null or empty.");

            var sBuilder = new StringBuilder();
            sBuilder.Append("forecast/climate?q=");
            sBuilder.Append(city);

            if (!string.IsNullOrWhiteSpace(stateCode))
                sBuilder.Append($",{stateCode}");

            if (!string.IsNullOrWhiteSpace(countryCode))
                sBuilder.Append($",{countryCode}");
            sBuilder.Append("&appid=");
            sBuilder.Append(_config.ApiKey);
            // Number of days returned
            sBuilder.Append($"&cnt={cnt}");
            // Return JSON or XML - By default JSON
            if (!string.IsNullOrEmpty(mode))
                sBuilder.Append($"&mode={mode}");
            // Imperial or metric
            if (!string.IsNullOrEmpty(units))
                sBuilder.Append($"&units={units}");
            sBuilder.Append("&lang=");
            sBuilder.Append(lang);

            var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
            httpResponse.EnsureSuccessStatusCode();

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("City not found.");
                throw new Exception($"API returned a {httpResponse.StatusCode} status.");
            }

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new Exception($"{city} not found.");
                case HttpStatusCode.Unauthorized:
                    throw new Exception("Invalid API key or unauthorized access.");
                case HttpStatusCode.BadRequest:
                    throw new Exception("Bad request. Please check the request parameters.");
                case HttpStatusCode.Forbidden:
                    throw new Exception("API access is forbidden.");
                default:
                {
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                    }

                    break;
                }
            }

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            // Print the JSON response string to the console
            //Console.WriteLine(responseContent);
            return JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);
        }
        catch (HttpRequestException ex)
        {
            // Catch other HttpRequestException instances not handled by the above catch blocks
            throw new Exception("There was a problem with the HTTP request.", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception("Invalid JSON response received.", ex);
        }
        catch (ArgumentNullException ex)
        {
            throw new Exception("The city name cannot be null or empty.", ex);
        }
    }

    public async Task<WeatherForecastModel> Paid_GetThirtyDayClimateForecastByCityIdAsync(
            int cityId, 
            string lang = "en",
            int cnt = 2,
            string mode = "",
            string units = "",
            Action<HttpClientHandler> httpClientHandler = null)
        {
            try
            {
                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/climate?id=");
                sBuilder.Append(cityId);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Return JSON or XML - By default JSON
                if(!string.IsNullOrEmpty(mode))
                    sBuilder.Append($"&mode={mode}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");
                sBuilder.Append("&lang=");
                sBuilder.Append(lang);

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{cityId} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    case HttpStatusCode.Forbidden:
                        throw new Exception("API access is forbidden.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }
                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }

        }

    public async Task<WeatherForecastModel> Paid_GetThirtyDayClimateForecastByZipCodeAsync(
        string zipCode,
        string countryCode = "",
        int cnt = 2,
        string mode = "",
        string units = "",
        string lang = "en",
        Action<HttpClientHandler> httpClientHandler = null)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentNullException(nameof(zipCode), "ZIP code cannot be null or empty.");
            try
            {
                var query = zipCode;
                if (!string.IsNullOrEmpty(countryCode))
                    query = $"{query},{countryCode}";

                var sBuilder = new StringBuilder();
                sBuilder.Append("forecast/climate?zip=");
                sBuilder.Append(query);
                sBuilder.Append("&appid=");
                sBuilder.Append(_config.ApiKey);
                // Number of days returned
                sBuilder.Append($"&cnt={cnt}");
                // Return JSON or XML - By default JSON
                if(!string.IsNullOrEmpty(mode))
                    sBuilder.Append($"&mode={mode}");
                // Imperial or metric
                if(!string.IsNullOrEmpty(units))
                    sBuilder.Append($"&units={units}");
                sBuilder.Append("&lang=");
                sBuilder.Append(lang);

                var httpResponse = await _httpClient.GetAsync(sBuilder.ToString());
                httpResponse.EnsureSuccessStatusCode();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception($"{zipCode} not found.");
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid API key or unauthorized access.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Bad request. Please check the request parameters.");
                    default:
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"API returned a {httpResponse.StatusCode} status.");
                        }

                        break;
                    }
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<WeatherForecastModel>(responseContent);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("There was a problem with the HTTP request.", ex);
            }
    }

    public void Dispose()
        {
            if (_disposed)
                return;

            _httpClient.Dispose();
            _disposed = true;
        }
    }
}