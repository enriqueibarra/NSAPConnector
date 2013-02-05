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

                command.Parameters.Add(new SapParameter("PROJECT_TYPE","EP"));

                session.StartSession();

                var resultDataSet = command.ExecuteDataSet();

                session.EndSession();

                //transaction.Commit();
                //var rfcTables = command.ExecuteRfcTables();

            }

            Console.WriteLine("Done !");
            Console.ReadKey();
        }
    }
}
