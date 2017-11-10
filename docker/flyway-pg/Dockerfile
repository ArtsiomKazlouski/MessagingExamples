FROM jimmyoak/flyway:4.0.3
RUN apt-get update && apt-get -y install postgresql-client
COPY wait-for-postgres.sh /flyway/wait-for-postgres.sh
RUN chmod +x /flyway/wait-for-postgres.sh

