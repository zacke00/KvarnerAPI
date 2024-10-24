welcome lets get started

We need to initialize the Database file for this build with 
*
dotnet ef migrations add InitialCreate
*

If we have somehow lost the Database file we need to update the database folder with a new one
*
dotnet ef database update
*

*
The authenticator we use is JwtBearer
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

This will ensure us that no one can add to the api without the proper authentication
