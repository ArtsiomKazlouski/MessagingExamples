using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EHR.Cds.Infrastructure;
using EHR.Cds.Models;
using Microsoft.Extensions.Options;

namespace EHR.Cds.Hooks.Handlers
{
   

    /// <summary>
    /// Один листок нетрудоспособности для пациента одновременно:
    /// </summary>
    public class EpisodeOfCareRequestHandler:DisabilitySheetHandlerBase
    {
        private readonly EpisodeOfCareRequestOptions _options;
        private readonly IUnitOfWork _unitOfWork;
     
        public EpisodeOfCareRequestHandler(IUnitOfWork unitOfWork, IOptions<DisabilitySheetCdsSettings> options)
        {
            _options = options.Value.EpisodeOfCareRequestOptions;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override async Task<Card> HandleCore(DisabilitySheetHookContext context)
        {
         
            var sql = @"
                        SELECT
                            eoc.start as beginDate,
                            eoc.end as endDate
                        FROM episodeofcare  eoc
                        JOIN composition c on c.episodeofcare_id = eoc.id 
                        where c.status != 'entered-in-error' 
                                and eoc.status!='cancelled' 
                                and eoc.patient_id = @PatientId 
                                and (eoc.id!=@EpisodeOfCareId or @EpisodeOfCareId is null);
                        ";
          
            //temporary data
            var hardStopQueryParameters = new
            {
                context.PatientId,
                context.EpisodeOfCareId
            };

            var hardStopRange = new DateRange()
            {
                BeginDate = context.TreatmentStart,
                EndDate = context.TreatmentEnd
            };

            var existingDisabilitiesPeriods = (await _unitOfWork.QueryAsync(
                (connection, transaction) => connection.QueryAsync<DateRange>(sql, hardStopQueryParameters, transaction))).ToList();


            if (existingDisabilitiesPeriods.Any(t => t.Intersects(hardStopRange)))
            {
                return  new Card("Для Указанного листка нетрудоспособности существует листок с пересекающимся периодом", Indicator.HardStop, new Link("")) ;
            }

            var warningRange = new DateRange()
            {
                BeginDate = context.TreatmentStart.AddDays(-(_options.LeftPeriodIncrementForWarningDays+1)),
                EndDate = context.TreatmentEnd.AddDays(+(_options.LeftPeriodIncrementForWarningDays + 1))
            };

            if (existingDisabilitiesPeriods.Any(t => t.Intersects(warningRange)))
            {
                return new Card("Для Указанного листка нетрудоспособности существует листок с пересекающимся периодом", Indicator.Warning, new Link(""));
            }

            return new Card("Правило пройдено успешно", Indicator.Success,new Link(""));
        }

       
    }

    public class DateRange
    {
        public DateRange()
        {
            
        }

        public DateRange(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new Exception("Invalid range edges.");
            }
            BeginDate = start.Date;
            EndDate = end.Date;
        }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public static bool operator ==(DateRange range1, DateRange range2)
        {
            return range1.Equals(range2);
        }

        public static bool operator !=(DateRange range1, DateRange range2)
        {
            return !(range1 == range2);
        }
        public override bool Equals(object obj)
        {
            if (obj is DateRange)
            {
                var range1 = this;
                var range2 = (DateRange)obj;
                return range1.BeginDate.Date == range2.BeginDate.Date && range1.EndDate.Date == range2.EndDate.Date;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Querying
        public bool Intersects(DateRange range)
        {
            var type = GetIntersectionType(range);
            return type != IntersectionType.None;
        }
        public bool IsInRange(DateTime date)
        {
            return (date > this.BeginDate) && (date < this.EndDate);
        }
        public IntersectionType GetIntersectionType(DateRange range)
        {
            if (this == range)
            {
                return IntersectionType.RangesEqauled;
            }
            else if (IsInRange(range.BeginDate) && IsInRange(range.EndDate))
            {
                return IntersectionType.ContainedInRange;
            }
            else if (IsInRange(range.BeginDate))
            {
                return IntersectionType.StartsInRange;
            }
            else if (IsInRange(range.EndDate))
            {
                return IntersectionType.EndsInRange;
            }
            else if (range.IsInRange(this.BeginDate) && range.IsInRange(this.EndDate))
            {
                return IntersectionType.ContainsRange;
            }
            return IntersectionType.None;
        }
        public DateRange GetIntersection(DateRange range)
        {
            var type = this.GetIntersectionType(range);
            if (type == IntersectionType.RangesEqauled || type == IntersectionType.ContainedInRange)
            {
                return range;
            }
            else if (type == IntersectionType.StartsInRange)
            {
                return new DateRange(range.BeginDate, this.EndDate);
            }
            else if (type == IntersectionType.EndsInRange)
            {
                return new DateRange(this.BeginDate, range.EndDate);
            }
            else if (type == IntersectionType.ContainsRange)
            {
                return this;
            }
            else
            {
                return default(DateRange);
            }
        }
        #endregion


        public override string ToString()
        {
            return BeginDate.ToString() + " - " + EndDate.ToString();
        }


        public TimeSpan Period()
        {
            return EndDate - BeginDate;
        }
    }

    public enum IntersectionType
    {
        /// <summary>
        /// No Intersection
        /// </summary>
        None = -1,
        /// <summary>
        /// Given range ends inside the range
        /// </summary>
        EndsInRange,
        /// <summary>
        /// Given range starts inside the range
        /// </summary>
        StartsInRange,
        /// <summary>
        /// Both ranges are equaled
        /// </summary>
        RangesEqauled,
        /// <summary>
        /// Given range contained in the range
        /// </summary>
        ContainedInRange,
        /// <summary>
        /// Given range contains the range
        /// </summary>
        ContainsRange,
    }

}
