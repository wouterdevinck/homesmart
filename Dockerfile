ARG ARCH=amd64

FROM node:16.13.2-bullseye-slim AS build-frontend
WORKDIR /app
COPY src/frontend ./
RUN npm install && npm run build

FROM mcr.microsoft.com/dotnet/sdk:6.0.101 AS build-backend
WORKDIR /app
COPY src/backend ./
RUN dotnet publish -c Release -o out /p:UseAppHost=false Home.Web

FROM mcr.microsoft.com/dotnet/aspnet:6.0.1-bullseye-slim-${ARCH}
WORKDIR /app
COPY --from=build-backend /app/out ./
COPY --from=build-frontend /app/dist ./wwwroot
EXPOSE 5000
USER nobody
ENTRYPOINT [ "dotnet", "Home.Web.dll", "--urls", "http://+:5000" ] 