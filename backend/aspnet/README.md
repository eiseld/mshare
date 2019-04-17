# Getting started with ASP.NET

You can get started with just a basic text editor, the docker auto reload will do everything for you, however if you'd like a more sophisticated programming experience I recommend [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/) with ASP.NET Core package (you can install it when selecting workflows).

## Architecture of the backend

There are 6 things to keep in mind when adding a new feature.
* API Protocols
* Controllers
* Error handling
* Services
* Data objects / DBContext
* Dependency injection

### API Protocols
```API/Request``` and ```API/Response``` namespaces contain request and response objects to use with controllers. Every request must have an ```AbstractValidator``` validator, this automatically validates the syntax of the incoming request (e.g. Email is truly an email, password is truly a strong password).

### Controllers
Every controller must be derived from ```BaseController```.

> **_NOTE:_**  There is also ```ControllerBase```, don't mix them up!

Every controller has a ```[Route("[controller]")]``` class annotation that shows what the root path will be (e.g. for AuthController everything in it will be accessible from /auth/<...>).

Next comes the ```[ApiController]```. It tells the system that this is a controller meant to be used as an API.


 ```[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]``` Limits the access of this particular controller object for only authenticated users. The base class's ```GetCurrentUserID``` method can be used to access the current authenticated user.

The task of a controller object is to translate the user's request into business model actions, most of the time it should only call a service and return Ok with or without the result.

Member methods that have the ```        [AllowAnonymous]``` annotation can be accessed without authentication.

```[Route("{id}")]``` asks for a parameter in the route itself, if you want to get a complex object from the body or header user ```[FromBody]``` or ```[FromHeader]``` annotaion on the method.

Every other annotation should be trivial for the reader.

### Error Handling
```ErrorHandlingMiddleware``` catches all errors thrown inside a request and responds to the client accordingly. This is why you can safely return Ok in the controller, all errors thrown when processing a request will be caught and handled automatically.

Under the namespace ```MShare_ASP.Services.Exceptions``` there are currently these exceptions: ```DatabaseException```, ```BusinessException```, ```ResourceGoneException```, ```ResourceForbiddenException```, ```ResourceNotFoundException```.

### Services
Services are where the business logic goes. Every service is divided up with categories in mind. (e.g. AuthService is everything that has to do with authentication, EmailService only sends out emails).

When an error happens (user already registered, cannot find group with id, ...) an appropriate exception should be thrown.

> **_NOTE:_**  When creating a new service, make sure that the service's interface is in the namespace ```MShare_ASP.Services```.

### Data objects / DBContext
When modifying the database the appropriate steps have to be taken to implement the MySql schema into the [EF Core](https://docs.microsoft.com/en-us/ef/core/).

Every Table is represented by ```Dao<tablename>``` class with the appropriate tag (you can see a couple of examples in the code base already).

After that the tables have to be added to the ```MshareDbContext```.

### [Dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.2)

In a nutshell dependency injection takes the burden of initializing everything by hand and with the help of registering classes (```AddTransient```, ```AddSingleton```, ```AddDbContext```) the system does it automatically.

For example if you need the ```EmailService``` to be used in an other service, you simply list ```IEmailService``` as a constructor parameter and if the ```EmailService``` has been registered with ```IEmailService``` interface (```services.AddTransient<IEmailService, EmailService>()```) the dependency injection framework will find it automatically.