using lvl.Repositories.Authorization;
using NHibernate;
using System;

namespace lvl.Repositories
{
    /// <summary>
    /// This was implemented as SQLite needs to have the same session used for all requests,
    /// or the database will disappear. 
    /// </summary>
    public class SessionProvider
    {
        protected ISessionFactory SessionFactory { get; }
        protected IInterceptor Interceptor { get; }

        public SessionProvider(ISessionFactory sessionFactory, IInterceptor interceptor)
        {
            SessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
            Interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
        }

        /// <summary>
        /// Constructs and returns a session.
        /// </summary>
        /// <returns>The constructed session</returns>
        public virtual ISession GetSession() => SessionFactory.OpenSession(Interceptor);
    }
}
