SELECT fhir.index_resource('MedicationDispense');
CREATE INDEX fb_medicationdispense_whenhandedover_date_idx
  ON public.medicationdispense
  USING gist
  (fhirbase_date_idx.index_as_date(content, '{whenHandedOver}'::text[], 'dateTime'::text));