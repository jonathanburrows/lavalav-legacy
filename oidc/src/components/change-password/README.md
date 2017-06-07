# Change Password Component

## Goals
Provide a way for users to change their password, so they can secure their data if their password is compromised.




## Syntax
```html
<lvl-oidc-change-password></lvl-oidc-change-password>
```



## Requirements
It is expected that the component:

Has an old password input, that:
1. Is required
2. Will show a required message if submitted without a value
3. Will show a validation message if the password does not match
4. Will show a validation message if the user logged out before submitting


Has a new password input, that:
1. Is required
2. Will show a required message if submitted without a value


Will have a save button.


Will allow the user to re-log in using their updated password.


Will prevent the user from relogging in using their old password.



## Roles
Only users with the 'oidc' role can access this component.



## Menu
This will not appear on the menu.



## Remarks
By default, the route /oidc/change-password maps to this component