# Setting Up a Pre-Restored SQL Server Docker Image on MacOS with M1 (ARM) Architecture

This guide will walk you through the process of setting up a SQL Server instance in a Docker container on a MacOS machine with an M1 (ARM) architecture. By pre-restoring the database, you can significantly reduce container startup times, making it ideal for testing and development environments.

## Table of Contents

- [Prerequisites](#Prerequisites)
-     [Overview](#Overview)
	-     [Step 1: Create Dockerfile.restore](#Step 1: Create Dockerfile.restore)
	-     [Step 2: Build and Run the Temporary Restore Container](#Step 2: Build and Run the Temporary Restore Container)
	-     [Step 3: Commit the Restored Container to a New Image](#Step 3: Commit the Restored Container to a New Image)
	-     [Step 4: Create Dockerfile.final](#Step 4: Create Dockerfile.final)
	-     [Step 5: Build the Final Pre-Restored Image](#Step 5: Build the Final Pre-Restored Image)
	-     [Step 6: Run the Pre-Restored SQL Server Container](#Step 6: Run the Pre-Restored SQL Server Container)
-     [Additional Considerations](#Additional Considerations)
-     [Conclusion](#Conclusion)

## Prerequisites

Before you begin, ensure you have the following installed and configured on your MacOS machine:

- Docker Desktop for Mac: Make sure you have the latest version installed, supporting Apple Silicon (M1) architecture.
-     Docker Command-Line Interface (CLI): Familiarity with basic Docker commands.
-     SQL Server Backup File: The AdventureWorksLT2019.bak file.
-     Initialization SQL Script: The init.sql script for any post-restore configurations.

## Overview

On MacOS with M1 (ARM) architecture, Docker defaults to using ARM-based images. However, the official Microsoft SQL Server Docker images are built for the linux/amd64 architecture. To overcome this, you need to specify the platform when building your Docker images. This guide covers creating a Docker image with a pre-restored SQL Server database, optimizing container startup times.

### Step 1: Create Dockerfile.restore

This Dockerfile sets up SQL Server, installs necessary tools, copies the backup and initialization scripts, and restores the database during the image build process.

```bash
# Dockerfile.restore
FROM mcr.microsoft.com/mssql/server:2019-latest

ENV SA_PASSWORD=YourStrong!Passw0rd
ENV ACCEPT_EULA=Y

# Install mssql-tools
USER root
RUN apt-get update && apt-get install -y curl apt-transport-https gnupg && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && ACCEPT_EULA=Y apt-get install -y msodbcsql17 mssql-tools && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> /etc/bash.bashrc

# Create a directory for backup files
RUN mkdir -p /var/opt/mssql/backup

# Copy the AdventureWorks backup and initialization scripts
COPY AdventureWorksLT2019.bak /var/opt/mssql/backup/
COPY init.sql /var/opt/mssql/backup/

# Expose SQL Server port
EXPOSE 1433

# Start SQL Server and restore the database
CMD /bin/bash -c "/opt/mssql/bin/sqlservr & \
    sleep 30 && \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -Q \
    \"RESTORE DATABASE AdventureWorks FROM DISK = '/var/opt/mssql/backup/AdventureWorksLT2019.bak' WITH MOVE 'AdventureWorksLT2019_Data' TO '/var/opt/mssql/data/AdventureWorksLT2019_Data.mdf', MOVE 'AdventureWorksLT2019_Log' TO '/var/opt/mssql/data/AdventureWorksLT2019_Log.ldf', RECOVERY, STATS=5\" && \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD -d AdventureWorks -i /var/opt/mssql/backup/init.sql && \
    pkill sqlservr"
```

Explanation:

- Base Image: Uses the official Microsoft SQL Server 2019 image.
-     Environment Variables: Sets the SA_PASSWORD and accepts the EULA.
-     Install mssql-tools: Installs additional SQL Server tools for command-line operations.
-     Backup Directory: Creates a directory to store backup files.
-     Copy Files: Copies the backup and initialization scripts into the container.
-     Expose Port: Exposes port 1433 for SQL Server.
-     CMD: Starts SQL Server, waits for it to initialize, restores the database, runs initialization scripts, and then stops SQL Server.

### Step 2: Build and Run the Temporary Restore Container

Build the Dockerfile.restore image and run a temporary container to perform the database restoration.

#### Build the Restore Image
```bash
docker build -f Dockerfile.restore -t sqlserver-restoretwo .
```

#### Run the Temporary Container

```bash
docker run --name temp-sqlserver sqlserver-restoretwo
```

This container will execute the restoration process as defined in the CMD directive of Dockerfile.restore.

### Step 3: Commit the Restored Container to a New Image

After the restoration is complete, commit the state of the temporary container to create a new image with the pre-restored database.

#### Commit the Container
```bash
docker commit temp-sqlserver sqlserver-with-adventureworkstwo
```

This command creates a new image named sqlserver-with-adventureworkstwo based on the state of temp-sqlserver.

#### Remove the Temporary Container
```bash
docker rm temp-sqlserver
```
Cleaning up by removing the temporary container as it's no longer needed.

### Step 4: Create Dockerfile.final

This Dockerfile uses the pre-restored image as its base, allowing you to run SQL Server without performing the restoration process each time.

```bash
# Dockerfile.final
FROM sqlserver-with-adventureworkstwo

# (Optional) Install additional tools or perform further configurations if needed

# Expose SQL Server port
EXPOSE 1433

# Set the default command to run SQL Server
CMD [ "/opt/mssql/bin/sqlservr" ]
```
Explanation:

- Base Image: Uses the sqlserver-with-adventureworkstwo image containing the pre-restored database.
-     Expose Port: Ensures port 1433 is exposed.
-     CMD: Sets the default command to start SQL Server when the container runs.

### Step 5: Build the Final Pre-Restored Image

Due to the M1 (ARM) architecture of your Mac, you need to specify the platform (linux/amd64) when building the final image to ensure compatibility.

```bash
docker build --platform linux/amd64 -f Dockerfile.final -t sqlserver-prerestoredtwo .
```

Explanation:

- --platform linux/amd64: Specifies the target platform, ensuring Docker builds the image for the amd64 architecture, which is compatible with the SQL Server image.
-     -f Dockerfile.final: Points to the final Dockerfile.
-     -t sqlserver-prerestoredtwo: Tags the new image as sqlserver-prerestoredtwo.

### Step 6: Run the Pre-Restored SQL Server Container

With the sqlserver-prerestoredtwo image built, you can now run containers that start SQL Server with the AdventureWorks database already restored, eliminating the need for the restoration process during each startup.

```bash
docker run -d \
  --name my-sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 \
  sqlserver-prerestoredtwo
```

Explanation:

- -d: Runs the container in detached mode.
-     --name my-sqlserver: Names the container my-sqlserver.
-     Environment Variables:
-         ACCEPT_EULA=Y: Accepts the End User License Agreement.
-         SA_PASSWORD=YourStrong!Passw0rd: Sets the system administrator password.
-     -p 1433:1433: Maps port 1433 of the host to port 1433 of the container.
-     sqlserver-prerestoredtwo: Specifies the image to run.

## Additional Considerations
### Handling ARM and AMD64 Architectures

MacOS with M1 chips uses the ARM architecture, whereas many Docker images, including SQL Server's official image, are built for amd64. Specifying the platform during the build process ensures compatibility.

### Image Size

Pre-restoring the database increases the size of your Docker image. Ensure your system has adequate storage and consider using .dockerignore to exclude unnecessary files from the build context.

### Security

- Sensitive Information: Avoid hardcoding sensitive information like SA_PASSWORD in Dockerfiles. Consider using Docker secrets or environment variables securely.
-     Database Credentials: Ensure that the SA_PASSWORD is strong and complies with SQL Server's password policies.

### Automation

For streamlined workflows, consider automating the build and commit process using scripts or integrating it into your CI/CD pipeline.

### Docker Compose

If managing multiple services, Docker Compose can simplify container orchestration. Below is an example docker-compose.yml for running the pre-restored SQL Server:

```bash
version: '3.8'

services:
  sqlserver:
    image: sqlserver-prerestoredtwo
    container_name: my-sqlserver
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong!Passw0rd
    ports:
      - "1433:1433"
    restart: unless-stopped
```
Usage:

```bash
docker-compose up -d
```

## Conclusion

By following this guide, you've successfully created a Docker image with a pre-restored SQL Server database tailored for MacOS with M1 architecture. This setup optimizes container startup times, making it ideal for testing and development scenarios. Remember to manage your images and containers efficiently to maintain a clean and secure environment.

If you encounter any issues or have further questions, feel free to reach out for assistance!