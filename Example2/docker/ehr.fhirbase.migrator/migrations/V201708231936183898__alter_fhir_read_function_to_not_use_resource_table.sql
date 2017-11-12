CREATE OR REPLACE FUNCTION fhir.read(
	_resource_type_ text,
	_id_ text)
    RETURNS jsonb
    LANGUAGE 'plpgsql'

AS $BODY$
DECLARE
  _result_ jsonb;
BEGIN
  EXECUTE format(E'
   SELECT COALESCE( 
     (SELECT content FROM %1$I WHERE logical_id = %2$L limit 1),
     fhirbase_crud._simple_outcome(\'error\', \'404\', \'Not Found\', \'Resource %1$I with id = %2$s not found\')
     )', lower(_resource_type_), fhirbase_crud._extract_id(_id_)) INTO _result_;
  RETURN _result_;
END;
$BODY$;




