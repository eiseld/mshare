docker exec -it mshare-mysql bash -c "mysql --user=root --password=ilovescrum --execute=\"drop DATABASE if exists mshare;CREATE DATABASE mshare;\""
docker exec -i mshare-mysql mysql --user=root --password=ilovescrum mshare < data.sql
PAUSE