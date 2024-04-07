# Use the official .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose port 5000 to the outside world
EXPOSE 5031

# Command to run the application
ENTRYPOINT ["dotnet", "YourApiProjectName.dll"]
