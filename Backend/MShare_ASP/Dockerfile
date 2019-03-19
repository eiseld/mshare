FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["MShare_ASP/MShare_ASP.csproj", "MShare_ASP/"]
RUN dotnet restore "MShare_ASP/MShare_ASP.csproj"
COPY . .
WORKDIR "/src/MShare_ASP"
RUN dotnet build "MShare_ASP.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MShare_ASP.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MShare_ASP.dll"]