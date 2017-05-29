# Recover Username Component

## Goals
Provide a way for users to get an email with their username, from their email.



## Syntax
```html
<lvl-oidc-recover-username><lvl-oidc-recover-username>
```



## Requirements
It is expected that the component:

Has an email input, that:
1. Is required
2. Will display a Required validation message if left with no value
3. Will display a Required validation if submitted with no value
4. Will display a validation message if submitted with an invalid email
5. Will display "No user with that email" validation message if submitted with an email that no user has



It has a Recover button.


It will display validation messages when the recover button is pressed with a bad email.


It will send an email with the username when the recover button is pressed with a valid email.


It will show a confirmation message when the recover button is pressed with a valid email.



## Remarks
By default, the route /oidc/recover-username maps to this component.