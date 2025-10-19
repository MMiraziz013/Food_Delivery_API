# 🧾 EXAM PROJECT: Food Delivery Platform API (with Permission-Based Authorization)

## 🎯 Objective

Develop a **RESTful API** for a food delivery platform using **C# (.NET 9)**, **Entity Framework Core**, and **PostgreSQL**, following **Clean Architecture** and applying **SOLID principles**.
Authorization must be **permission-based**, not only by role.

# Food Delivery Platform

## Project Description

Develop a RESTful API for a **Food Delivery Platform** using **C#**, **Entity Framework Core**, and **PostgreSQL**.

---

## Main Entities

### 1. Restaurants

| Field          | Type    | Description          |
| -------------- | ------- | -------------------- |
| Id             | int     | Unique identifier    |
| Name           | string  | Restaurant name      |
| Address        | string  | Restaurant address   |
| Rating         | decimal | Rating (1–5)         |
| WorkingHours   | string  | Working hours        |
| Description    | string  | Description          |
| ContactPhone   | string  | Contact phone number |
| IsActive       | bool    | Activity status      |
| MinOrderAmount | decimal | Minimum order amount |
| DeliveryPrice  | decimal | Delivery cost        |

---

### 2. Menu

| Field           | Type    | Description                   |
| --------------- | ------- | ----------------------------- |
| Id              | int     | Unique identifier             |
| RestaurantId    | int     | Linked restaurant ID          |
| Name            | string  | Dish name                     |
| Description     | string  | Dish description              |
| Price           | decimal | Dish price                    |
| Category        | string  | Dish category                 |
| IsAvailable     | bool    | Availability status           |
| PreparationTime | int     | Preparation time (in minutes) |
| Weight          | int     | Dish weight (in grams)        |
| PhotoUrl        | string  | Photo URL                     |

---

### 3. Users

| Field            | Type     | Description                     |
| ---------------- | -------- | ------------------------------- |
| Id               | int      | Unique identifier               |
| Name             | string   | User full name                  |
| Email            | string   | User email address              |
| Phone            | string   | Phone number                    |
| Password         | string   | Password hash                   |
| Address          | string   | Delivery address                |
| RegistrationDate | DateTime | Registration date               |
| Role             | enum     | Role (Client / Courier / Admin) |

---

### 4. Orders

| Field           | Type      | Description              |
| --------------- | --------- | ------------------------ |
| Id              | int       | Unique identifier        |
| UserId          | int       | Linked user ID           |
| RestaurantId    | int       | Linked restaurant ID     |
| CourierId       | int       | Linked courier ID        |
| OrderStatus     | enum      | Order status             |
| CreatedAt       | DateTime  | Order creation date      |
| DeliveredAt     | DateTime? | Delivery completion time |
| TotalAmount     | decimal   | Total order amount       |
| DeliveryAddress | string    | Delivery address         |
| PaymentMethod   | enum      | Payment method           |
| PaymentStatus   | enum      | Payment status           |

---

### 5. Order Details

| Field               | Type    | Description                     |
| ------------------- | ------- | ------------------------------- |
| Id                  | int     | Unique identifier               |
| OrderId             | int     | Linked order ID                 |
| MenuItemId          | int     | Linked menu item ID             |
| Quantity            | int     | Quantity ordered                |
| Price               | decimal | Price per item                  |
| SpecialInstructions | string  | Special instructions (optional) |

---

### 6. Couriers

| Field           | Type    | Description       |
| --------------- | ------- | ----------------- |
| Id              | int     | Unique identifier |
| UserId          | int     | Linked user ID    |
| Status          | enum    | Courier status    |
| CurrentLocation | string  | Current location  |
| Rating          | decimal | Rating (1–5)      |
| TransportType   | enum    | Type of transport |

---

## Entity Implementation (C#)

```csharp
public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public decimal Rating { get; set; }
    public string WorkingHours { get; set; }
    public string Description { get; set; }
    public string ContactPhone { get; set; }
    public bool IsActive { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal DeliveryPrice { get; set; }
}

public class Menu
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public bool IsAvailable { get; set; }
    public int PreparationTime { get; set; }
    public int Weight { get; set; }
    public string PhotoUrl { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public string Address { get; set; }
    public DateTime RegistrationDate { get; set; }
    public UserRole Role { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RestaurantId { get; set; }
    public int CourierId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string DeliveryAddress { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string SpecialInstructions { get; set; }
}

public class Courier
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public CourierStatus Status { get; set; }
    public string CurrentLocation { get; set; }
    public decimal Rating { get; set; }
    public TransportType TransportType { get; set; }
}
```

---

## Enumerations

```csharp
public enum UserRole
{
    Client,
    Courier,
    Admin
}

public enum OrderStatus
{
    Created,
    Confirmed,
    InProgress,
    ReadyForDelivery,
    OnDelivery,
    Delivered,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Card,
    OnlinePayment
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

public enum CourierStatus
{
    Active,
    Inactive,
    OnDelivery,
    OnBreak
}

public enum TransportType
{
    Bicycle,
    Motorcycle,
    Car,
    OnFoot
}
```

---

## Authentication & Authorization

### ✅ Authentication

- `POST /api/auth/register` — register new user
- `POST /api/auth/login` — issue **JWT token** containing user’s **role** and **permissions**

### ✅ Permission-Based Authorization

Each user role (Admin, Courier, Client) has a **set of permissions**.

1. Define permission constants (e.g., in `PermissionConstants.cs`)
2. Store permissions as **claims** in JWT
3. Implement custom `[PermissionAuthorize("PermissionName")]` attribute that checks user claims

#### Example:

```csharp
[PermissionAuthorize(PermissionConstants.Restaurants.View)]
[HttpGet("api/restaurants")]
public async Task<IActionResult> GetRestaurants() { ... }
```

### Example `PermissionConstants.cs`

```csharp
public static class PermissionConstants
{
    public static class Restaurants
    {
        public const string View = "Permissions.Restaurants.View";
        public const string Manage = "Permissions.Restaurants.Manage";
    }

    public static class Orders
    {
        public const string View = "Permissions.Orders.View";
        public const string Create = "Permissions.Orders.Create";
        public const string Manage = "Permissions.Orders.Manage";
    }

    public static class Couriers
    {
        public const string View = "Permissions.Couriers.View";
        public const string Manage = "Permissions.Couriers.Manage";
    }
}
```

### Role → Permissions mapping (example)

| Role        | Permissions                                             |
| ----------- | ------------------------------------------------------- |
| **Admin**   | All permissions                                         |
| **Courier** | Orders.View, Orders.Manage, Couriers.View               |
| **Client**  | Restaurants.View, Menu.View, Orders.Create, Orders.View |

---

## 🧱 Domain Entities

- **Restaurant**
- **Menu**
- **User**
- **Courier**
- **Order**
- **OrderDetail**

---

## ⚙️ Required Endpoints

### Auth

```
POST /api/auth/register
POST /api/auth/login
```

### Restaurants

```
GET    /api/restaurants             [PermissionAuthorize(Permissions.Restaurants.View)]
GET    /api/restaurants/{id}        [PermissionAuthorize(Permissions.Restaurants.View)]
POST   /api/restaurants             [PermissionAuthorize(Permissions.Restaurants.Manage)]
PUT    /api/restaurants/{id}        [PermissionAuthorize(Permissions.Restaurants.Manage)]
DELETE /api/restaurants/{id}        [PermissionAuthorize(Permissions.Restaurants.Manage)]
```

### Menu

```
GET    /api/menu                    [PermissionAuthorize(Permissions.Menu.View)]
POST   /api/menu                    [PermissionAuthorize(Permissions.Menu.Manage)]
PUT    /api/menu/{id}               [PermissionAuthorize(Permissions.Menu.Manage)]
DELETE /api/menu/{id}               [PermissionAuthorize(Permissions.Menu.Manage)]
```

### Orders

```
GET    /api/orders                  [PermissionAuthorize(Permissions.Orders.View)]
POST   /api/orders                  [PermissionAuthorize(Permissions.Orders.Create)]
PUT    /api/orders/{id}             [PermissionAuthorize(Permissions.Orders.Manage)]
DELETE /api/orders/{id}             [PermissionAuthorize(Permissions.Orders.Manage)]
```

### Couriers

```
GET    /api/couriers                [PermissionAuthorize(Permissions.Couriers.View)]
POST   /api/couriers                [PermissionAuthorize(Permissions.Couriers.Manage)]
PUT    /api/couriers/{id}           [PermissionAuthorize(Permissions.Couriers.Manage)]
DELETE /api/couriers/{id}           [PermissionAuthorize(Permissions.Couriers.Manage)]
```

---

# Analytics Tasks

Implement these queries in **Application Layer** (e.g., `AnalyticsService`).
Each must use EF Core LINQ and return DTOs.

1. `/api/restaurants/active` → Active restaurants sorted by rating
2. `/api/menu/available` → Available dishes where price < 1000
3. `/api/orders/by-status` → Count orders by status
4. `/api/menu/by-category` → Average price by category
5. `/api/users/with-orders` → Users with total order count
6. `/api/orders/by-courier/{id}` → Orders delivered by courier
7. `/api/orders/total-today` → Total amount of today’s orders
8. `/api/couriers/top-rated` → Top 5 couriers by rating
9. `/api/orders/expensive` → Orders above average total
10. `/api/analytics/restaurants/top` → Top 10 restaurants by order count (last month)

---

## 🧪 Bonus Tasks

- Implement `SeedPermissions()` for each role.
- Add pagination and filtering.
- Add unit tests for one service or controller (xUnit / Moq).
- Containerize with Docker.

---
