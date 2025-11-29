# ProductAPISolution

This API provides user authentication, including registration, login, token refresh and product CRUD operations on products with filtering and pagination support. It uses **JWT authentication** for secure endpoints.

The API runs at:

```
https://localhost:724/api
```

Please change the connection string in the **appsettings.json** to the corresponding server from your machine.
There is no need to add any migrations just run the application since the DbInitializer will handle any migrations and create the database.
And make sure you have the corresponding .NET 8 SDK so that you can run the project on any IDE.

Endpoints for Authentication are:

/api/auth/Register - POST
/api/auth/Login - POST
/api/auth/RefreshToken - POST

Endpoints for Product are:

/api/product - POST
/api/products/{id} - GET single product
/api/products - GET with query params
**Query Parameters:**

- `pageNumber` (int, required) – Page number.
- `pageSize` (int, required) – Items per page.
- `search` (string, optional) – Search term for product name.
- `category` (string, optional) – Filter by category.
- `minPrice` (decimal, optional) – Minimum price filter.
- `maxPrice` (decimal, optional) – Maximum price filter.

/api/products/{id} - PUT
/api/products/{id} - DELETE
