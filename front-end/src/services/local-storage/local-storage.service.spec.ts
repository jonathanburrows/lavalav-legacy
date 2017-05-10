import { LocalStorageService } from './local-storage.service';

// These tests will ensure that LocalStorageService calls LocalStorage's methods.
describe(LocalStorageService.name, () => {
    const localStorageService = new LocalStorageService();

    it('propogates setItem', () => {
        const item = { hello: 'world!' };

        localStorageService.setItem('__key', JSON.stringify(item));

        const savedItem = JSON.parse(localStorage.getItem('__key'));
        expect(savedItem.hello).toBe(item.hello);
    });

    it('propogates getItem', () => {
        const savedItem = { hello: 'world!' };
        localStorage.setItem('__key', JSON.stringify(savedItem));

        const item = JSON.parse(localStorageService.getItem('__key'));

        expect(item.hello).toBe(savedItem.hello);
    });

    it('propogates key', () => {
        localStorage.setItem('__key', JSON.stringify({ hello: 'world!' }));

        expect(localStorageService.key(0)).toBe(localStorage.key(0));
    });

    it('propogates removeItem', () => {
        localStorage.setItem('__key', JSON.stringify({ hello: 'world!' }));

        localStorageService.removeItem('__key');

        const item = localStorageService.getItem('__key');
        expect(item).toBeNull();
    });

    it('propogates clear', () => {
        localStorage.setItem('__key', JSON.stringify({ hello: 'world!' }));

        localStorageService.clear();

        const item = localStorageService.getItem('__key');
        expect(item).toBeNull();
    });

    it('propogates length', () => {
        localStorage.setItem('__key', JSON.stringify({ hello: 'world!' }));

        localStorageService.clear();

        expect(localStorageService.length).toBe(localStorage.length);
    });
});
