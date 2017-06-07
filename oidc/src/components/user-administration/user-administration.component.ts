import { ApplicationRef, Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { Navigatable, ValidatableForm, ValidationBuilder } from '@lvl/front-end';
import { User } from '../../models';
import { UserAdministrationService } from '../../services';

/**
 *  Master/detail for managing users, and updating their roles.
 */
@Component({
    selector: 'lvl-oidc-user-administration',
    templateUrl: 'user-administration.component.html'
})
@Navigatable({
    group: 'Administration',
    title: 'User Admin',
    icon: 'security',
    roles: ['administrator']
})
export class UserAdministrationComponent implements OnInit {
    /** Used by odata query. */
    private $top = 7;
    private $fields = ['Id', 'Username', 'Claims'];

    /** Allows access to the user entered search value.*/
    public searchModel = new SearchModel();

    /* Items displayed on the master list. */
    public users: User[] = [];

    /** Item in focus in the detail panel. */
    public selectedUser: User;
    /** This was seperated from user so highlighting can be done before loading the record. */
    public selectedUserId: number;

    constructor(
        private userAdministrationService: UserAdministrationService,
        private router: Router,
        private location: Location,
        private activatedRoute: ActivatedRoute,
        private applicationRef: ApplicationRef) { }

    ngOnInit() {
        const userId = +this.activatedRoute.snapshot.params.id;
        this.selectUser(userId);

        // Check if the user has added a filter.
        const userProvidedQuery = this.router.parseUrl(this.router.url).queryParams;
        const userProvidedFilter = userProvidedQuery['$filter'];
        const substringPattern = /substringof\(Username, '(.*)'\)/g;

        const filter = substringPattern.exec(userProvidedFilter);

        if (filter) {
            this.searchModel.filter = filter[1];
        }

        this.applyFilter();

        this.location.subscribe(_ => { this.ngOnInit(); this.applicationRef.tick(); });
    }

    /**
     *  Will apply an odata query, and update the master list with the details.
     */
    applyFilter() {
        const query = {
            $top: this.$top,
            $select: this.$fields.join(','),
            $orderby: 'Username'
        };

        if (this.searchModel.filter) {
            query['$filter'] = `substringof(Username, '${this.searchModel.filter}') eq true`;
        }

        // Dont update the browser history, it will add too many urls.
        this.location.replaceState(this.getUrl());

        this.userAdministrationService.query(query).subscribe(result => {
            this.users = result.value.map(v => new User(v));
        });
    }

    /**
     *  Will return First/Last name, if they have no claims, the subject is returned.
     */
    public getNameOfUser(user: User) {
        const firstName = user.claims.find(c => c.type === 'given_name');
        const lastName = user.claims.find(c => c.type === 'family_name');

        if (firstName && lastName) {
            return `${firstName.value} ${lastName.value}`;
        } else if (firstName) {
            return firstName.value;
        } else if (lastName) {
            return lastName.value;
        } else {
            return user.subjectId;
        }
    }

    /**
     *  Updates the selected user, and updates the detail panel with the new entity.
     *  @param userId the primary key of the user.
     */
    selectUser(userId: number) {
        if (!userId) {
            this.selectedUser = null;
            this.selectedUserId = null;
        } else if (this.selectedUserId !== userId) {
            this.selectedUserId = userId;

            // Be sure to add the updated url to the history, for easy navigation.
            this.location.go(this.getUrl());

            this.userAdministrationService.get(userId).subscribe(user => this.selectedUser = user);
        }
    }

    /**
     *  Will construct a url based on the selected user, and what has been searched.
     */
    private getUrl() {
        const originalPath = document.location.pathname;
        const pathWithoutParam = originalPath.replace(/\/\d*$/g, '');

        const idSection = this.selectedUserId ? `/${this.selectedUserId}` : '';

        const filterSection = this.searchModel.filter ? `?$filter=substringof\(Username, '${this.searchModel.filter}'\)` : '';

        return pathWithoutParam + idSection + filterSection;
    }
}

class SearchModel {
    filter: string;
}
