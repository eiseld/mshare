docker exec -it mshare-mysql bash -c "mysql -u root --execute=\"drop DATABASE if exists mshare;CREATE DATABASE mshare;\""
docker exec -it mshare-mysql bash -c "mysql -u root --execute=\"SET GLOBAL FOREIGN_KEY_CHECKS=0;\""
for file in dump/*.sql
do
docker exec -i mshare-mysql mysql -u root mshare < ${file}
docker cp ./dump mshare-mysql:/var/lib/mysql/
done
for file in dump/*.txt
do
docker exec -i mshare-mysql mysqlimport -u root --ignore --force mshare ${file}
done
docker exec -it mshare-mysql bash -c "mysql -u root --execute=\"SET GLOBAL FOREIGN_KEY_CHECKS=1;\""

