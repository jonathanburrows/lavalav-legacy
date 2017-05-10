import { Injectable } from '@angular/core';

/**
 *  Abstract layer for storing items locally.
 *  @remarks - this was done to decouple local storage, so that it can be unit tested.
 */
@Injectable()
export abstract class StorageService implements Storage {
    // an error is thrown because typescript doesnt support abstract readonly properties.
    get length(): number { throw new Error('Not implemented.'); }

    [key: string]: any;
    [index: number]: string;

    abstract clear(): void;
    abstract getItem(key: string): any;
    abstract key(index: number): string;
    abstract removeItem(key: string): void;
    abstract setItem(key: string, data: string): void;
}
