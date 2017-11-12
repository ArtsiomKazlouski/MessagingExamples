CREATE OR REPLACE FUNCTION fhirbase_conformance.conformance(_cfg jsonb)
  RETURNS jsonb AS
$BODY$
  SELECT json_build_object( -- fhirbase_conformance:3
    'resourceType', 'Conformance', -- fhirbase_conformance:4
    'id', COALESCE(_cfg->>'id',''), -- fhirbase_conformance:5
    'version', _cfg->'version', -- fhirbase_conformance:6
    'name', COALESCE(_cfg->>'name',''), -- fhirbase_conformance:7
    'publisher', _cfg->'publisher', -- fhirbase_conformance:8
    'description', COALESCE(_cfg->>'description', ''), -- fhirbase_conformance:10
    'date', _cfg->'date', -- fhirbase_conformance:12
    'software', _cfg->'software', -- fhirbase_conformance:13
    'fhirVersion', _cfg->'fhirVersion', -- fhirbase_conformance:14
    'acceptUnknown', _cfg->'acceptUnknown', -- fhirbase_conformance:15
    'format', _cfg->'format', -- fhirbase_conformance:16
    'rest', ARRAY[json_build_object( -- fhirbase_conformance:17
      'mode', 'server', -- fhirbase_conformance:18      
      'resource', -- fhirbase_conformance:21
        COALESCE((SELECT json_agg( -- fhirbase_conformance:22
            json_build_object( -- fhirbase_conformance:23
              'type', r.logical_id, -- fhirbase_conformance:24
              'readHistory', true, -- fhirbase_conformance:28
              'updateCreate', true, -- fhirbase_conformance:29
              'interaction', ARRAY['{ "code": "read" }'::json, '{ "code": "vread" }'::json, '{ "code": "update" }'::json, '{ "code": "history-instance" }'::json, '{ "code": "create" }'::json, '{ "code": "history-type" }'::json], -- fhirbase_conformance:30
              'searchParam',  ( -- fhirbase_conformance:31
                SELECT  json_agg(json_build_object(
	'name', sp.name,
	'type', sp.search_type	
) ) FROM searchparameter sp -- fhirbase_conformance:32
                WHERE sp.base = r.logical_id and sp.search_type in ('number','date', 'string', 'token', 'reference', 'composite', 'quantity', 'uri')  -- fhirbase_conformance:33
              ) -- fhirbase_conformance:34
            ) -- fhirbase_conformance:35
          ) -- fhirbase_conformance:36
          FROM structuredefinition r -- fhirbase_conformance:37
          WHERE r.installed = true and r.logical_id not in ('Resource') -- fhirbase_conformance:38
        ), '[]'::json) -- fhirbase_conformance:39
    )] -- fhirbase_conformance:40
  )::jsonb -- fhirbase_conformance:41
$BODY$
  LANGUAGE sql IMMUTABLE