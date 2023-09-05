using System.Collections.Generic;

namespace OpenWeatherMap.Models
{
    public struct CurrentWeatherModel
{
    public Coord coord { get; set; }
    public List<Weather> weather { get; set; }
    public string Base { get; set; }
    public CurrentWeatherMain CurrentWeatherMain { get; set; }
    public int Visibility { get; set; }
    public Wind wind { get; set; }
    public Clouds clouds { get; set; }
    public int dt { get; set; }
    public CurrentWeatherSys CurrentWeatherSys { get; set; }
    public int timezone { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int cod { get; set; }
}

public struct WeatherForecastModel
{
    public string cod { get; set; }
    public int message { get; set; }
    public int cnt { get; set; }
    public List<List> list { get; set; }
}

public struct List
{
    public int dt { get; set; }
    public WeatherForecastMain main { get; set; }
    public List<Weather> weather { get; set; }
    public Clouds clouds { get; set; }
    public WeatherForecastWind wind { get; set; }
    public int visibility { get; set; }
    public double pop { get; set; }
    public Rain rain { get; set; }
    public WeatherForecastSys sys { get; set; }
    public string dt_txt { get; set; }
}

public struct City
{
    public int id { get; set; }
    public string name { get; set; }
    public Coord coord { get; set; }
    public string country { get; set; }
    public int population { get; set; }
    public int timezone { get; set; }
    public int sunrise { get; set; }
    public int sunset { get; set; }
}

public struct Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}

public struct CurrentWeatherMain
{
    public double temp { get; set; }
    public double feels_like { get; set; }
    public double temp_min { get; set; }
    public double temp_max { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
}

public struct WeatherForecastMain
{
public double temp { get; set; }
public double feels_like { get; set; }
public double temp_min { get; set; }
public double temp_max { get; set; }
public double pressure { get; set; }
public double sea_level { get; set; }
public double grnd_level { get; set; }
public double humidity { get; set; }
public double temp_kf { get; set; }
}

public struct Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}

public struct Clouds
{
    public int all { get; set; }
}

public struct Wind
{
    public double speed { get; set; }
    public int deg { get; set; }
}

public struct WeatherForecastWind
{
    public double speed { get; set; }
    public int deg { get; set; }
    public double gust { get; set; }
    
}

public struct Rain
{
    public double _3h { get; set; }
}

public struct CurrentWeatherSys
{
    public int Type { get; set; }
    public int Id { get; set; }
    public string Country { get; set; }
    public int Sunrise { get; set; }
    public int Sunset { get; set; }
}

public struct WeatherForecastSys
{
    public string Pod { get; set; }
}
}