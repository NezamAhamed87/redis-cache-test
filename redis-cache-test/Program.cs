using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            string connection = configuration["ELEndPoint"].ToString();
            Console.WriteLine($"Application Name: {configuration["ApplicationName"]}");
            Console.WriteLine($"Elasticache Endpoint Name: {configuration["ELEndPoint"]}");

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connection);
            IDatabase db = redis.GetDatabase();

            // Command line interface
            Console.WriteLine("Select operation: Read, Write, List");
            var operation = Console.ReadLine().ToLower();

            switch (operation)
            {
                case "read":
                    Console.WriteLine("Enter key:");
                    var key = Console.ReadLine();
                    var value = db.StringGet(key);
                    Console.WriteLine(value.HasValue ? value.ToString() : "Key not found.");
                    break;
                case "write":
                    Console.WriteLine("Enter key:");
                    key = Console.ReadLine();
                    Console.WriteLine("Enter value:");
                    var newValue = Console.ReadLine();
                    db.StringSet(key, newValue);
                    Console.WriteLine("Value set successfully.");
                    break;
                case "list":
                    var server = redis.GetServer(redis.GetEndPoints().First());
                    foreach (var redisKey in server.Keys(pattern: "*"))
                    {
                        Console.WriteLine(redisKey.ToString());
                    }
                    break;
                default:
                    Console.WriteLine("Invalid operation.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
