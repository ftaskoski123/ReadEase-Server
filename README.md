# ReadEase 📕
Read Ease is your ideal book companion, crafted for avid readers like you. Its sleek, user-friendly interface provides features such as real-time updates on the New York Times' best books, a customizable "Read List," and personalized settings to enhance your reading experience.


## Features
- **Create Your Account:**  Sign up and create your personal account. You will receive an email notification upon successful registration.
- **Password Reset:**  Easily reset your password if you forget it.
- **New York Times Best Sellers:**  Stay updated with real-time updates on the New York Times' best seller books.
- **Manage Your Collection:** 
  - **Insert Books:** Add books to your collection.
  - **Search:** Find books by title, category, author, or any combination thereof, with pagination.
   (search filters are saved in session storage so when the user navigates through pages, the filter is saved).
  - **Download:** Download your entire collection or individual books.
  - **Edit Books:** Change the title, author, and category of any book.
  - **Delete Books:** Remove books from your collection.
- **Settings:**
  - **Update Email**: Update your email address (if it is not already in use).
  - **Update Password**: Change your password.
  - **Profile Picture**: Upload a new profile picture.
- **Admin Panel:**  Access an exclusive Admin panel (for Admin users only, which must be manually set).
  - **Manage Categories:** Insert new categories and delete existing categories.
  - **Manage Users:** View all users with pagination, search for users, and delete users.

## Technologies
### Backend
- **Programming Language:** C#
- **Framework:** .NET 8
- **Database:** MSSQL with T-SQL
- **Authentication:** Cookie Authentication
- **ORM:** Dapper
- **API:** REST APIs
- **Architecture:** MVC (Models, Views, Controllers)
### Frontend
- **Programming Language:** JavaScript, TypeScript
- **Framework/Library:** Vue.js
- **Styling:** Tailwind CSS
- **HTTP Client:** Axios
- **Build Tools:** Vite

## Explore the Code
- Check out the frontend code here: [ClientApp](https://github.com/ftaskoski123/ReadEase-Server/tree/master/ClientApp)
- Check out the project here: [ReadEase](https://readease.somee.com/) 
