import { EmailAddress, Required } from '@lvl/front-end';

export class RecoverUsernameViewModel {
    @Required() @EmailAddress() email: string;
}
