docker-compose -f "docker-compose.yml" stop 
docker-compose -f "docker-compose.yml" rm --force 
docker-compose -f "docker-compose.ci.build.yml" run --rm ci-build
docker-compose -f "docker-compose.yml" -f "docker-compose.override.yml"  up --build