/**
 *  Will contain metadata to be used by the router.
 *  @param annotations The metadata which will affect routing.
 *  @throws {Error} annotations is null.
 *  @throws {Error} the class is not hidden from the menu, and it has no group.
 *  @throws {Error} the class is not hidden from the menu, and it has no title.
 */
export function Navigatable(annotations: NavigatableMetadata): ClassDecorator {
    return function(constructor: Function) {
        if (!annotations) {
            throw new Error(`options in  is null on ${constructor.name}'s @Navigatable()`);
        }
        if (!annotations.hideInMenu && !annotations.group) {
            /* tslint:disable:max-line-length */
            throw new Error(`hideInMenu is false and group is null, and group is required by the menu -- on ${constructor.name}'s @Navigatable()`);
        }
        if (!annotations.hideInMenu && !annotations.title) {
            throw new Error(`hideInMenu is false and title is null, and title is required by the menu -- on ${constructor.name}'s @Navigatable()`);
        }

        Reflect.defineMetadata(Navigatable.name, annotations, constructor);
    };
}

/** Information which will be used by the router. */
export interface NavigatableMetadata {
    /** If true, the component will not appear on the menu, otherwise, it will be hidden. */
    hideInMenu?: boolean;

    /**
     *  Will group navigation items, and place the link under this title.
     *  @remarks - if hideInMenu is false and this isnt specified, an error is thrown.
     */
    group?: string;

    /** Will display this on the menu link.
     *  @remarks - if hideInMenu is false and this isnt specified, an error is thrown.
     */
    title?: string;

    /** The text litagy of the icon to be displayed in the menu. Optional. */
    icon?: string;

    /**
     *  If specified, only users with one of the given roles can access this component.
     *  If not specified, then all users can access the component.
     *  @remarks - administrator will always have access to all roles.
     */
    roles?: string[];
}
