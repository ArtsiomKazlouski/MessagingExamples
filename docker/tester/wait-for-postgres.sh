#!/bin/bash

# Exit immediately if a command exits with a non-zero status.
set -e

host="$1"
port="$2"
username="$3" ## the username in db
pgpass="$4" ## the password in db
database="$5" ## the database name
shift

until PGPASSWORD=$pgpass psql -h "$host" -p "$port" -U "$username" -d "$database" -c '\l'; do
  >&2 echo "Postgres is unavailable - sleeping"
  sleep 1
done

>&2 echo "Postgres is up"

