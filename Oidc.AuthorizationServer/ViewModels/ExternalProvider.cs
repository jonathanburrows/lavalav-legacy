namespace lvl.Oidc.AuthorizationServer.ViewModels
{
    /// <summary>
    ///     A 3rd party which can authenticate a user.
    /// </summary>
    public class ExternalProvider
    {
        /// <summary>
        ///     Title which will be shown in GUIs.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     The unique name of the provider.
        /// </summary>
        public string AuthenticationScheme { get; set; }
    }
}
