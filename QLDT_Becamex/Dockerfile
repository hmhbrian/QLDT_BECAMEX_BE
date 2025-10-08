FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src

# Cài thêm các package cần thiết để build
RUN apk add --no-cache bash libc6-compat icu-libs

# Copy csproj & restore
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source
COPY . .

# Build & publish (bỏ --no-restore)
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Cài package cho Unicode support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

ENTRYPOINT ["dotnet", "QLDT_Becamex.dll"]
