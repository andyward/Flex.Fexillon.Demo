# Flex.Fexillon.Demo

A simple demo showing how to work with the Flex APIs from ASP.NET Core 5 using **Xbim.Flex.DI.Extensions**

This is demonstrate as a aset of simple OpenAPI services via a Swagger UI.

## Installing the Client Secret

Note this demo requires the setting of a Flex:ClientSecret key to acquire a access token. To keep this secret secure and outside of source control we recommend installing it locally using
`dotnet user secrets`

``` 
cd Flex.Fexillon.Demo
dotnet user-secrets init
dotnet user-secrets set "Flex:ClientSecret" '<yoursecret>'
```

## A note on security

This demo uses an Admin security approach, whereby the access token grants access to all data the application is allowed to see. In a real time application
you'd normallly impersonate lower priviledged users using `IFlexClientsProvider` (rather than `IFlexAdminClientsProvider`).
