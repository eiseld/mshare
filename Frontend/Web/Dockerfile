#########################
### build environment ###
#########################

# base image
FROM node:10.15.1

# base image
FROM httpd:2.4

# Copy settings
COPY ./config/httpd.conf.dev /usr/local/apache2/conf/httpd.conf

# expose ports
EXPOSE 80
EXPOSE 4200

