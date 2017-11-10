$migrationName = Read-Host "Please Enter Migration Name"
$filename = "V"+(Get-Date -Format "yyyyMMddHHmmssffff") +"__"+ $migrationName + ".sql" 
New-Item $filename -type file