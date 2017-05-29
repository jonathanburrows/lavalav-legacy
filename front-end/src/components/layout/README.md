# Layout Component

## Goals
Create a page structure which is compliant with material design, so applications can be made mobile friendly.



### Simple Example
```<lvl-layout siteTitle="my-site"></lvl-layout>```



### Api

#### Inputs

| Name | Type | Description |
| :--- | :--- | :--- |
| siteTitle (optional) | string | The name which will appear on the toolbar |



### Toolbar Actions
To have buttons show up on the right side of the application toolbar, they must have a [lvl-toolbar-action] attribute:
```html
	<lvl-layout siteTitle="my-site">
		<lvl-search-action lvl-toolbar-action></lvl-search-action>
		<div lvl-toolbar-action>my special action</div>
	</lvl-layout>
```



### Side Nav Requirements
It is expected that there is a side panel which contains all the navigation links. It is expected that it will:
1. have a title section, which is the same height as the app toolbar
2. have the site name, which will be populated with the siteName input
3. throw an error if a navigation link has no group
4. create a title for each group
5. create a link for each navigation item
6. render an icon for a navigation item if it has one
7. not render an icon for a navigation item if it has none
8. automatically display for screens which are desktops
9. load hidden for mobile devices and tablets
10. expand on mobile devices when the hamburger menu is pressed
11. hide on mobile devices when an area on the screen is pressed



### Toolbar Requirements
It is expected that there is an application toolbar which contains application related controls. It is expected that it will:
1. have a hamburger button on mobile devices and tablets
2. have no visible hambuger button on desktops
3. display the current page's title
4. have a search icon
5. show a search bar when the search icon is pressed
6. hide the search bar when the close search icon is pressed
7. hide the search bar on load



### Remarks
The layout component will render a <router-outlet></router-outlet>. To render content, add the property: contentType="content".