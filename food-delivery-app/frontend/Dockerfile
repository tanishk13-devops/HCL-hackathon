# Build stage
FROM node:18 AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# Serve stage
FROM node:18-alpine
WORKDIR /app

RUN npm install -g http-server

COPY --from=build /app/dist/food-delivery-app ./dist

EXPOSE 4200

CMD ["http-server", "dist", "-p", "4200", "--cors"]
