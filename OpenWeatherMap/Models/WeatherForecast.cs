using System.Collections.Generic;

namespace OpenWeatherMap.Models
{
    public class CurrentWeatherModel
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

public class WeatherForecastModel
{
    public string cod { get; set; }
    public int message { get; set; }
    public int cnt { get; set; }
    public List<List> list { get; set; }
}

public class List
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

public class City
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

public class Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}

public class CurrentWeatherMain
{
    public double temp { get; set; }
    public double feels_like { get; set; }
    public double temp_min { get; set; }
    public double temp_max { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
}

public class WeatherForecastMain
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

public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}

public class Clouds
{
    public int all { get; set; }
}

public class Wind
{
    public double speed { get; set; }
    public int deg { get; set; }
}

public class WeatherForecastWind
{
    public double speed { get; set; }
    public int deg { get; set; }
    public double gust { get; set; }
    
}

public class Rain
{
    public double _3h { get; set; }
}

public class CurrentWeatherSys
{
    public int Type { get; set; }
    public int Id { get; set; }
    public string Country { get; set; }
    public int Sunrise { get; set; }
    public int Sunset { get; set; }
}

public class WeatherForecastSys
{
    public string Pod { get; set; }
}
}