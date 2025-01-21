# Etapa 1: Construir o projeto Angular
FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --prod

# Etapa 2: Servir o projeto Angular com NGINX
FROM nginx:alpine
COPY --from=build /app/dist/app-angular /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]