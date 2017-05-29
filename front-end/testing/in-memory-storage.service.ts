import { Injectable } from '@angular/core';

import { StorageService } from '../src/services/storage/storage.service';

/**
 *  Provides a way for testing to clear itself between iterations.
 */
@Injectable()
export class InMemoryStorageService extends StorageService {
    private items: { [key: string]: string } = {};

    get length(): number {
        return Object.keys(this.items).length;
    }

    clear(): void {
        this.items = {};
    }

    getItem(key: string): any {
        return this.items[key];
    }

    key(index: number): string {
        return Object.keys(this.items)[index];
    }

    removeItem(key: string): void {
        delete this.items[key];
    }

    setItem(key: string, data: string): void {
        this.items[key] = data;
    }
}
