import { EmailAddress, MaxLength, Phone } from '@lvl/front-end';

export class PersonalDetailsViewModel {
    @EmailAddress() @MaxLength(255) email: string;
    @MaxLength(255) firstName: string;
    @MaxLength(255) lastName: string;
    @Phone() phoneNumber: number;
    @MaxLength(255) job: string;
    @MaxLength(255) location: string;
}
