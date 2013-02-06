NSAPConnector
==================

Wrapper library for the SAP .Net Connector 3.0. Adapts existing api to a more friendly form for the .Net developers.
NSAPConnector just modifies the api syntax without altering existing behavior and it also do not improve in any way
the performance of the SAP .Net Connector, you can consider this library as a syntactic sugar. NSAPConnector tries to 
emulate an ADO.Net behavior and usage which is the most common way of dealing with database data in the .Net world.

Because there're two different versions for the SAP .Net Connector 3.0 a x86 and a x64 version, there're 2 versions
of the NSAPConnector (basically it is the same project but with different references).

NSAPConnector was created by using .Net Framework 4.0 .

For the destination configuration part you can still use the config file which has the same structure as for
SAP .Net Connector 3.0 or you can provide configuration parameters directly to the SapConnection constructor(use
SapConfigParameters class for parameters name).

## NuGet
You can add this library to your project by using NuGet, you just have to type in your Package Manager Console:

```
Install-Package NSAPConnector_x86

```
or for the 64 bit version

```
Install-Package NSAPConnector_x64

```

## Usage examples (C#)

* Get a DataSet populated with all the returned result tables

```
           DataSet resultDataSet;

            using (var connection = new SapConnection("DEV"))
            {
                connection.Open();

                var command = new SapCommand("NAME_OF_THE_BAPI", connection);

                command.Parameters.Add("PARAM_NAME","Parameter Value");

                resultDataSet = command.ExecuteDataSet();
            }
```

* Get a DataReader which gather data from the specified returned result table

```
           SapDataReader sapDataReader;

            using (var connection = new SapConnection("DEV"))
            {
                connection.Open();

                var command = new SapCommand("NAME_OF_THE_BAPI", connection);

                command.Parameters.Add("PARAM_NAME","Parameter Value");

                sapDataReader = command.ExecuteReader("RESULTS");
                
                while(sapDataReader.Read())
                {
                    Console.WriteLine(sapDataReader.Item.GetString(0));
                }
            }
```

* Use transactions

```
           
            using (var connection = new SapConnection("DEV"))
            {
                connection.Open();
            
                var command = new SapCommand("NAME_OF_THE_BAPI", connection);

                command.Parameters.Add("PARAM_NAME","Parameter Value");

                var transaction = connection.BeginTransaction();
                
                 try
                  {
                    command.ExecuteRfc();

                    transaction.Commit();
                  }
                  catch (Exception ex)
                  {
                    //handle exception
                    transaction.Rollback();
                  }
                
                
            }
```


* Use sessions for statefull calls

```
           
            using (var connection = new SapConnection("DEV"))
            {
                connection.Open();
            
                var command = new SapCommand("NAME_OF_THE_BAPI", connection);

                command.Parameters.Add("PARAM_NAME","Parameter Value");

                var session = new SapSession(connection);
                
                session.StartSession();
                
                command.ExecuteRfc();
                
                session.EndSession();
                            
            }
```
