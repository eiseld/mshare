FROM microsoft/dotnet:2.2-sdk AS builder

WORKDIR /app

COPY MShare_ASP/MShare_ASP.csproj .

COPY . .

WORKDIR /app/MShare_ASP

RUN ["dotnet","publish","-c","Release","-o","publish"]

FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app

COPY --from=builder /app/MShare_ASP/publish .

CMD ["dotnet", "MShare_ASP.dll"]