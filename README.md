### Usage
Newsy application was built using dotnet6. Postgre db should be running and 'newsy' db must be created.
To run newsy application locally there is buildAndRun.bat that can be used. It will execute db migrations, build and run application.
For testing endpoints Postman collection can be used. It is stored in foder 'Postman Collection'

### Endpoints
Newsy application has implementation for following endpoints:
- GET /api/article: to get all articles. Pagination and filtering can be done on articles. Both authors and readers can access this endpoint. Unpublished articles will be visible only to it's author.
- GET /api/article/{GUID}: to get article. Visibility same as for get articles.
- POST /api/article: to create article. Only users with author role can do it.
- PUT /api/article/{GUID}: to update article. Only author of that article can do this action.
- DELETE /api/article/{GUID}: to delete article. Only author of that article can do this action.

- GET /api/author: to get all authors. Pagination and filtering can be done on authors. Both authors and readers can access this endpoint.
- GET /api/author/{GUID}: to get author details and his articles. Unpublished articles visible only to author himself.
- PUT /api/author/{GUID}: to update author. Author can update only his profile.

- POST /api/user/register: allowed to unathorized users. User can register as author or reader.
- POST /api/user/login: allowed to unathorized users. Returns JWT token that can be used with all author and article endpoints.

### Functional improvements
Functional improvements for future:
- new role like Admin should be introduced. Currently any user can decide to be author on registration. In case there is management role like Admin this can be improved.
- article tags could be useful so readers can filter articles using tags.
- reading counter. We can show numerically popularity of articles.
- content internationalization.
- notification system - if user has it's favorire author, we can notify him when new article is published.

### Technical improvements
Technical improvements for future:
- secrets like connection string and key for generating JWT should be stored in Vault or any other security storage.
- caching can be added if analisys shows that it can improve speed without making bad expirience due to stale data.
- add logs and metrics.
- integration tests.
