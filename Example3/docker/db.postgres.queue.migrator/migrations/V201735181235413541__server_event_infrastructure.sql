
CREATE TABLE server_event( id bigserial PRIMARY KEY,  resource jsonb , created_at timestamp with time zone default CURRENT_DATE , fetched_at timestamp with time zone  NULL ) ;

create or REPLACE function create_server_event(payload jsonb ) returns jsonb 
LANGUAGE plpgsql
AS $func$
BEGIN
  
   INSERT INTO server_event.server_event(resource,created_at,fetched_at)
   VALUES (payload::jsonb,now(),NULL );
  RETURN payload;
END;
$func$;
