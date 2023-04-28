# Backend Challenge Starter
This is a template project to save you the hassle of having to set up your project from scratch. 
You're free to change it however you want or start from scratch. Whatever is easiest for you.

# Running the project
_assumes you have .NET Core 6 installed_
1. Navigate to the backend challenge folder
```
cd ./BackendChallenge
```
2. Run the project
```
dotnet run
```
3. Verify it's working by navigating to this endpoint in your browser (you may need to change the port based on what address is shown when you run project).
It should display the text "working".
```
https://localhost:7076/test
```
4. Call user endpoint with UserToken in header
```
https://localhost:7076/users
```
5. Call learningPlan endpoint with UserToken in header
```
https://localhost:7076/learning-plan
```
6. Call incentives endpoint with UserToken in header
```
https://localhost:7076/incentives
```

Test Plan
<GetUsers API Endpoint>

1. Test case: Valid request returns 200 OK response and user data

    *Preconditions:
    There are active users for the company of the querying user in the database
    A valid user token is passed in the request header
    *Test steps:
        *Send a GET request to the users endpoint with a valid user token in the request header
        *Verify that the response status code is 200 OK
        *Verify that the response body contains an array of UserResponses including userId, firstName, and lastName
        *Verify that the user data in the response matches the user data in the database

2. Test case: Unauthorized request returns 401 Unauthorized response

Preconditions:

A user token is not passed in the request header or is empty/null
Test steps:

Send a GET request to the users endpoint without a user token in the request header
Verify that the response status code is 401 Unauthorized

3. Test case: Nonexistent user token returns 404 Not Found response

Preconditions:

A user token passed in the request header does not match any user token in the database
Test steps:

Send a GET request to the users endpoint with a nonexistent user token in the request header
Verify that the response status code is 404 Not Found

4. Test case: Cancelled request returns 500 Internal Server Error response

Preconditions:

The request is cancelled before completing the database query
Test steps:

Send a GET request to the users endpoint with a valid user token in the request header and a cancellation token
Cancel the request before the database query completes
Verify that the response status code is 500 Internal Server Error