# Portfolio API

This project implements a Portfolio API that integrates with GitHub.

## Setup

1.  **GitHub Token**: You need a Personal Access Token from GitHub.
2.  **User Secrets**: Store your GitHub credentials in User Secrets.

Run the following commands in the terminal (replace with your actual values):

```powershell
dotnet user-secrets set "GitHub:Token" "YOUR_GITHUB_TOKEN" --project Portfolio.API/Portfolio.API.csproj
dotnet user-secrets set "GitHub:UserName" "YOUR_GITHUB_USERNAME" --project Portfolio.API/Portfolio.API.csproj
```

## Running the Project

```powershell
dotnet run --project Portfolio.API/Portfolio.API.csproj
```

## API Endpoints

*   `GET /api/Portfolio`: Returns your portfolio repositories (cached).
*   `GET /api/Portfolio/search?name=...&language=...&user=...`: Search for public repositories.
