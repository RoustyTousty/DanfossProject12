using MySql.Data.MySqlClient;
 
namespace DatabaseConnection
{
    class ConnectionSQL
    {
        static void Main()
        {
            string connectionString = "server=localhost;database=danfossproject12;uid=root;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
 
                    string selectQuery = "SELECT * FROM summer_season;";
                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("--- Summer Season ---");
                            while (reader.Read())
                            {
                                string from = reader.GetDateTime("time_from").ToString();
                                string to = reader.GetDateTime("time_to").ToString();
                                double demand = reader.GetDouble("Heat_Demand");
                                double price = reader.GetDouble("Electricity_Price");
                                Console.WriteLine($"From: {from}, To: {to}, Heat Demand: {demand}, Electricity Price: {price}");
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Error connecting to MySQL: {ex.Message}");
                }
            }
        }
    }
}