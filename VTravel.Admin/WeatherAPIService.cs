using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Transactions;
using MySql.Data.MySqlClient;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace VTravel.Admin
{
	public class WeatherAPIService: BackgroundService
	{
		private readonly ILogger<WeatherAPIService> _logger;
		private readonly IConfiguration _configuration;
		private readonly HttpClient _httpClient;
		private string DefaultConnectionString;
		private DateTime _lastRun = DateTime.MinValue;
		private bool _isRunning = false;
		public WeatherAPIService(ILogger<WeatherAPIService> logger, IConfiguration configuration, HttpClient httpClient)
		{
			_configuration = configuration;
			_logger = logger;
			DefaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
			_httpClient = httpClient;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var getweather = General.GetSettingsValue("get_weather");

			if (getweather == "1")
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					try
					{
						var runNow = false;
						if (DateTime.Now.Subtract(_lastRun).TotalHours > 4)
						{
							runNow = true;
						}

						if (runNow && !_isRunning)
						{
							_isRunning = true;

							try
							{
									await LoadData(stoppingToken);
									_lastRun = DateTime.Now;
									_isRunning = false;
							}
							catch (Exception ex)
							{
								General.LogReqResp(ex.Message, "Error");
							}
						}
					}
					catch(Exception ex)
					{
						General.LogReqResp(ex.Message, "Error");
					}
					finally
					{
						_isRunning = false;
					}
					await Task.Delay(10000, stoppingToken);
				}

			}
		}

		async Task LoadData(CancellationToken stoppingToken)
		{
			try
			{
				var _apiUrl = General.GetSettingsValue("weather_api_url");

				var response = await _httpClient.GetAsync(_apiUrl, stoppingToken);
				response.EnsureSuccessStatusCode(); // Throws an exception if the response status is not successful


				var responseBody = await response.Content.ReadAsStringAsync();
				WeatherResponse weather = JsonConvert.DeserializeObject<WeatherResponse>(responseBody);

				using (var connection = new MySqlConnection(Startup.conStr))
				{
					await connection.OpenAsync();  // Ensure the connection is properly opened
					using (var command = connection.CreateCommand())
					{
						command.CommandText = "SELECT COUNT(*) FROM weather_data";  // Example query
						var rows = await command.ExecuteScalarAsync();  // Safely execute the query
					//var query = @"SELECT COUNT(*) FROM weather_data";
					//int? rows = await connection.ExecuteScalarAsync<int?>(query);

						if(Convert.ToInt32(rows) == 0)
						{
							command.CommandText = string.Format(@"INSERT INTO weather_data (temperature, humidity, details, last_updated, sunrise, sunset, weather_icon) VALUES ({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}')",
										 Convert.ToDouble(weather.Main.Temp) - 273.15, weather.Main.Humidity, weather.Weather[0].Description, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
										 , DateTimeOffset.FromUnixTimeSeconds(weather.WeatherSys.Sunrise).DateTime.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.FromUnixTimeSeconds(weather.WeatherSys.Sunset).DateTime.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss")
										 , weather.Weather[0].Icon);

							var results = await command.ExecuteScalarAsync();
						}
						else
						{
							command.CommandText = string.Format(@"UPDATE weather_data SET temperature={0}, humidity = {1}, details = '{2}', last_updated = '{3}', sunrise = '{4}', sunset = '{5}', weather_icon = '{6}'",
										 Convert.ToDouble(weather.Main.Temp) - 273.15, weather.Main.Humidity, weather.Weather[0].Description, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
										 , DateTimeOffset.FromUnixTimeSeconds(weather.WeatherSys.Sunrise).DateTime.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), DateTimeOffset.FromUnixTimeSeconds(weather.WeatherSys.Sunset).DateTime.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss")
										 , weather.Weather[0].Icon);

							var results = await command.ExecuteScalarAsync();
						}

					}
				}


			}
			catch(Exception ex)
			{
				General.LogReqResp(ex.Message, "Error");
			}
		}
	}
	public class WeatherMain
	{
		[JsonProperty("temp")]
		public double Temp { get; set; }

		[JsonProperty("humidity")]
		public int Humidity { get; set; }
	}
	public class WeatherDesc
	{
		[JsonProperty("description")]
		public string Description { get; set; }
		[JsonProperty("icon")]
		public string Icon { get; set; }
	}
	public class WeatherSys
	{
		[JsonProperty("sunrise")]
		public long Sunrise { get; set; }
		[JsonProperty("sunset")]
		public long Sunset { get; set; }
	}

	public class WeatherResponse
	{
		[JsonProperty("main")]
		public WeatherMain Main { get; set; }
		[JsonProperty("weather")]
		public List<WeatherDesc> Weather { get; set; }
		[JsonProperty("sys")]
		public WeatherSys WeatherSys { get; set; }
	}
}
