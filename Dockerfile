ARG ARCH=amd64

FROM node:16.13.2-bullseye-slim AS build-frontend
WORKDIR /app
COPY src/frontend ./
RUN npm install && npm run build

FROM mcr.microsoft.com/dotnet/sdk:7.0.101 AS build-backend
WORKDIR /app
COPY src/backend ./
RUN dotnet publish -c Release -o out /p:UseAppHost=false Home.Web

FROM mcr.microsoft.com/dotnet/aspnet:7.0.1-bullseye-slim-${ARCH} AS runtime
RUN apt-get update && \ 
    apt-get install -y --no-install-recommends \
      curl && \
    apt-get autoremove -yqq && \
    apt-get autoclean -yqq && \
    rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/* /var/cache/apt

FROM runtime
WORKDIR /app
COPY --from=build-backend /app/out ./
COPY --from=build-frontend /app/dist ./wwwroot
EXPOSE 5000
USER nobody
HEALTHCHECK --interval=60s --timeout=30s --start-period=60s --retries=3 \
  CMD curl --fail http://localhost:5000/status/live || exit 1
ENTRYPOINT [ "dotnet", "Home.Web.dll", "--urls", "http://+:5000" ]
