# BookStoreDemo

## Setup Environment

Softwares for developing:

* VS 2022
* dotNet 8
* Docker (for testing container)
* WSL (for testing on Linux env)
* VSCode (for executing .http file, etc.)
  * [Rest Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) (for .http file. VS doesn't support it well.)
  
Environment variables:

* Set private/secret data with command: `dotnet user-secrets set "<key>" "<value>"`
  * Authentication:JwtSecretKey - the secret key for JWT token.
  * Authentication:DefaultUserName - the name of the default Admin user.
  * Authentication:DefaultUserEmail - the email of the default Admin user.
  * Authentication:DefaultUserPassword - the password of the default Admin user.

## APIs Usage Guide

Please refer to the Swagger docs: https://localhost:7072/swagger/index.html

## System Design

This is demo online book store backend service project. It's based on the ASP.NET Core Web API. It leveage the JWT toke for authentication and authization. The APIs access control is based on Role level. In this demo it just have two roles for now: `Admin` and `User`. Admin role can maintains the data like `Product`, `Category`, `User`'s role, etc. User role can access the APIs of `Cart` and `Order`. It uses an in memory database as the storage layer, and it can be replaced with other databases which can work with EntityFramework (this feature hasn't been implemented yet.). It's also not created for a real product, it does not support cache or distribution transaction etc., or other features to support high workload access.

## TODO List

* Add unit tests...
* Split the code in `Program.cs` into separated classes and functions.

