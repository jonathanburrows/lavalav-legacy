import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';

/**
 *  Returns a new set of headers.
 *  @remarks - this was done so security implementations can attach bearer tokens, while still being consumed by api.service.
 */
@Injectable()
export class HeadersService {
    /**
     *  Returns a new set of headers, with content-type application/json, and Accept application/json
     */
    public getHeaders() {
        return new Headers({
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Data-Type': 'json'
        });
    }
}
