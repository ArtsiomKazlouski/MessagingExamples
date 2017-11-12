$pgConnection = $env:POSTGRES_CONNECTION

$pgUser = $env:POSTGRES_USER

$pgPassword = $env:POSTGRES_PASSWORD

& ./flyway.cmd -url="$pgConnection" -user="$pgUser" -password="$pgPassword" -baselineVersion=201708101734473447 -baselineOnMigrate=true migrate