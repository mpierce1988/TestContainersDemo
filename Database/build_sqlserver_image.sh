set -e # Exit immediately if a command exits with a non-zero status.
set -u # Treat unset variables as an error

# Load configuration
if [ -f config.env ]; then
    source config.env
else
    echo "Configuration file config.env not found!"
    exit 1
fi

# Functions

# Function to create Dockerfile.restore
create_dockerfile_restore() {
    cat <<EOF > $DOCKERFILE_RESTORE
# Dockerfile.restore
FROM mcr.microsoft.com/mssql/server:2019-latest

ENV SA_PASSWORD=$SA_PASSWORD
ENV ACCEPT_EULA=$ACCEPT_EULA

# Install mssql-tools
USER root
RUN apt-get update && apt-get install -y curl apt-transport-https gnupg && \\
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \\
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \\
    apt-get update && ACCEPT_EULA=Y apt-get install -y msodbcsql17 mssql-tools && \\
    echo 'export PATH="\$PATH:/opt/mssql-tools/bin"' >> /etc/bash.bashrc

# Create a directory for backup files
RUN mkdir -p /var/opt/mssql/backup

# Copy the AdventureWorks backup and initialization scripts
COPY $BACKUP_FILE /var/opt/mssql/backup/
COPY $INIT_SCRIPT /var/opt/mssql/backup/

# Expose SQL Server port
EXPOSE 1433

# Start SQL Server and restore the database
CMD /bin/bash -c "/opt/mssql/bin/sqlservr & \\
    sleep 30 && \\
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P \$SA_PASSWORD -Q \\
    \\"RESTORE DATABASE AdventureWorks FROM DISK = '/var/opt/mssql/backup/$BACKUP_FILE' WITH MOVE 'AdventureWorksLT2019_Data' TO '/var/opt/mssql/data/AdventureWorksLT2019_Data.mdf', MOVE 'AdventureWorksLT2019_Log' TO '/var/opt/mssql/data/AdventureWorksLT2019_Log.ldf', RECOVERY, STATS=5\\" && \\
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P \$SA_PASSWORD -d AdventureWorks -i /var/opt/mssql/backup/$INIT_SCRIPT && \\
    pkill sqlservr"
EOF
    echo "Dockerfile.restore created."
}

# Function to create Dockerfile.final
create_dockerfile_final() {
    cat <<EOF > $DOCKERFILE_FINAL
# Dockerfile.final
FROM $RESTORED_IMAGE_NAME

# (Optional) Install additional tools or perform further configurations if needed

# Expose SQL Server port
EXPOSE 1433

# Set the default command to run SQL Server
CMD [ "/opt/mssql/bin/sqlservr" ]
EOF
    echo "Dockerfile.final created."
}

# Function to build the restore image
build_restore_image() {
    docker build -f $DOCKERFILE_RESTORE -t $RESTORE_IMAGE_NAME --platform $TARGET_PLATFORM .
    echo "Restore image $RESTORE_IMAGE_NAME built."
}

# Function to run the temporary container
run_temporary_container() {
    docker run --name $TEMP_CONTAINER_NAME $RESTORE_IMAGE_NAME
    echo "Temporary container $TEMP_CONTAINER_NAME run and exited after restoration."
}

# Function to commit the container to a new image
commit_container() {
    docker commit $TEMP_CONTAINER_NAME $RESTORED_IMAGE_NAME
    echo "Container $TEMP_CONTAINER_NAME committed to image $RESTORED_IMAGE_NAME."
}

# Function to remove the temporary container
remove_temporary_container() {
    docker rm $TEMP_CONTAINER_NAME
    echo "Temporary container $TEMP_CONTAINER_NAME removed."
}

# Function to build the final pre-restored image
build_final_image() {
    docker build --platform $TARGET_PLATFORM -f $DOCKERFILE_FINAL -t $FINAL_IMAGE_NAME .
    echo "Final pre-restored image $FINAL_IMAGE_NAME built."
}

# Function to clean up intermediate Dockerfiles
cleanup() {
    rm -f $DOCKERFILE_RESTORE $DOCKERFILE_FINAL
    echo "Cleanup completed. Intermediate Dockerfiles removed."
}

# Main Execution Flor
main() {
    echo "Starting SQL Server pre-restored image build process..."

    create_dockerfile_restore
    build_restore_image
    run_temporary_container
    commit_container
    remove_temporary_container
    create_dockerfile_final
    build_final_image

    echo "SQL Server pre-restored image $FINAL_IMAGE_NAME is ready."
}

# Execute main function
main