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
					    "base": "Claim",
					    "contact": [
					        {
					            "telecom": [
					                {
					                    "system": "url",
					                    "value": "http://hl7.org/fhir"
					                }
					            ]
					        },
					        {
					            "telecom": [
					                {
					                    "system": "url",
					                    "value": "http://www.hl7.org/Special/committees/pafm/index.cfm"
					                }
					            ]
					        }
					    ],
					    "date": "2015-04-09T07:11:27+00:00",
					    "id": "Claim-created",
					    "name": "created",
					    "description": "Created",
					    "publisher": "HL7 FHIR Project (Patient Administration)",
					    "resourceType": "SearchParameter",
					    "type": "date",
					    "url": "http://hl7.org/fhir/SearchParameter/Claim-created",
					    "xpath": "f:Claim/f:created"
				}
	  }	,
	  { 
	  	 "resource" :
				{
				    "base": "Claim",
				    "contact": [
				        {
				            "telecom": [
				                {
				                    "system": "url",
				                    "value": "http://hl7.org/fhir"
				                }
				            ]
				        },
				        {
				            "telecom": [
				                {
				                    "system": "url",
				                    "value": "http://www.hl7.org/Special/committees/fm/index.cfm"
				                }
				            ]
				        }
				    ],
				    "date": "2015-04-09T07:11:27+00:00",
				    "description": "Organization",
				    "id": "Claim-organization",
				    "name": "organization",
				    "publisher": "HL7 FHIR Project (Financial Management)",
				    "resourceType": "SearchParameter",
				    "target": [
				        "Organization"
				    ],
				    "type": "reference",
				    "url": "http://hl7.org/fhir/SearchParameter/Claim-organization",
				    "xpath": "f:Claim/f:organization"
				}
	},
	{ 
	  	 "resource" :
				{
				   "id":"Claim-medicationprescription",
				   "url":"http://hl7.org/fhir/SearchParameter/Claim-medicationprescription",
				   "base":"Claim",
				   "date":"2015-04-09T07:11:27+00:00",
				   "name":"originalPrescription",
				   "type":"reference",
				   "xpath":"f:Claim/f:originalPrescription",
				   "target":[  
					  "MedicationPrescription"
				   ],
				   "contact":[  
					  {  
						 "telecom":[  
							{  
							   "value":"http://hl7.org/fhir",
							   "system":"url"
							}
						 ]
					  },
					  {  
						 "telecom":[  
							{  
							   "value":"http://www.hl7.org/Special/committees/medication/index.cfm",
							   "system":"url"
							}
						 ]
					  }
				   ],
				   "publisher":"HL7 FHIR Project (Pharmacy)",
				   "description":"",
				   "resourceType":"SearchParameter"
				}
	}
  ]
}
        '::jsonb)
		
		
        