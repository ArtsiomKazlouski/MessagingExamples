using System;
using System.Collections.Generic;
using System.Text;

namespace EHR.Cds.Hooks
{
    public class DisabilitySheetCdsSettings
    {

        public AvailableSickLeafDaysOptions AvailableSickLeafDaysOptions { get; set; }=new AvailableSickLeafDaysOptions();

        public EpisodeOfCareRequestOptions EpisodeOfCareRequestOptions { get; set; }=new EpisodeOfCareRequestOptions();

        public CareForSickFamilyMemberOptions CareForSickFamilyMemberOptions { get; set; } = new CareForSickFamilyMemberOptions();

        public CompositionStatementDateOptions CompositionStatementDateOptions { get; set; } = new CompositionStatementDateOptions();

        public ExcessTermContinuousPeriodDisabilityOptions ExcessTermContinuousPeriodDisabilityOptions { get; set; } = new ExcessTermContinuousPeriodDisabilityOptions();

    }


    public class AvailableSickLeafDaysOptions
    {
        public string HardStop { get; set; }= "Exceeding the maximum number of days";

        public string Succes { get; set; } = "All ok";

        /// <summary>
        /// 0 - left days; 1- maxDays
        /// </summary>
        public string WarningTemplate { get; set; } =
            "Left {0} days prior to exceeding the {1} days of disability for the last year";

        public IList<string> TuberculosisMkbDiagnosis { get; set; } =
            new List<string>() {"a15", "a16", "a17", "a18", "a19", "a15-a19"};

        public int DaysPriorToExcees { get; set; } = 10;

        public int MaximumTuberculosisDaysCount { get; set; } = 240;

        public int MaximumDaysCountWithoutTuberculosis { get; set; } = 150;

        public IEnumerable<string> AvailableKindDisabilityCodes { get; set; } = new string[]
        {
            "commonDisease", "occupationalDisease", "occupationalInjury", "householdInjury",
            "effectsOfHouseholdInjury", "effectsOfOccupationalInjury", "childCareTo3years", "prosthetics", "prostheticsDueToInjury", "quarantine "
        };

    }

    public class EpisodeOfCareRequestOptions
    {
        public string DoesNotHasOpenedSl { get; set; } = "Patient doesn't have opened SickLeaf on target period";

        public string HasOpenedSl { get; set; } = "Patient has opened SickLeaf on target period";

        public string HasOpenedSlInBegin { get; set; } = "Patient has opened SickLeaf in the beginig of target period";

        public int LeftPeriodIncrementForWarningDays { get; set; } = 1;

    }


    public class CareForSickFamilyMemberOptions
    {
      
    }

    public class CompositionStatementDateOptions
    {

    }

    public class ExcessTermContinuousPeriodDisabilityOptions
    {
        public IList<string> TuberculosisMkbDiagnosis { get; set; } =
            new List<string>() { "a15", "a16", "a17", "a18", "a19", "a15-a19" };

        public int DaysPriorToExcees { get; set; } = 10;

        public int MaximumTuberculosisDaysCount { get; set; } = 180;

        public int MaximumDaysCountWithoutTuberculosis { get; set; } = 120;

        public IEnumerable<string> AvailableKindDisabilityCodes { get; set; } = new string[]
        {
            "commonDisease", "occupationalDisease", "occupationalInjury", "householdInjury",
            "effectsOfHouseholdInjury", "effectsOfOccupationalInjury", "childCareTo3years", "prosthetics", "prostheticsDueToInjury", "quarantine"
        };
    }
}
