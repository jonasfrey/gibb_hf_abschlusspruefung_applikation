# ---- Build-Stage: Vue/Vite-App zu statischen Dateien bauen ----
FROM node:24-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
# Nutzt .env.production -> VITE_API_BASE leer -> Aufrufe same-origin (/api/...)
RUN npm run build

# ---- Runtime-Stage: schlanker statischer File-Server ----
FROM nginx:alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
