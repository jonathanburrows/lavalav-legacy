import { Navigatable } from './navigatable.decorator';

const navigatableComponentMetadata = {
    group: 'my-group',
    title: 'My Page',
    icon: 'search'
};

@Navigatable(navigatableComponentMetadata)
class NavigatableComponent { }

describe(Navigatable.name, () => {
    it('will store the metadata with the Navigatable key', () => {
        const metadata = Reflect.getOwnMetadata(Navigatable.name, NavigatableComponent);

        expect(metadata).toBe(navigatableComponentMetadata);
    });
});
