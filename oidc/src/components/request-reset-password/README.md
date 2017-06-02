# Request Reset Password Component

## Goals
Provide a way for users have an email be sent to them with a link to change their password, so they can re-access their accounts if they forget.


## Syntax
```html
<lvl-oidc-request-reset-password></lvl-oidc-request-reset-password>
```



## Requirements
The request reset password component is expected to:


Have a username input.


Show a required validation message if the username is unfocused without a value.


Show a required validation message if the form is submitted without a username.


Have a request reset button.


Show a validation message on the username input if there is no user with that username.


Show a validation message on the username input if the user does not have an associated email.


Remove the Request reset button if the request is successful.


Show a success message if the request is successful.



## Remarks
By defualt, the route /oidc/request-reset-password is mapped to this component.