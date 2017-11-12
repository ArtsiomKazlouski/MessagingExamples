create materialized view composition_decomposition as 
select c.logical_id as composition_id, 
split_part((c.content #>> '{subject, reference}'), '/', 1) as subject_type,
split_part((c.content #>> '{subject, reference}'), '/', 2) as subject_id,
split_part((c.content #>> '{custodian, reference}'), '/', 2) as organization_id
from composition c;


CREATE  INDEX subject_type_idx
   ON composition_decomposition (subject_type);


CREATE OR REPLACE FUNCTION public.search_sick_leaf(searchterm text, _offset_ INTEGER, _limit_ integer)
  RETURNS TABLE(composition_id Text, sl_id text, iss TEXT, organization TEXT, full_name TEXT) AS
$BODY$

With ip as(
select
c.logical_id as composition_id, c.content as c, p.content as p, o.content as o
from composition_decomposition cr
join composition c on c.logical_id = cr.composition_id
join patient p on p.logical_id = cr.subject_id 
join organization o on o.logical_id = cr.organization_id
where 
(fhirbase_idx_fns.index_identifier_as_token(p.content, '{identifier}') && ARRAY[searchterm]::text[]) 
and cr.subject_type = 'Patient'
),
fp as(
select
c.logical_id as composition_id, c.content as c, p.content as p, o.content as o
from composition_decomposition cr
join composition c on c.logical_id = cr.composition_id
join patient p on p.logical_id = cr.subject_id 
join organization o on o.logical_id = cr.organization_id
where 
((fhirbase_idx_fns.index_as_string(p.content, '{name,family}') ilike '%'||searchterm||'%') ) 
and cr.subject_type = 'Patient'
),
icp as (
select
c.logical_id as composition_id, c.content as c, p.content as p, o.content as o
from composition_decomposition cr
join composition c on c.logical_id = cr.composition_id
join patient p on p.logical_id = cr.subject_id 
join organization o on o.logical_id = cr.organization_id
where 
(fhirbase_idx_fns.index_identifier_as_token(c.content, '{identifier}') && ARRAY[searchterm]::text[])
and cr.subject_type = 'Patient'
),
icr as (
select
c.logical_id as composition_id, c.content as c, p.content as p, o.content as o
from composition_decomposition cr
join composition c on c.logical_id = cr.composition_id
join relatedperson p on p.logical_id = cr.subject_id 
join organization o on o.logical_id = cr.organization_id
where 
(fhirbase_idx_fns.index_identifier_as_token(c.content, '{identifier}') && ARRAY[searchterm]::text[])
and cr.subject_type = 'RelatedPerson'
),
fr as (
select
c.logical_id as composition_id, c.content as c, p.content as p, o.content as o
from composition_decomposition cr
join composition c on c.logical_id = cr.composition_id
join relatedperson p on p.logical_id = cr.subject_id 
join organization o on o.logical_id = cr.organization_id
where 
(fhirbase_idx_fns.index_as_string(p.content, '{name,family}'::text[]) ilike '%'||searchterm||'%') 
and cr.subject_type = 'RelatedPerson'
),
ir as (
select
c.logical_id as composition_id, c.content as c, p.content as p, o.content as o
from composition_decomposition cr
join composition c on c.logical_id = cr.composition_id
join relatedperson p on p.logical_id = cr.subject_id 
join organization o on o.logical_id = cr.organization_id
where 
(fhirbase_idx_fns.index_identifier_as_token(p.content, '{identifier}') && ARRAY[searchterm]::text[])
and cr.subject_type = 'RelatedPerson'
),
r as (
select * from icr union all
select * from fr union all
select * from ir 
),
p as (
select * from ip union all
select * from fp union all
select * from icp
),
r_norm as (
select r.composition_id, r.c,r.o,r.p#>'{name}' as name from r
),
p_norm as (
select p.composition_id, p.c,p.o,p.p#>'{name,0}' as name from p
),
all_norm as (
select * from r_norm union all
select * from p_norm
)
select 
a.composition_id as composition_id,
a.c  #>> '{identifier, value}' as sl_id,
(select x#>> '{valueDate}' as sl_iss from json_array_elements((a.c #>> '{extension}')::json) x where x#>>'{url}' = 'http://fhir.org/fhir/StructureDefinition/by-disability-sheet-date-created') as iss,
a.o #>> '{name}' as organization,
Coalesce((a.name #>> '{family, 0}') || ' ','')  ||
Coalesce((a.name #>> '{given, 0}') || ' ','') ||
Coalesce((a.name #>> '{given, 1}'),'') as full_name
from all_norm a
offset _offset_
limit _limit_

$BODY$
  LANGUAGE sql IMMUTABLE;