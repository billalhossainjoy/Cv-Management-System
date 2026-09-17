FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY ["src/CVMS.Domain/CVMS.Domain.csproj", "src/CVMS.Domain/"]
COPY ["src/CVMS.Application/CVMS.Application.csproj", "src/CVMS.Application/"]
COPY ["src/CVMS.Infrastructure/CVMS.Infrastructure.csproj", "src/CVMS.Infrastructure/"]
COPY ["src/CVMS.Web/CVMS.Web.csproj", "src/CVMS.Web/"]

RUN dotnet restore "src/CVMS.Web/CVMS.Web.csproj"

COPY . .

WORKDIR "/app/src/CVMS.Web"
RUN dotnet publish "CVMS.Web.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CVMS.Web.dll"]