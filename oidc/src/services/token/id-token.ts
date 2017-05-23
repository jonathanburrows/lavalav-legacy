export interface IdToken {
    /** Identifier for the issuer of the response. */
    iss: string;

    /** A locally unique, never changing identifier for the user. */
    sub: string;

    /** Audiences that the token is intended for. */
    aud: string;

    /** Expiration time on or after which the ID Token must not be accepted. */
    exp: number;

    /** Time that the token was issued. */
    iat: number;

    /** Time when the end user authentication occurred. */
    auth_time?: string;

    /** Associates a client session with an ID token to mitigate replay attacks. */
    nonce?: string;

    /** Signifies that the authentication performed has been satisfied. */
    acr?: string;

    /** JSON array of authentication methods used. */
    amr?: string;

    /** Party to which the ID token was issued, containing the id token. */
    azp?: string;
}
