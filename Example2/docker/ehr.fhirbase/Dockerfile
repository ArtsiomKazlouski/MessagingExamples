FROM valeryk/docker-postgres-tds-fdw:9.6-1.0.8

COPY scripts/ /docker-entrypoint-initdb.d/

RUN mkdir -p /mnt/hdd_slow_1/pgsql/ts_01/ && chown postgres /mnt/hdd_slow_1/pgsql/ts_01/ && mkdir -p /mnt/hdd_fast_1/pgsql/ts_02/ && chown postgres /mnt/hdd_fast_1/pgsql/ts_02/