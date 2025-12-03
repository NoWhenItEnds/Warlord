namespace Warlord.Organisations.Objectives
{
    /// <summary> A goal that the organisation wishes to achieve. </summary>
    public abstract class OrganisationObjective
    {
        /// <summary> The organisation issuing the command. </summary>
        protected OrganisationController _organisation;


        /// <summary> A goal that the organisation wishes to achieve. </summary>
        /// <param name="organisation"> The organisation issuing the command. </param>
        public OrganisationObjective(OrganisationController organisation)
        {
            _organisation = organisation;
        }


        //public abstract void Start(OrganisationController controller);

        //public abstract void Stop(OrganisationController controller);
    }
}
