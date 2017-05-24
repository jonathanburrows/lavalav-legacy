import { Required } from '@lvl/front-end';

/** Represents what a user will authenticate themselves. */
export class Credentials {
    @Required() username: string;
    @Required() password: string;
}
