import {
    CreditCard,
    EmailAddress,
    MaxLength,
    MinLength,
    Phone,
    Range,
    RegularExpression,
    Required
} from '../../src';

export class ValidationModel {
    @CreditCard() creditCard: string;

    @EmailAddress() emailAddress: string;

    @MaxLength(10) maxLength: string;

    @MinLength(3) minLength: string;

    @Phone() phone: number;

    @Range(1, 10) range: number;

    @RegularExpression(/hello, world!/) regularExpression: string;

    @Required() required: string;
}
