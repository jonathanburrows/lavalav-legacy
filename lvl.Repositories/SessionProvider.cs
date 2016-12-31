using NHibernate;

namespace lvl.Repositories
{
    /// <summary>
    /// This was implemented as SQLite needs to have the same session used for all requests,
    /// or the database will disappear. 
    /// </summary>
    public class SessionProvider
    {
        protected ISessionFactory SessionFactory { get; }

        public SessionProvider(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        /// <summary>
        /// Constructs and returns a session.
        /// </summary>
        /// <returns>The constructed session</returns>
        public virtual ISession GetSession() => SessionFactory.OpenSession();
    }
}
