import {
    Component,
    Input,
    OnInit,
    OnChanges
} from '@angular/core';
import { FormBuilder } from '@angular/forms';

import { ValidatableForm } from '@lvl/front-end';
import { User, ClaimEntity } from '../../models';
import { OidcOptions, UserAdministrationService } from '../../services';

/**
 *  Will allow an administrator to update a user's roles, so that they can access different parts of the application.
 */
@Component({
    selector: 'lvl-oidc-user-roles',
    templateUrl: 'user-roles.component.html',
    styleUrls: ['user-roles.component.scss']
})
export class UserRolesComponent implements OnInit, OnChanges {
    @Input() model: User;
    form: ValidatableForm<User>;

    constructor(
        public options: OidcOptions,
        private formBuilder: FormBuilder,
        private userAdministrationService: UserAdministrationService) {
        this.options.roles.sort();
    }

    ngOnInit() {
        if (!this.model) {
            throw new Error(`${UserRolesComponent.name} was created without a model.`);
        }

        this.form = new ValidatableForm<User>({
            selectedRoles: this.formBuilder.control([])
        });
        this.updateSelectedRoles();
    }

    ngOnChanges() {
        if (this.form) {
            this.updateSelectedRoles();
        }
    }

    /**
     *  Updates the checkboxes with what is in the user's claims.
     */
    private updateSelectedRoles() {
        const claims = this.model.claims || [];
        const userRoles = claims.filter(claim => claim.type === 'role');

        const selectedRoles = this.options.roles.filter(role => userRoles.some(userRole => role === userRole.value));

        const roleControl = this.form.get('selectedRoles');
        roleControl.setValue(selectedRoles);
    }

    /**
     *  Will save the user's roles against the server.
     *  @remarks
     *      Will not alter any claims which are not of type role.
     *      Will not alter any roles which are not in oidcOptions.roles.
     */
    save() {
        const claims = this.model.claims || [];
        const unrelatedClaims = claims.filter(c => c.type !== 'role' || this.options.roles.indexOf(c.value) === -1);

        const selectedRoles = this.form.get('selectedRoles').value;
        const selectedClaims = selectedRoles.map(role => new ClaimEntity({ type: 'role', value: role }));

        this.model.claims = unrelatedClaims.concat(selectedClaims);

        this.form.saveAsync(() => this.userAdministrationService.update(this.model));
    }
}
