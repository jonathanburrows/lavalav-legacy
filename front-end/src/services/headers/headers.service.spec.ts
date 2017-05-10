import { HeadersService } from './headers.service';

describe(HeadersService.name, () => {
    const headersService = new HeadersService();

    it('will set content-type to application/json', () => {
        const headers = headersService.getHeaders();

        const contentType = headers.get('content-type');
        expect(contentType).toBe('application/json');
    });

    it('will set Accept to application/json', () => {
        const headers = headersService.getHeaders();

        const accept = headers.get('accept');
        expect(accept).toBe('application/json');
    });
});
