using NHibernate;

namespace lvl.Repositories
{
    /// <summary>
    /// This was implemented as SQLite needs to have the same session used for all requests,
    /// or the database will disappear. 
    /// </summary>
    /// <remarks>Must handle the disposale of created sessions, since sessions may be persistent.</remarks>
    public class SessionManager
    {
        protected ISessionFactory SessionFactory { get; }

        public SessionManager(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        /// <summary>
        /// Constructs and returns a session.
        /// </summary>
        /// <returns>The constructed session</returns>
        public virtual ISession OpenSession() => SessionFactory.OpenSession();
    }
}
