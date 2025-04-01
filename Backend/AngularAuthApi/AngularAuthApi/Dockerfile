# Use the .NET 6.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["AngularAuthApi/AngularAuthApi.csproj", "AngularAuthApi/"]
RUN dotnet restore "AngularAuthApi/AngularAuthApi.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/AngularAuthApi"

# Build the application
RUN dotnet build "AngularAuthApi.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AngularAuthApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
RUN mkdir -p /app/wwwroot
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "AngularAuthApi.dll"]

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "AngularAuthApi.dll"] 