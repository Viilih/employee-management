# Employee management
This is a repository to have CRUD operations for Departments, Coordinatos and employees 
and have their relationships.

### Technologies:
- .NET 8 
- C#
- Mediatr
- CQRS
- Fluent Results
- PostgresSql

### How to install and run the project

### Endpoints

#### /api/CreateCoordinator/CreateCoordinator

Request:
```json
{
  "firstName": "name",
  "lastName": "last name",
  "email": "email@test.com"
}
```

Response:
```json
{
  "coordinatorId": 2,
  "firstName": "name",
  "lastName": "last name",
  "email": "email@test.com",
  "departments": []
}
```