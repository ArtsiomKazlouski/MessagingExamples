select 
        *
from
        fhirbase_migration_201502270000_metadata_up.load_searchparameters('

{
  "resourceType": "Bundle",
  "entry": [
	  { 
	  	 "resource" : 
				  	 {
					    "base": "MedicationPrescription",
					    "id": "MedicationPrescription-validityPeriod",
					    "name": "validityPeriod",
					    "description": "validityPeriod",					   
					    "resourceType": "SearchParameter",
					    "type": "date",
						"target":["Period"],
					    "xpath": "f:MedicationPrescription/f:dispense/f:validityPeriod"
				}
	  }	
	
	 
  ]
}
        '::jsonb)
		
		
        