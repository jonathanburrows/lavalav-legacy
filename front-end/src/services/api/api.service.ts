import { Injectable, Type } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { Entity, IAggregateRoot } from '../../models';
import { HeadersService } from '../headers';
import { FrontEndOptions } from '../front-end-options';

/**
 *  Communicates with the Api endpoints on the configured resource server.
 */
@Injectable()
export class ApiService {
    constructor(private http: Http, private headersService: HeadersService, private options: FrontEndOptions) { }

    /**
     *  Returns a list of all entities of a given type from the resource server.
     *  @param entityType The type of entities which will be returned.
     *  @returns {Observable<TEntity[]>} all entities of matching type.
     *  @throws {Error} entityType is null.
     *  @throws {Error} the request failed.
     */
    public getCollection<TEntity extends Entity, IAggregateRoot>(entityType: Type<TEntity>): Observable<TEntity[]> {
        if (!entityType) {
            throw new Error('entityType is null.');
        }

        const name = this.getTypeName(entityType);
        const url = `${this.options.resourceServerUrl}/api/${name}`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers }).map(response => {
            if (!response) {
                throw new Error(`GET ${url} returned null.`);
            }

            if (response.status < 200 || response.status >= 300) {
                throw new Error(`GET ${url} failed with ${response.status} ${response.text()}`);
            }

            const deserialized = response.json();
            if (!deserialized) {
                throw new Error(`GET ${url} returned null.`);
            }

            if (!deserialized.length) {
                throw new Error(`GET ${url} did not return an array.`);
            }

            return deserialized.map(entity => new entityType(entity));
        });
    }

    /**
     *  Returns the entity of a given type with the given identifier.
     *  @param entityType The type of entity which is being requested.
     *  @param id The identifier for the desired entity.
     *  @returns {Observable<TEntity>} the matching entity.
     *  @throws {Error} entityType is null.
     *  @throws {Error} id is null.
     *  @throws {Error} the request failed.
     */
    public get<TEntity extends Entity, IAggregateRoot>(entityType: Type<TEntity>, id: number): Observable<TEntity> {
        if (!entityType) {
            throw new Error('entityType is null.');
        }
        if (!id) {
            throw new Error('id is null');
        }

        const name = this.getTypeName(entityType);
        const url = `${this.options.resourceServerUrl}/api/${name}/${id}`;
        const headers = this.headersService.getHeaders();

        return this.http.get(url, { headers: headers }).map(response => {
            if (!response) {
                throw new Error(`GET ${url} returned null.`);
            }

            if (response.status < 200 || response.status >= 300) {
                throw new Error(`GET ${url} failed with ${response.status} ${response.text()}`);
            }

            const deserialized = response.json();
            if (!deserialized) {
                throw new Error(`GET ${url} returned null.`);
            }

            return new entityType(deserialized);
        });
    }

    /**
     *  Creates an entity on the resource server and returns an observable for the created record.
     *  @param entityType The type of entity which is being created.
     *  @param creating The entity which will be created
     *  @remarks The created entity will have the generated id and may have other changes.
     *  @throws {Error} entityType is null.
     *  @throws {Error} updating is null.
     *  @throws {Error} the request failed.
     */
    public create<TEntity extends Entity, IAggregateRoot>(entityType: Type<TEntity>, creating: TEntity): Observable<TEntity> {
        if (!entityType) {
            throw new Error('entityType is null.');
        }
        if (!creating) {
            throw new Error('creating is null.');
        }

        const name = this.getTypeName(entityType);
        const url = `${this.options.resourceServerUrl}/api/${name}`;
        const headers = this.headersService.getHeaders();

        return this.http.post(url, creating, { headers: headers }).map(response => {
            if (!response) {
                throw new Error(`POST ${url} returned null.`);
            }

            if (response.status < 200 || response.status >= 300) {
                throw new Error(`POST ${url} failed with ${response.status} ${response.text()}`);
            }

            const deserialized = response.json();
            if (!deserialized) {
                throw new Error(`POST ${url} returned null.`);
            }

            return new entityType(deserialized);
        });
    }

    /**
     *  Updates an entity on the resource server and returns an observable for the updated record.
     *  @param entityType The type of entity which is being updated.
     *  @param updating The entity which will be updated
     *  @remarks The updated entity may have changes from the server.
     *  @throws {Error} entityType is null.
     *  @throws {Error} updating is null.
     *  @throws {Error} the request failed.
     */
    public update<TEntity extends Entity, IAggregateRoot>(entityType: Type<TEntity>, updating: TEntity): Observable<TEntity> {
        if (!entityType) {
            throw new Error('entityType is null.');
        }
        if (!updating) {
            throw new Error('updating is null.');
        }

        const name = this.getTypeName(entityType);
        const url = `${this.options.resourceServerUrl}/api/${name}`;
        const headers = this.headersService.getHeaders();

        return this.http.put(url, updating, { headers: headers }).map(response => {
            if (!response) {
                throw new Error(`PUT ${url} returned null.`);
            }

            if (response.status < 200 || response.status >= 300) {
                throw new Error(`DELETE ${url} failed with ${response.status} ${response.text()}`);
            }

            const deserialized = response.json();
            if (!deserialized) {
                throw new Error(`PUT ${url} returned null.`);
            }

            return new entityType(deserialized);
        });
    }

    /**
     *  Deletes an entity on the resource server and returns an observable for the removed record.
     *  @param entityType The type of entity which is being removed.
     *  @param deleting The entity which will be updated
     *  @throws {Error} entityType is null.
     *  @throws {Error} deleting is null.
     *  @throws {Error} the request failed.
     */
    public delete<TEntity extends Entity, IAggregateRoot>(entityType: Type<TEntity>, deleting: TEntity): Observable<TEntity> {
        if (!entityType) {
            throw new Error('entityType is null.');
        }
        if (!deleting) {
            throw new Error('deleting is null.');
        }

        const name = this.getTypeName(entityType);
        const url = `${this.options.resourceServerUrl}/api/${name}`;
        const headers = this.headersService.getHeaders();

        return this.http.post(url, deleting, { headers: headers }).map(response => {
            if (!response) {
                return null;
            }

            const deserialized = response.json();
            if (!deserialized) {
                return null;
            }

            return new entityType(deserialized);
        });
    }

    private getTypeName<TEntity>(entityType: Type<TEntity>): string {
        const entityFunction = <any>entityType;
        return entityFunction.name;
    }

    private handleFailedRequest(error: any) {
        throw new Error(error);
    }
}
