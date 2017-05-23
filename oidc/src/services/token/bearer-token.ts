/**
 *  Contains information about the user.
 */
export interface BearerToken {
    /** Token which can be used to authenticate the user. */
    access_token: string;

    /** Always Bearer token, added for compliance with openid spec. */
    token_type: string;

    /** Token which can be exchanged for a new access token. */
    refresh_token?: string;

    /** Contains claims relating to the user. */
    id_token: string;

    /** Client set value to protect against reply attacks. */
    state: string;

    /** Seconds to the expiration (since first issued). */
    expires_in?: number;

    /** Authorization code. */
    code?: string;

    /** Denotes an error that occured during issuing. */
    error?: string;
}
