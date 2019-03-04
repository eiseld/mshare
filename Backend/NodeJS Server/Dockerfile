FROM node:10.15.1

RUN mkdir /usr/src/app
WORKDIR /usr/src/app

ENV PATH /usr/src/app/node_modules/.bin:$PATH

#COPY package*.json ./
COPY package.json /usr/src/app/package.json

RUN npm install

COPY . .

EXPOSE 8081

CMD [ "npm", "start" ]