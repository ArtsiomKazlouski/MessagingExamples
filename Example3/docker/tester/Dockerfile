FROM microsoft/aspnetcore-build:1.0-1.1
RUN apt-get update && apt-get -y install postgresql-client
COPY wait-for-postgres.sh /app/wait-for-postgres.sh
COPY wait-for-it.sh /app/wait-for-it.sh
RUN chmod +x /app/wait-for-postgres.sh
RUN chmod +x /app/wait-for-it.sh
WORKDIR /app

