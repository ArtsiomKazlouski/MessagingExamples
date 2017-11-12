CREATE OR REPLACE FUNCTION fhirbase_search.build_search_query(
	_resource_type text,
	_query text)
    RETURNS text
    LANGUAGE 'sql'
AS $BODY$

  -- TODO add to documentation -- fhirbase_search:224
  -- custom sort param _sort:asc=a, _sort:desc=b => { _sort:["a:asc", "b:desc"] } -- fhirbase_search:225
  WITH conds AS ( -- fhirbase_search:226
    SELECT -- fhirbase_search:227
      fhirbase_search.build_cond(lower(resource_type),row(x.*))::text as cond, -- fhirbase_search:228
      resource_type, -- fhirbase_search:229
      chain, -- fhirbase_search:230
      link_path, -- fhirbase_search:231
      parent_resource -- fhirbase_search:232
    FROM  fhirbase_search._expand_search_params(_resource_type, _query) x -- fhirbase_search:233
  ), joins AS ( --TODO: what if no middle join present ie we have a.b.c.attr = x and no a.b.attr condition -- fhirbase_search:234
    SELECT -- fhirbase_search:235
      format(E'JOIN %I ON fhirbase_idx_fns.index_as_reference(%I.content, %L) && ARRAY[%I.logical_id]::text[] AND \n %s', -- fhirbase_search:236
        lower(resource_type), -- fhirbase_search:237
        lower(parent_resource), -- fhirbase_search:238
        link_path, -- fhirbase_search:239
        lower(resource_type), -- fhirbase_search:240
        string_agg(cond, E'\nAND\n')) as sql -- fhirbase_search:241
      FROM conds -- fhirbase_search:242
      WHERE parent_resource IS NOT NULL -- fhirbase_search:243
      GROUP BY resource_type, parent_resource, chain, link_path -- fhirbase_search:244
      ORDER by chain -- fhirbase_search:245
  ), special_params AS ( -- fhirbase_search:246
    SELECT key[1] as key, value[1] as value -- fhirbase_search:247
    FROM fhirbase_params._parse_param(_query) -- fhirbase_search:248
    where key[1] ilike '_%' -- fhirbase_search:249
  ) -- fhirbase_search:250
  SELECT -- fhirbase_search:252
  fhirbase_gen._tpl('SELECT {{r}}.version_id, {{r}}.logical_id, {{r}}.resource_type, {{r}}.updated, {{r}}.published, {{r}}.category, {{r}}.content FROM {{r}}', 'r',  quote_ident(lower(_resource_type))) -- fhirbase_search:253
  || E'\n' || COALESCE((SELECT string_agg(sql, E'\n')::text FROM joins), ' ') -- fhirbase_search:254
  || E'\nWHERE ' -- fhirbase_search:255
  || COALESCE((SELECT string_agg(cond, ' AND ') -- fhirbase_search:256
      FROM conds -- fhirbase_search:257
      WHERE parent_resource IS NULL -- fhirbase_search:258
      GROUP BY resource_type), ' true = true ') -- fhirbase_search:259
  || COALESCE(fhirbase_search.build_sorting(_resource_type, _query), '') -- fhirbase_search:260
  || format(E'\nLIMIT %s',COALESCE( (SELECT value::integer FROM special_params WHERE key = '_count'), '100')) -- fhirbase_search:261
  || format(E'\nOFFSET %s', -- fhirbase_search:262
    ( -- fhirbase_search:263
      COALESCE((SELECT value::integer FROM special_params WHERE key = '_page'), 0)::integer -- fhirbase_search:264
      * -- fhirbase_search:265
      COALESCE((SELECT value::integer FROM special_params WHERE key = '_count'), 1000)::integer -- fhirbase_search:266
    )) -- fhirbase_search:267

$BODY$;