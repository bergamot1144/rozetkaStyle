using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConsoleAppDBManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = @"Data Source=working; Initial Catalog=rozetka; Trusted_Connection=True; TrustServerCertificate=True";
            DatabaseManager dbManager = new DatabaseManager(connectionString);

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Create a new table");
                Console.WriteLine("2. Add data to a table");
                Console.WriteLine("3. View tables");
                Console.WriteLine("4. View table data");
                Console.WriteLine("5. Update table data");
                Console.WriteLine("0. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await dbManager.CreateTableAsync();
                        break;
                    case "2":
                        await dbManager.InsertDataIntoTableAsync();
                        break;
                    case "3":
                        await dbManager.ViewTablesAsync();
                        break;
                    case "4":
                        await dbManager.ViewTableDataAsync();
                        break;
                    case "5":
                        await dbManager.UpdateTableDataAsync();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }

    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        // 1. Создание таблицы
        public async Task CreateTableAsync()
        {
            Console.Write("Enter table name: ");
            string tableName = Console.ReadLine();

            Console.WriteLine("Define fields (format: name type constraints). Type 'done' when finished:");
            List<string> fields = new List<string>();
            while (true)
            {
                string fieldInput = Console.ReadLine();
                if (fieldInput.ToLower() == "done")
                    break;

                fields.Add(fieldInput);
            }

            string query = $"CREATE TABLE {tableName} ({string.Join(", ", fields)});";
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"Table '{tableName}' created successfully.");
        }

        // 2. Вставка данных в таблицу
        public async Task InsertDataIntoTableAsync()
        {
            Console.Write("Enter table name: ");
            string tableName = Console.ReadLine();

            Console.WriteLine("Enter the data fields separated by commas (e.g., 'Name, Age'):");
            string fields = Console.ReadLine();

            Console.WriteLine("Enter the values separated by commas (e.g., 'John, 30'):");
            string values = Console.ReadLine();

            string query = $"INSERT INTO {tableName} ({fields}) VALUES ({values});";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"Data inserted into '{tableName}' successfully.");
        }

        // 3. Просмотр всех таблиц
        public async Task ViewTablesAsync()
        {
            string query = "SELECT name FROM sys.tables";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    Console.WriteLine("Tables in the database:");
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }
                }
            }
        }

        // 4. Просмотр данных в таблице
        public async Task ViewTableDataAsync()
        {
            Console.Write("Enter table name: ");
            string tableName = Console.ReadLine();

            string query = $"SELECT * FROM {tableName};";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    int fieldCount = reader.FieldCount;
                    while (await reader.ReadAsync())
                    {
                        for (int i = 0; i < fieldCount; i++)
                        {
                            Console.Write($"{reader.GetValue(i)}\t");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        // 5. Редактирование данных
        public async Task UpdateTableDataAsync()
        {
            Console.Write("Enter table name: ");
            string tableName = Console.ReadLine();

            Console.Write("Enter the field to update: ");
            string fieldToUpdate = Console.ReadLine();

            Console.Write("Enter the new value: ");
            string newValue = Console.ReadLine();

            Console.Write("Enter the condition to update (e.g., 'Id = 1'): ");
            string condition = Console.ReadLine();

            string query = $"UPDATE {tableName} SET {fieldToUpdate} = {newValue} WHERE {condition};";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"Data updated in '{tableName}' successfully.");
        }
    }
}
