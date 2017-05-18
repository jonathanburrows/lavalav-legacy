namespace lvl.Oidc.AuthorizationServer.ViewModels
{
    /// <summary>
    ///     Information on how to connect to an external authenticator.
    /// </summary>
    /// <remarks>
    ///     IMPORTANT: keep these values in the application secrets. Not the config file.
    /// </remarks>
    public class ExternalApplicationInformation
    {
        /// <summary>
        ///     The app identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The secret used to authenticate the identifier.
        /// </summary>
        public string Secret { get; set; }
    }
}
