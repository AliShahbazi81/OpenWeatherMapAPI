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

     public async Task<CurrentWeatherModel> GetForecastByCityNameAsync(string city, Action<HttpClientHandler> httpClientHandler = null)
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
     
     public async Task<WeatherForecastModel> GetForecastByZipCodeAsync(string zipCode, string countryCode = "", Action<HttpClientHandler> httpClientHandler = null)
     {
         if (string.IsNullOrWhiteSpace(zipCode))
         {
             throw new ArgumentNullException(nameof(zipCode), "ZIP code cannot be null or empty.");
         }

         var query = !string.IsNullOrEmpty(countryCode) ? $"{zipCode},{countryCode}" : zipCode;

         try
         {
             
             var sBuilder = new StringBuilder();
             sBuilder.Append("forecast?zip=");
             sBuilder.Append(query);
             sBuilder.Append("&appid=");
             sBuilder.Append(_config.ApiKey);
             
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