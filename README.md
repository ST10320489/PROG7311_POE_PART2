# AgriMarket

## Overview
AgriMarket is a role-based web application designed to connect farmers and employees through a streamlined product and blog management system. It features a user-friendly interface with authentication, role management, content creation, search, and filters for a modern online agricultural marketplace and blog platform, all backed by a real-time Supabase cloud database.

---

## Development Environment Setup

### Prerequisites
Ensure your system meets the following requirements before running the project:

#### Development Tools
- Visual Studio 2022 or later with ASP.NET and web development workload installed
- .NET 7.0 SDK or higher

#### Project Dependencies (NuGet Packages)
- `Postgrest-csharp` – v3.5.1  
- `Supabase` – v1.1.1  
- `Supabase.Postgrest` – v4.1.0  
- `System.Data.SQLite.Core` – v1.0.119  
- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` – v7.0.20  
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` – v7.0.20  
- `Microsoft.AspNetCore.Identity.UI` – v7.0.20  
- `Microsoft.EntityFrameworkCore.Sqlite` – v7.0.20  
- `Microsoft.EntityFrameworkCore.Tools` – v7.0.20  

#### Target Frameworks
- `Microsoft.AspNetCore.App` – v7.0.20  
- `Microsoft.NETCore.App` – v7.0.20  

### Browser Compatibility
- Compatible with modern browsers  
- Tested on the latest version of Google Chrome (please ensure your browser is updated)

---

## How to Build and Run the Prototype

1. Download and unzip the Visual Studio project folder submitted as a `.zip` file.  
2. Open Visual Studio.  
3. Drag and drop the unzipped folder into Visual Studio or open the `.sln` file manually via **File > Open > Project/Solution**.  
4. Visual Studio will auto-detect missing packages. Accept prompts to install dependencies if necessary.  
5. Click the **Run** or **Start** button (green play icon) to build and launch the project.  
6. The homepage will launch in your default browser.

### You can now explore:
- Marketplace  
- Blog  
- About  
- Contact  

### To access full dashboard functionality:
- **Option 1**: Register a new account (will be assigned the Farmer role).
- **Option 2**: Use pre-created test accounts:

  **Farmer**
  - Email: `farmer@gmail.com`  
  - Password: `123`  

  **Employee**
  - Email: `employee@gmail.com`  
  - Password: `123`  

### You may also:
- Register and log in as a new user, add and delete your own products/posts.  
- Log in as `employee@gmail.com` to remove test data or accounts.

---

## ⚠ Supabase Warning
This project connects to a **live cloud-based Supabase database**.

> Please avoid unnecessary deletions or changes, as they affect the entire system.  
You are encouraged to:
- Create accounts
- Add/edit/delete your own data  
- Clean up by logging into the **employee account**, searching by email, and deleting test accounts.

---

## System Functionalities and User Roles

### User Role: Farmer

#### Register and Login
- New users register with password confirmation and validation.
- Passwords are securely hashed.
- Assigned **Farmer** role by default.

#### Farmer Dashboard
- Add new products or blog posts
- Edit/delete own posts and products
- View success/error messages for each action
- Validation prevents empty submissions

---

### User Role: Employee

#### Employee Login
- Use pre-assigned credentials
- Can change password after login

#### Employee Dashboard
- Full CRUD for users, products, and posts
- Assign roles to new users
- Force password reset for updated users
- Cascade delete: remove user and their data
- Search users by email/name to see all related content

---

## Public Pages (Accessible Without Login)

- **Homepage**
- **About**
- **Contact**
- **Marketplace**
  - Shows all products
  - Filters by:
    - Name
    - Description
    - Category
    - Price range
    - Date range
  - Friendly message if no matches found
- **Blog**
  - Shows all posts
  - Searchable by name/description
- **Product/Blog Detail**
  - Name/Title
  - Content
  - Date
  - Author
  - Category
  - Image and other details

---

## Notes

- **Security**: Passwords are hashed. Role-based access is enforced.
- **UX**: All actions provide user feedback.
- **Error Handling**: Prevents invalid submissions and filter failures.
- **Accessibility**: Designed for both technical and non-technical users.

---

## Other Links

- 🔗 **GitHub Repository**: [https://github.com/ST10320489/PROG7311_POE_PART2](https://github.com/ST10320489/PROG7311_POE_PART2)  
- ▶️ **YouTube Demonstrations**:  
  - [Part 1](https://youtu.be/m3q9VzkN2gc)  
  - [Part 2](https://www.youtube.com/watch?v=Jbumf0pnEmc)

---

## References

- Wikipedia (2025). *Model–view–controller*. Available at: [Wikipedia MVC](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller)  
- Microsoft (2025). *ASP.NET Core – Open-source web development framework*. Available at: [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)  
- W3Schools (2025). *HTML Scripts Tutorial*. Available at: [W3Schools](https://www.w3schools.com/html/html_scripts.asp)
