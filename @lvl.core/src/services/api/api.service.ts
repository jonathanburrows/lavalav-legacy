import { Injectable, Type } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map';

import { CoreOptions } from '../core-options';
import { HeadersService } from '../headers';
import { IEntity } from '../../models';

@Injectable()
export class ApiService {
    constructor(private http: Http, private headersService: HeadersService, private coreOptions: CoreOptions) { }

    public getSingle<TEntity extends Object, IEntity>(entityType: Type<TEntity>, id: number): Observable<TEntity> {
        const name = this.getTypeName(entityType);
        const url = `${this.coreOptions.resourceServerUrl}/api/${name}/${id}`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers }).map(entity => entity.json());
    }

    public get<TEntity extends Object, IEntity>(entityType: Type<TEntity>): Observable<TEntity[]> {
        debugger;
        const name = this.getTypeName(entityType);
        const url = `${this.coreOptions.resourceServerUrl}/api/${name}`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers }).map(entities => entities.json());
    }

    private getTypeName<TEntity>(entityType: Type<TEntity>): string {
        const entityFunction = <any>entityType;
        return entityFunction.name;
    }
}
