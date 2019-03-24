FROM node:11.12.0 as builder

WORKDIR /usr/src/app

COPY package.json .
RUN npm i

COPY . .

RUN npm run buildprod

FROM httpd:2.4 as apache-prod

COPY ./config/httpd.conf.prod /usr/local/apache2/conf/httpd.conf
COPY ./config/.htaccess /usr/local/apache2/htdocs/
COPY --from=builder /usr/src/app/dist/mshare /usr/local/apache2/htdocs

FROM httpd:2.4 as apache-dev

COPY ./config/httpd.conf.dev /usr/local/apache2/conf/httpd.conf
COPY ./config/.htaccess /usr/local/apache2/htdocs/
COPY --from=builder /usr/src/app/dist/mshare /usr/local/apache2/htdocs