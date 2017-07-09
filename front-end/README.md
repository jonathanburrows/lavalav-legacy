# front-end

## Goals
To provide a set of angular services and components that arent related to a specific business domain.



## Folder Structure.
When setting up an angular project, the following folder structure should be adhered

```
/demo-app
----/components
----/environments
----/models
----demo-app.module.ts
----global.scss
----index.html
----main.ts
/src
----/components
----/models
----/pipes
----/services
----/styles
----module-name.module.ts
----module-name.router.module.ts
----module-name.scss
```



## Config 
front-end.module has the following options:
```
{
    // Url to the api.
	resourceServerUrl: string,

    // required by angular cli
	production: boolean
}
```



## Service Architecture
(component and decorators have been omitted for brevity)
<img src="docs\service-architecture.png" />



## Remarks
Services/components related to a specific business domain should not be placed here.