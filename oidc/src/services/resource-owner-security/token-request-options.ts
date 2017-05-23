/** Details on getting a token from the authorization server using the resource owner flow. */
export interface TokenRequestOptions {
    /** Always bearer, added for compliance with the specification. */
    grant_type: string;

    /** Identifier of the resource owner client. */
    client_id?: string;

    /** Authenticates the identity of the client. */
    client_secret?: string;

    /** Specifies which privileges are required by the client. */
    scope?: string;

    /** Name of the user requesting an access token. */
    username?: string;

    /** Authenticates the identity of the user. */
    password?: string;

    /** Token which can be exchanged for an acces token instead of using credentials. */
    refresh_token?: string;
}
