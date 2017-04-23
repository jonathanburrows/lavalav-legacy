import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';

@Injectable()
export class HeadersService {
    public getHeaders() {
        return new Headers({
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        });
    }
}