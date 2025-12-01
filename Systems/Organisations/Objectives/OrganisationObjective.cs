using System;

namespace Warlord.Organisations.Objectives
{
    /// <summary> A goal that the organisation wishes to achieve. </summary>
    public abstract class OrganisationObjective : IComparable<OrganisationObjective>
    {
        /// <summary> The objective's priority. Lower is higher. </summary>
        public Int32 Priority { get; private set; }




        //public abstract void Start(OrganisationController controller);

        //public abstract void Stop(OrganisationController controller);


        /// <inheritdoc/>
        public Int32 CompareTo(OrganisationObjective? other)
        {
            if (other == null) { return 1; }
            return Priority.CompareTo(other.Priority);
        }
    }
}
