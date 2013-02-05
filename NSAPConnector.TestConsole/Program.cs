using System;


namespace NSAPConnector.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SapConnection("DEV"))
            {
                connection.Open();

                var session = new SapSession(connection);

                //var transaction = connection.BeginTransaction();

                var command = new SapCommand("BAPI_GET_PROJECT_DETAILS_TABLE", connection);

                command.Parameters.Add("PROJECT_TYPE","EP");

                session.StartSession();

                var resultDataSet = command.ExecuteDataSet();

                session.EndSession();

                var sapDataReader = command.ExecuteReader("RESULTS");

                
                var i = 0;
                while(sapDataReader.Read())
                {
                    Console.WriteLine("{0} : {1}",i,sapDataReader.Item.GetString(0));
                    i++;
                }

                Console.WriteLine(sapDataReader.RowCount);

                //transaction.Commit();
                //var rfcTables = command.ExecuteRfcTables();

            }

            Console.WriteLine("Done !");
            Console.ReadKey();
        }
    }
}
