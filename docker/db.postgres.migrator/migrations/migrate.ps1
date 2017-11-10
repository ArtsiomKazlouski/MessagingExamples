$pgConnection = $env:POSTGRES_CONNECTION

$pgUser = $env:POSTGRES_USER

$pgPassword = $env:POSTGRES_PASSWORD

& ./flyway.cmd -url="$pgConnection" -user="$pgUser" -password="$pgPassword" -baselineOnMigrate=true migrate