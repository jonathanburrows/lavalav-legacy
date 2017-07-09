# Ontology
## Goals
Provide Generic interfaces and conventions for defining a domain.


## Creating a domain model
A domain model must inherit from `FluentNHibernate.Data.Entity`:
```cs
public class MyEntity : Entity { }
```


If it is going to be saved directly to the api (not as a child of another entity), then implement the `lvl.Ontology.IAggregateRoot` interface:
```cs
public class MyParentEntity : Entity, IAggregateRoot { }
```


If it is not going to be saved directly, and instead will be saved as part of a part entity's transaction, then implement the `lvl.Ontology.IAggregateScope` interface:
```cs
public class MyChildEntity : Entity, IAggregateScope<ParentEntity> { }

// Will be saved to the api, added for clarity
public class MyParentEntity : Entity, IAggregateRoot { }
```


## Conventions
### Foreign Key Id Convention
In order to allow for id references in the domain model (instead of object references), the foreign key property must be decorated with a `[ForeignKeyId]` attribute:
```cs
public class BusinessSpecificEntity: Entity, IAggregateRoot 
{
	[ForeignKeyId(typeof(Category))]
	public int CategoryId { get; set; }
}

public class Category: Entity, IAggregateRoot { }
```

When referencing an entity through a foreign key, it will not be cascaded to in any operations.



### Invlid Aggregate Root Convention
Will automatically be called at startup. When an object reference is made to a non-root object, not in the same scope, it will throw an exception.



### Max Length Convention
Will set the VARCHAR (or similar) in the database. By default, the length is 255. Decorate string properties with the standard [MaxLength] attribute.
```cs
public class MyEntity: Entity, IAggregateRoot 
{
	[MaxLength(2000)]
	public string ReallyLongTextField { get; set; }
}
```


### Required Convention
Ensures that non-nullable primatives, primatives marked with [Required], and strings marked with [Required] are set as _Not Null_ in the database.
```cs
public class EntityWithRequiredFields: Entity, IAggregateRoot 
{ 
	public int RequiredNotNullable {get; set; }

	[Required]
	public int? RequiredNullable { get; set; }
}
```



### Table Naming Convention
Used to override a tables name and schema.



### Unique Convention
Used to mark a column as unique in the database:
```cs
public class MyEntity: Entity, IAggregateRoot 
{ 
	[Unique]
	public string KeyIdentifier { get; set; }
}
```



### Config
Domain will have a single config, _ConnectionString_:
```
{
	domain: {
		// Connection String to be used by all the application.
		ConnectionString: string
	}
}
```


## Architecture
<img src="docs\ontology-architecture.png" />
