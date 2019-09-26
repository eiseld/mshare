:: Delete database
docker exec -it mshare-mysql bash -c "mysql -u root --execute=\"drop DATABASE if exists mshare;CREATE DATABASE mshare;\""

:: Turn off foreign key checks
docker exec -it mshare-mysql bash -c "mysql -u root --execute=\"SET GLOBAL FOREIGN_KEY_CHECKS=0;\""

:: Import structure files
for /f %%f in ('dir /b dump\*.sql') do docker exec -i mshare-mysql mysql -u root mshare < dump/%%f

:: Copy data files to docker
docker cp ./dump mshare-mysql:/

:: Import data files
for /f %%f in ('dir /b dump\*.txt') do docker exec -i mshare-mysql mysqlimport -u root --ignore --force mshare /dump/%%f

:: Turn on foreign key checks
docker exec -it mshare-mysql bash -c "mysql -u root --execute=\"SET GLOBAL FOREIGN_KEY_CHECKS=1;\""

PAUSE