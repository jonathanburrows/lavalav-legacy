# Account Menu Component

## Goals
Provide a menu for navigating to account forms which works on all devices, so that users can manage their accounts.



## Syntax
Intended to be used in a lvl-layout, and decorated with lvl-toolbar-action.
```html
<lvl-layout>
	<lvl-oidc-account-menu lvl-toolbar-action></lvl-oidc-account-menu>
</lvl-layout>
```


## Requirements
The account menu is expected to:

Be position in the application toolbar, on the right hand side.

Be visible when logged in.

Be hidden when not logged in.

Have a link to go to the personal details form, which will redirect the user when clicked.

Have a link to change the users password, which will redirect the user when clicked.

Have a link to sign out, which will log the user out and redirec them to the logic page when clicked.
