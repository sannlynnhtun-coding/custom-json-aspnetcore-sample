# Custom JSON ASP.NET Core Sample

This sample shows how two .NET 10 console apps can read one shared JSON config file from the solution root.

## Project Layout

```text
custom-json-aspnetcore-sample/
  Config/
    custom-setting.json
  ProjectA/
    ProjectA.csproj
    Program.cs
  ProjectB/
    ProjectB.csproj
    Program.cs
  custom-json-aspnetcore-sample.slnx
```

`ProjectA` and `ProjectB` are separate console apps. Both load the same physical file:

```text
Config/custom-setting.json
```

## Config Flow

1. The app starts from `Program.cs`.
2. `Host.CreateApplicationBuilder(args)` creates the host builder.
3. `FindSharedConfigPath(...)` walks upward from the project content root.
4. The app finds `Config/custom-setting.json` at the solution root.
5. `AddJsonFile(...)` adds the shared JSON file to configuration.
6. `SampleConfig` is bound from the `SampleConfig` JSON section.
7. `SampleConfigService` receives the config through `IOptions<SampleConfig>`.
8. The service prints the config values and the project name.

## Shared Config

```json
{
  "SampleConfig": {
    "ApplicationName": "Custom JSON Console Sample",
    "IsEnabled": true,
    "RetryCount": 3,
    "Api": {
      "BaseUrl": "https://example.local/api",
      "TimeoutSeconds": 30
    }
  }
}
```

Changing `Config/custom-setting.json` affects both projects because both apps read the same root file.

## Run Workflow

Build the solution:

```powershell
dotnet build .\custom-json-aspnetcore-sample.slnx
```

Run `ProjectA`:

```powershell
dotnet run --project .\ProjectA\ProjectA.csproj
```

Run `ProjectB`:

```powershell
dotnet run --project .\ProjectB\ProjectB.csproj
```

## Expected Output

Each project prints the same config values, with its own project name:

```text
Project: ProjectA
Application: Custom JSON Console Sample
Enabled: True
Retry Count: 3
API Base URL: https://example.local/api
API Timeout Seconds: 30
```

`ProjectB` prints the same values with `Project: ProjectB`.

## Development Workflow

1. Update shared values in `Config/custom-setting.json`.
2. Run either project to confirm the new values are loaded.
3. Add new config properties to the JSON file.
4. Add matching properties to `SampleConfig` or nested config classes in each project.
5. Use `IOptions<SampleConfig>` in services that need the config.

Skipped a shared class library for this sample. Add one when the duplicated config classes become real application code.
