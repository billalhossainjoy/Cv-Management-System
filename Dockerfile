FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

COPY ["src/CVMS.Web/CVMS.Web.csproj", "src/CVMS.Web/"]
RUN dotnet restore "src/CVMS.Web/CVMS.Web.csproj"

COPY . .

WORKDIR "/src/CVMS.Web"
RUN dotnet publish "CVMS.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CVMS.Web.dll"]