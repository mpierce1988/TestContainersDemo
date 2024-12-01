# Use the official Microsoft SQL Server 2019 image
FROM mcr.microsoft.com/mssql/server:2019-latest

# Set environment variables
ENV SA_PASSWORD=YourStrong!Passw0rd
ENV ACCEPT_EULA=Y

# Install mssql-tools
USER root
RUN apt-get update && apt-get install -y curl apt-transport-https gnupg && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && ACCEPT_EULA=Y apt-get install -y msodbcsql17 mssql-tools && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc && \
    /bin/bash -c "source ~/.bashrc"

# Create a directory for backup files
RUN mkdir -p /var/opt/mssql/backup

# Copy the AdventureWorks backup into the container
COPY AdventureWorksLT2019.bak /var/opt/mssql/backup/
COPY init.sql /var/opt/mssql/backup/

# Expose SQL Server port
EXPOSE 1433

# Run SQL Server and restore the AdventureWorks database
CMD /bin/bash -c "/opt/mssql/bin/sqlservr & \
    while ! /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -Q 'SELECT 1' &> /dev/null; do \
        echo 'Waiting for SQL Server to start...'; \
        sleep 5; \
    done; \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -Q \
    \"RESTORE DATABASE AdventureWorks FROM DISK = '/var/opt/mssql/backup/AdventureWorksLT2019.bak' WITH MOVE 'AdventureWorksLT2019_Data' TO '/var/opt/mssql/data/AdventureWorksLT2019_Data.mdf', MOVE 'AdventureWorksLT2019_Log' TO '/var/opt/mssql/data/AdventureWorksLT2019_Log.ldf', RECOVERY, STATS=5\" && \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -d AdventureWorks -i /var/opt/mssql/backup/init.sql && wait"