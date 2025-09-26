# ========== Build ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /
# Sao chép csproj trước để tận dụng cache
COPY *.sln ./ 
COPY QLDT_Becamex/QLDT_Becamex.csproj ./ 
RUN dotnet restore ./QLDT_Becamex.csproj

# Sao chép toàn bộ mã nguồn và publish
COPY . .
RUN dotnet publish QLDT_Becamex/QLDT_Becamex.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== Runtime ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "QLDT_Becamex.dll"]