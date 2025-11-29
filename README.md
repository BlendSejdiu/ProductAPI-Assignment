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

Register user
```
POST /api/auth/Register
```

Logs in user
```
POST /api/auth/Login
```

Create refresh token
```
POST /api/auth/refresh-token
```

## Product Endpoints

All product endpoints require **JWT authentication**. Include the token in the header:

```
Authorization: Bearer {accessToken}
```

Create Product
```
POST /api/products
```
Get single Product
```
GET /api/products/{id}
```

Get Products by parameters
```
GET /api/products
```
**Query Parameters:**

- `pageNumber` (int, required) – Page number.
- `pageSize` (int, required) – Items per page.
- `search` (string, optional) – Search term for product name.
- `category` (string, optional) – Filter by category.
- `minPrice` (decimal, optional) – Minimum price filter.
- `maxPrice` (decimal, optional) – Maximum price filter.

Update Product
```
PUT /api/products/{id}
```
Delete Product
```
DELETE /api/products/{id}
```
