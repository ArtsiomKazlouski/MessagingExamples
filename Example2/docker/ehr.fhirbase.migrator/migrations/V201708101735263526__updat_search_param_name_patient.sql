--1
ALTER TABLE searchparameter ADD COLUMN sub_path text[] default '{}';

--2
update searchparameter set sub_path = '{"family", "given","text", "prefix", "suffix"}' where logical_id = 'Patient-name';

--3
alter type query_param add attribute field_sub_path text[];

--4
CREATE OR REPLACE FUNCTION fhirbase_search._expand_search_params(
    _resource_type text,
    _query text)
  RETURNS SETOF query_param AS
$BODY$
  -- this recursive function collect metainformation for -- fhirbase_search:41
  -- chained params -- fhirbase_search:42
  WITH RECURSIVE params(parent_resource, link_path, res, chain, key, operator, value) AS ( -- fhirbase_search:43
    -- this is inital select -- fhirbase_search:44
    -- it produce some rows where key length > 1 -- fhirbase_search:45
    -- and we expand them joining meta inforamtion -- fhirbase_search:46
    SELECT null::text as parent_resource, -- we start with empty parent resoure -- fhirbase_search:47
           '{}'::text[] as link_path, -- path of reference attribute to join -- fhirbase_search:48
           _resource_type::text as res, -- this is resource to apply condition -- fhirbase_search:49
           ARRAY[_resource_type]::text[] || key as chain, -- initial chain -- fhirbase_search:50
           key as key, -- fhirbase_search:51
           operator as operator, -- fhirbase_search:52
           value as value -- fhirbase_search:53
    FROM fhirbase_params._parse_param(_query) -- fhirbase_search:54
    WHERE key[1] NOT IN ('_tag', '_security', '_profile', '_sort', '_count', '_page') -- fhirbase_search:55
    UNION -- fhirbase_search:57
    SELECT res as parent_resource, -- move res to parent_resource -- fhirbase_search:59
           fhirbase_coll._rest(ri.path) as link_path, -- remove first element -- fhirbase_search:60
           fhirbase_search.get_reference_type(x.key[1], re.ref_type) as res, -- set next res in chain -- fhirbase_search:61
           x.chain AS chain, -- save search path -- fhirbase_search:62
           fhirbase_coll._rest(x.key) AS key, -- remove first item from key untill only one key left -- fhirbase_search:63
           x.operator, -- fhirbase_search:64
           x.value -- fhirbase_search:65
     FROM  params x -- fhirbase_search:66
     JOIN  searchparameter ri -- fhirbase_search:67
       ON  ri.name = split_part(key[1], ':',1) -- fhirbase_search:68
      AND  ri.base = x.res -- fhirbase_search:69
     JOIN  structuredefinition_elements re -- fhirbase_search:70
       ON  re.path = ri.path -- fhirbase_search:71
    WHERE array_length(key,1) > 1 -- fhirbase_search:72
  ) -- fhirbase_search:73
  SELECT -- fhirbase_search:74
    parent_resource as parent_resource, -- fhirbase_search:75
    link_path as link_path, -- fhirbase_search:76
    res as resource_type, -- fhirbase_search:77
    fhirbase_coll._butlast(p.chain) as chain, -- fhirbase_search:78
    ri.search_type, -- fhirbase_search:79
    ri.is_primitive, -- fhirbase_search:80
    ri.type, -- fhirbase_search:81
    fhirbase_coll._rest(ri.path)::text[] as field_path, -- fhirbase_search:82
    fhirbase_coll._last(key) as key, -- fhirbase_search:83
    operator, -- fhirbase_search:84
    value, -- fhirbase_search:85
    ri.sub_path as field_sub_path
  FROM params p -- fhirbase_search:86
  JOIN searchparameter ri -- fhirbase_search:87
    ON ri.base = res -- fhirbase_search:88
   AND ri.name = key[1] -- fhirbase_search:89
 where array_length(key,1) = 1 -- fhirbase_search:90
  ORDER by p.chain -- fhirbase_search:91
$BODY$
LANGUAGE sql IMMUTABLE;

--5
CREATE OR REPLACE FUNCTION fhirbase_search.build_string_cond_ilike(
    tbl text,
    _q query_param)
  RETURNS text AS
$BODY$
  -- this function build condition to search string using ilike -- fhirbase_search:99
  -- expected trigram index on expression -- fhirbase_search:100
  -- (index_as_string(content, '{name}') ilike '%term%' OR index_as_string(content,'{name}') ilike '%term2') -- fhirbase_search:101
  SELECT '(' || string_agg( -- fhirbase_search:102
      format('fhirbase_idx_fns.index_as_string(%I.content, %L) ilike %L', tbl, case when sub_path.field is null then _q.field_path  else _q.field_path || sub_path.field end, '%' || x || '%'), -- fhirbase_search:103
      ' OR ') || ')' -- fhirbase_search:104
  FROM unnest(_q.value) x 
  left join unnest(_q.field_sub_path) sub_path(field) on(true)
$BODY$
  LANGUAGE sql IMMUTABLE;