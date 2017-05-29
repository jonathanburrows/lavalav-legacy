# Resource Owner Sign In Component

## Goals
Provide a way for users to sign in using the resource owner flow, so they can have increased UX when dealing with trusted apps.



## Syntax
```html
<lvl-oidc-resource-owner-sign-in></lvl-oidc-resource-owner-sign-in>
```



## Requirements
The is expected that the resource owner sign in component:

Has a username input, that:
1. Is required
2. Has a link titled 'Forgot?'
3. Will redirect to the recover username page when the forgot link is pressed
4. Will display a Required validation message if left with no value
5. Will display a Required validation message if submitted with no value
6. Will display an 'Cant find user' validation message if non-existant username is submitted


Has a password input, that:
1. Is required
2. Has a link titled 'Forgot?'
3. Will redirect to the reset password page when the forgot link is pressed
4. Will display a Required validation message if left with no value
5. Will display a Required validation message if submitted with no value
6. Will display an 'Incorrect password' validation message if an existing user with invalid password is submitted


It has a Register button.


It will redirect to the register account page when the Register button is clicked.


It has a Sign-in button.


It will redirect off of the signin page when the sign-in button is pressed with valid credentials.


It will display validation messages when the sign-in button is pressed with bad credentials.



## Remarks
By default, the route /oidc/sign-in maps to this component.