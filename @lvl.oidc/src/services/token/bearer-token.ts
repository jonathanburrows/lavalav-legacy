export interface BearerToken {
    access_token: string;

    token_type: string;

    refresh_token?: string;

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
