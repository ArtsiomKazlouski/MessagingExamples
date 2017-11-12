update searchparameter s
set type = 'Reference' 
where s.base = 'Patient' and ('{careprovider, organization, link}'::Text[] && ARRAY[s.name]::text[]);

SELECT fhir.index_search_param('Patient','careprovider');

SELECT fhir.index_search_param('Patient','family');

SELECT fhir.index_search_param('Patient','gender');

SELECT fhir.index_search_param('Patient','given');

SELECT fhir.index_search_param('Patient','identifier');

SELECT fhir.index_search_param('Patient','link');

SELECT fhir.index_search_param('Patient','name');

SELECT fhir.index_search_param('Patient','organization');