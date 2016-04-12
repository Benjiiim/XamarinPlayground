using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject
{
    public class Core
    {
        public static async Task<Weather> GetWeather(string zipCode)
        {
            string queryString =
                "https://query.yahooapis.com/v1/public/yql?q=select+*+from+weather.forecast+where+woeid+in+(select+woeid+from+geo.places(1)+where+text=" +
                zipCode + ")&format=json";

            dynamic results = await DataService.getDataFromService(queryString).ConfigureAwait(false);

            dynamic weatherOverview = results["query"]["results"]["channel"];

            if ((string)weatherOverview["description"] != "Yahoo! Weather Error")
            {
                Weather weather = new Weather();

                weather.Title = (string)weatherOverview["description"];

                dynamic wind = weatherOverview["wind"];
                weather.Temperature = (string)wind["chill"];
                weather.Wind = (string)wind["speed"];

                dynamic atmosphere = weatherOverview["atmosphere"];
                weather.Humidity = (string)atmosphere["humidity"];
                weather.Visibility = (string)atmosphere["visibility"];

                dynamic astronomy = weatherOverview["astronomy"];
                weather.Sunrise = (string)astronomy["sunrise"];
                weather.Sunset = (string)astronomy["sunset"];

                return weather;
            }
            else
            {
                return null;
            }
        }
    }
}
