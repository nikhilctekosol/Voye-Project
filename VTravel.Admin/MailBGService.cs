using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

//using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace VTravel.Admin
{
    public class MailBGService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory; 
        private readonly ILogger<MailBGService> _logger;
        private readonly IConfiguration _configuration;
        private string DefaultConnectionString;
        private Timer _timer;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _jwtToken;
        private readonly string base_url;

        //private readonly ILoggingService _loggingService; 
        TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public MailBGService(IConfiguration configuration, IServiceScopeFactory scopeFactory, IHttpClientFactory clientFactory)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            DefaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
            _clientFactory = clientFactory;
            _jwtToken = General.GetSettingsValue("admin_jwtkey");
            base_url = General.GetSettingsValue("base_url");

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var enabled = General.GetSettingsValue("mailservice_enabled");
            if (enabled == "1")
            {
                DateTime lastutc = DateTime.UtcNow;
                DateTime lastindian = TimeZoneInfo.ConvertTimeFromUtc(lastutc, indianTimeZone);
                DateTime _lastRun = new DateTime(lastindian.Year, lastindian.Month, lastindian.Day, 0, 0, 0);

                LogReqResp("Service started with last run :" + _lastRun.ToString("dd-MM-yyyy HH:mm:ss"), "Info");


                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        //TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                        DateTime utcDateTime = DateTime.UtcNow;
                        var now = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, indianTimeZone);

                        //var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 0, 42, 0);
                        //if (now > nextRunTime)
                        //{
                        //    nextRunTime = nextRunTime.AddDays(1);
                        //}
                        //var dueTime = nextRunTime - now;
                        //_timer = new Timer(EmailApi, null, dueTime, TimeSpan.FromDays(1));
                        Wakeup();

                        var send_mail_now = General.GetSettingsValue("send_mail_now");
                        if (send_mail_now == "1")
                        {
                            EmailApi();

                            //var setsettings = General.SetSettingsValue("send_mail_now", "0");
                            var query = string.Format(@"UPDATE system_settings SET settings_value='0' WHERE settings_name='send_mail_now'");


                            using (var connection = new MySqlConnection(Startup.conStr))
                            {

                                var results = connection.ExecuteAsync(query);


                            }
                        }
                        else
                        {

                            //LogReqResp("Time :" + now.ToString("dd-MM-yyyy HH:mm:ss"), "Info");

                            if (now.Subtract(_lastRun).TotalHours > 24)
                            {
                                LogReqResp("Email call in :" + now.ToString("dd-MM-yyyy HH:mm:ss"), "Info");

                                _lastRun = now;

                                EmailApi();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        General.LogReqResp("Error calling API: " + ex.Message, "Error");
                    }

                    await Task.Delay(900000, stoppingToken);
                }
            }

        }
        //public override Task StopAsync(CancellationToken stoppingToken)
        //{
        //    _timer?.Change(Timeout.Infinite, 0);
        //    return Task.CompletedTask;
        //}

        //public void Dispose()
        //{
        //    _timer?.Dispose();
        //}

        private async void EmailApi()
        {
            try
            {
                //using (var scope = _scopeFactory.CreateScope())
                //{
                //    var serviceProvider = scope.ServiceProvider;
                //    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

                //    // Create an HTTP client instance
                //    var httpClient = httpClientFactory.CreateClient();


                    await MakeApiCall(base_url + "api/reservation/first-email");
                    await MakeApiCall(base_url + "api/reservation/second-email");
                    LogReqResp("API call completed.", "Info");
                //}
            }
            catch(Exception ex)
            {
                General.LogReqResp("Error executing API: " + ex.Message, "Error");
            }
        }
        private async Task MakeApiCall(string endpoint)
        {
            try
            {
                LogReqResp("API call starting.", "Info");
                //var response = await httpClient.GetAsync(endpoint);

                var httpClient = _clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

                var response = await httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    LogReqResp("API call to email success:" + endpoint, "Info");
                    //Console.WriteLine($"API call to {endpoint} succeeded.");
                }
                else
                {
                    LogReqResp($"Error executing API:" + endpoint + "-" + response, "Error");
                    //Console.WriteLine($"API call to {endpoint} failed with status code {response.StatusCode}.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                //LogException(ex);
                General.LogReqResp("Error Make API Call: " + ex.Message, "Error");
                //Console.WriteLine($"An error occurred while making API call to {endpoint}: {ex.Message}");
            }
        }
        public void LogException(Exception exception)
        {
            var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            var logFile = Path.Combine(logFolder, $"{DateTime.Now:yyyyMMdd}.txt");
            File.AppendAllText(logFile, $"{DateTime.Now:G}: {exception}\n\n");
        }
        public void LogReqResp(string content, string type)
        {
            var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            var logFile = Path.Combine(logFolder, $"{DateTime.Now:yyyyMMdd}.txt");
            File.AppendAllText(logFile, $"{DateTime.Now:G}: {content} : {type}\n\n");
        }
        private async void Wakeup()
        {
            try
            {
                //LogReqResp("Wakeup Call.", "Info");
                //using (var scope = _scopeFactory.CreateScope())
                //{
                //    var serviceProvider = scope.ServiceProvider;
                //    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

                    // Create an HTTP client instance
                    //var httpClient = httpClientFactory.CreateClient();


                    //LogReqResp("wake call starting.", "Info");
                    await MakeApiCall(base_url + "api/reservation/wake-up");
                    //LogReqResp("wake call completed.", "Info");
                //}
            }
            catch (Exception ex)
            {
                General.LogReqResp("Error executing API: " + ex.Message, "Error");
            }
        }
    }
}
