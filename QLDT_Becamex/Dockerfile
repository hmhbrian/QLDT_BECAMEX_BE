# Chọn hình ảnh .NET SDK trên Alpine cho giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Thiết lập thư mục làm việc
WORKDIR /src

# Sao chép các tệp csproj và khôi phục các phụ thuộc
COPY QLDT_Becamex.csproj ./

RUN dotnet restore

# Sao chép mã nguồn và build ứng dụng
COPY . .
RUN dotnet publish "QLDT_Becamex.csproj" -c Release -o /app/publish

# Sử dụng hình ảnh .NET runtime Alpine (nhẹ hơn) cho giai đoạn runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

# Thiết lập thư mục làm việc
WORKDIR /app

# Sao chép ứng dụng đã build từ giai đoạn build vào image
COPY --from=build /app/publish .

# Chỉ định port 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "QLDT_Becamex.dll"]
