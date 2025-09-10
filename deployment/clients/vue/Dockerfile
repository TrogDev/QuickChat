# The context folder must be the repository root
# docker build -t <repo>/quickchat-client-vue -f deployment/clients/vue/Dockerfile .

FROM node:24 AS build
WORKDIR /app
COPY src/clients/vue .
RUN npm install
RUN npm run build

FROM nginx:1.29.1-alpine as prod
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
