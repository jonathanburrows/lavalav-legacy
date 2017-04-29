using FluentNHibernate.Mapping;

namespace lvl.Ontology.Authorization
{
    public class OwnedByUserFilter: FilterDefinition
    {
        public OwnedByUserFilter()
        {
            WithName(nameof(OwnedByUserFilter));
            AddParameter("OwnedByUserId", NHibernate.NHibernateUtil.String);
            WithCondition("1=:OwnedByUserId");
        }
    }
}
