ARG ARCH=amd64

FROM mcr.microsoft.com/dotnet/sdk:6.0.101 AS build-backend
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out /p:UseAppHost=false src/backend/Home.Web

FROM mcr.microsoft.com/dotnet/aspnet:6.0.1-bullseye-slim-${ARCH}
WORKDIR /app
COPY --from=build-backend /app/out ./
EXPOSE 5000
USER nobody
ENTRYPOINT [ "dotnet", "web.dll", "--urls", "http://+:5000" ] 