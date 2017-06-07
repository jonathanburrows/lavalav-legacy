import { Required } from '@lvl/front-end';

/**
 *  What the user wants their password to be, and what it was (for validation).
 */
export class ResetPasswordViewModel {
    /** The current password, to be compared before updating. */
    @Required() oldPassword: string;

    /** The password that the user wants. */
    @Required() newPassword: string;
}
