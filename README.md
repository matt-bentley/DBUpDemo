# DBUpDemo
This is an example of how to use DBUp for SQL database development across local instance databases in a team project. Docker containers have been used to host local SQL Server instances however any implementation of SQL Server could be used.

Unit tests have been created to check for SQL syntax errors and code quality. These should be run before the main project.

## Docker Setup
Pull the docker image for SQL Server (Linux and Windows distributions are available)
```PowerShell
docker pull microsoft/mssql-server-linux:2017-latest
```

Create a container to host your instance of SQL Server. The User will be named 'SA' and the container will be bound to port 1401 on the host. The container will be named sql1
```PowerShell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" -p 1401:1433 --name sql1 -d microsoft/mssql-server-linux:2017-latest
```

You can now connect to your SQL instance using the IP address for your Docker host.


