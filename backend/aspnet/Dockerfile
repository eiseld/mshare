FROM microsoft/dotnet:2.2-sdk AS builder

WORKDIR /src

COPY MShare_ASP/MShare_ASP.csproj MShare_ASP/

RUN dotnet restore MShare_ASP/MShare_ASP.csproj

COPY . .

WORKDIR /src/MShare_ASP

RUN dotnet build MShare_ASP.csproj -c Release -o /app

RUN dotnet publish "MShare_ASP.csproj" -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app

COPY --from=builder /app .

ENTRYPOINT ["dotnet", "MShare_ASP.dll"]
